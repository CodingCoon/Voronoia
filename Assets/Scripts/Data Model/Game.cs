using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using VoronationCore;
using VoronationGeometry;

public class Game : MonoBehaviour
{
    public static Game INSTANCE;
    private readonly List<Voronation> voronations = new List<Voronation>();
    [SerializeField] private AbstractPhase initialPhase;
    [SerializeField] private MatchStatusPanel statusPanel;
    [SerializeField] private LeaderSelectionManager selection;
    [SerializeField] private PlannedActionController plannedActions;
    [SerializeField] private int matchSeed = 12345;
    private AbstractPhase currentPhase;
    private bool transitioning;
    private bool ready;
    private int phaseVersion;
    private int lastManualFrame = -1;
    private int nextFactionId = 1;
    private MatchSession session;

    public PhaseType PhaseType { get; private set; }
    public GameState State { get; private set; } = GameState.RUNNING;
    public bool IsFaulted { get; private set; }
    public MatchState MatchState => session?.State;
    public RoundResult CurrentResult => session?.LastResult;
    public bool CanPlan => !IsFaulted && !transitioning && PhaseType == PhaseType.ACTION &&
        State == GameState.RUNNING && session?.Phase == SessionPhase.Planning;
    public bool CanAdvance => !IsFaulted && !transitioning && ready &&
        (PhaseType == PhaseType.ACTION || PhaseType == PhaseType.DEATH);

    private void Awake() { INSTANCE = this; }

    private void Start()
    {
        Transition();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Increment00Diagnostics.SceneStarted(GetPreachers().Count);
#endif
    }

    public void InitializeMatch(float halfMapSize)
    {
        if (session != null) throw new InvalidOperationException("Match is already initialized.");
        if (voronations.Count == 0) throw new InvalidOperationException("Match needs at least one faction.");
        Voronation human = voronations.Find(nation => nation.IsPlayer);
        if (human == null) throw new InvalidOperationException("Match needs a human faction.");
        var initial = new MatchState(0, matchSeed, halfMapSize, human.Id, MatchOutcome.Running,
            voronations.Select(nation => nation.CreateState()));
        var resolver = new RoundResolver(new LegacyVoronoiCellCalculator());
        initial = resolver.CalculateInitialCells(initial);
        session = new MatchSession(initial, resolver);
        SyncActiveViews(initial, true);
        ApplyStateCells(initial);
    }

    public CommandValidation PlanAction(ActionCommand command)
    {
        if (session == null) return CommandValidation.Rejected(CommandRejection.WrongRound, command?.KnightId);
        return session.PlanAction(command);
    }

    internal void RemovePlannedAction(KnightId id) { session?.RemovePlan(id); }

    public void NextPhase()
    {
        if (!CanAdvance || lastManualFrame == Time.frameCount) return;
        if (GameManager.Instance.IsTutorial && !TutorialManager.Instance.NextRoundButtonEnabled) return;
        lastManualFrame = Time.frameCount;
        Transition();
    }

    private void Transition()
    {
        if (transitioning || IsFaulted) return;
        transitioning = true;
        ready = false;
        try
        {
            currentPhase?.OnEnd();
            AbstractPhase next = currentPhase == null ? initialPhase : currentPhase.GetNextPhase();
            if (next == null) throw new InvalidOperationException("Missing next phase reference.");
            currentPhase = next;
            phaseVersion++;
            PhaseType = next.GetPhaseType();
            if (PhaseType == PhaseType.ACTION)
            {
                if (session?.Phase == SessionPhase.AwaitingContinue) session.Continue();
                foreach (Voronation nation in voronations) nation.Reset();
            }
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (PhaseType == PhaseType.APPLY) Increment00Diagnostics.RoundStarted(GetPreachers().Count);
#endif
            next.OnStart();
            ready = PhaseType == PhaseType.ACTION;
        }
        catch (Exception exception) { Fail(exception); }
        finally { transitioning = false; }
    }

    public void RunPhase(AbstractPhase owner, IEnumerator routine, bool advance = true)
    {
        if (owner != currentPhase || IsFaulted) return;
        StartCoroutine(RunGuarded(owner, phaseVersion, routine, advance));
    }

    private IEnumerator RunGuarded(AbstractPhase owner, int version, IEnumerator routine, bool advance)
    {
        yield return null;
        var pending = new Stack<IEnumerator>();
        pending.Push(routine);
        try
        {
            while (pending.Count > 0 && !IsFaulted && owner == currentPhase && version == phaseVersion)
            {
                object yielded = null;
                Exception failure = null;
                bool moved = false;
                try
                {
                    IEnumerator top = pending.Peek();
                    moved = top.MoveNext();
                    if (moved) yielded = top.Current;
                    else (pending.Pop() as IDisposable)?.Dispose();
                }
                catch (Exception exception) { failure = exception; }
                if (failure != null) { Fail(failure); yield break; }
                if (!moved) continue;
                if (yielded is IEnumerator nested) pending.Push(nested);
                else yield return yielded;
            }
            if (IsFaulted || owner != currentPhase || version != phaseVersion) yield break;
            if (advance || (PhaseType == PhaseType.DEATH && IsOver())) Transition();
            else ready = true;
        }
        finally
        {
            while (pending.Count > 0) (pending.Pop() as IDisposable)?.Dispose();
        }
    }

    internal RoundResult ResolveRound()
    {
        if (session == null) throw new InvalidOperationException("Match is not initialized.");
        RoundResult result = session.Resolve();
        State = ToGameState(result.After.Outcome);
        Debug.Log(result.ToLogText());
        return result;
    }

    internal IEnumerator PresentActions()
    {
        RoundResult result = CurrentResult ?? throw new InvalidOperationException("No round result to present.");
        foreach (ActionCommand command in result.Commands)
        {
            Leader leader = FindLeader(command.KnightId);
            if (leader == null) throw new InvalidOperationException("Missing knight view for " + command.KnightId + ".");
            switch (command.Kind)
            {
                case ActionKind.None:
                    break;
                case ActionKind.Move:
                    SoundManager.PlaySound("Move Leader");
                    yield return TweenPlayback.Wait(leader.AnimateMove(command.Target.Value, 1f));
                    break;
                case ActionKind.ImprovePower:
                case ActionKind.ImproveIncome:
                    SoundManager.PlaySound("Improve Leader");
                    yield return leader.ShowVFX(command.Kind == ActionKind.ImprovePower ? "Improve Power" : "Improve Income");
                    break;
                case ActionKind.Split:
                    RoundEvent created = result.Events.First(item => item.Kind == RoundEventKind.KnightCreated &&
                        item.RelatedKnightId == command.KnightId);
                    KnightState childState = result.After.FindKnight(created.KnightId.Value);
                    Voronation faction = FindVoronation(command.OwnerId);
                    Leader child = faction.AddPreacherView(childState);
                    SoundManager.PlaySound("Split Leader");
                    leader.HideKnob();
                    yield return TweenPlayback.Wait(child.AnimateAppearance(1f));
                    break;
            }
        }
        SyncActiveViews(result.After, false);
        foreach (Leader leader in GetPreachers()) leader.ClearAction();
    }

    internal void ApplyAccountingCells()
    {
        if (CurrentResult == null) throw new InvalidOperationException("No round result.");
        ApplyCells(CurrentResult.AccountingCells);
    }

    internal void PrepareEvaluation()
    {
        RoundResult result = CurrentResult ?? throw new InvalidOperationException("No round result.");
        foreach (Leader leader in GetPreachers())
            leader.SetRoundAccounting(result.Bookings.Where(booking => booking.KnightId == leader.Id &&
                booking.Kind != BookingKind.DebtRelief));
        foreach (FactionRoundSummary summary in result.FactionSummaries)
            FindVoronation(summary.FactionId)?.SetDisplayedMoney(summary.MoneyAfterAccounting);
    }

    internal IEnumerator PresentDeaths(EvaluationPanel evaluationPanel)
    {
        RoundResult result = CurrentResult ?? throw new InvalidOperationException("No round result.");
        foreach (RoundEvent roundEvent in result.Events.Where(item => item.Kind == RoundEventKind.KnightRemoved))
        {
            Leader leader = FindLeader(roundEvent.KnightId.Value);
            Voronation faction = FindVoronation(roundEvent.FactionId.Value);
            RoundBooking relief = result.Bookings.First(booking => booking.Kind == BookingKind.DebtRelief &&
                booking.KnightId == roundEvent.KnightId);
            faction.SetDebtRelief(relief.Amount);
            faction.SetDisplayedMoney(0);
            if (faction.IsPlayer) evaluationPanel.ShowDebtRelief(faction, leader.Number);
            yield return TweenPlayback.Wait(leader.AnimateRemoval());
            faction.RemoveLeaderView(leader);
            Destroy(leader.gameObject);
        }

        foreach (RoundEvent roundEvent in result.Events.Where(item => item.Kind == RoundEventKind.FactionRemoved))
        {
            Voronation faction = FindVoronation(roundEvent.FactionId.Value);
            if (faction == null) continue;
            voronations.Remove(faction);
            Destroy(faction.gameObject);
        }

        SyncActiveViews(result.After, true);
        ApplyStateCells(result.After);
        session.PresentationCompleted();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Increment00Diagnostics.RoundFinished(GetPreachers().Count);
#endif
    }

    public void ClearInteraction()
    {
        selection.UpdateLeader(null);
        plannedActions.UnPlan();
        foreach (Leader leader in GetPreachers()) leader.CancelInteraction();
    }

    public void Fail(Exception exception)
    {
        if (IsFaulted) return;
        IsFaulted = true;
        ready = false;
        phaseVersion++;
        currentPhase?.Cancel();
        foreach (Leader leader in GetPreachers()) leader.CancelAnimations();
        ClearInteraction();
        Debug.LogError("Round stopped in " + PhaseType + ": " + exception);
        statusPanel.ShowError();
    }

    public void ShowResult() { statusPanel.ShowResult(State); }

    private void OnDisable()
    {
        phaseVersion++;
        ready = false;
        StopAllCoroutines();
        currentPhase?.Cancel();
        foreach (Leader leader in GetPreachers()) leader.CancelAnimations();
        if (INSTANCE == this) INSTANCE = null;
    }

    public void AddReligion(Voronation religion)
    {
        if (voronations.Contains(religion)) return;
        religion.AssignIdentity(new FactionId(nextFactionId++));
        voronations.Add(religion);
    }

    internal void RemoveReligion(Voronation religion) { voronations.Remove(religion); }

    internal void UpdateGameState()
    {
        bool playerActive = false, aiActive = false;
        foreach (Voronation nation in voronations)
        {
            if (nation.GetLeaderCount() == 0) continue;
            playerActive |= nation.IsPlayer;
            aiActive |= nation.IsAi;
        }
        State = playerActive ? (aiActive ? GameState.RUNNING : GameState.PLAYER_WON) :
            (aiActive ? GameState.PLAYER_LOOSE : GameState.DRAW);
    }

    public List<Leader> GetPreachers()
    {
        var result = new List<Leader>();
        foreach (Voronation nation in voronations) result.AddRange(nation.GetLeaders());
        return result;
    }

    public List<Voronation> GetVoronations() => new List<Voronation>(voronations);
    internal bool IsOver() => State != GameState.RUNNING;
    internal Voronation GetHumanPlayer() => voronations.Find(nation => nation.IsPlayer);
    internal FactionState GetFactionState(FactionId id) => session?.State.FindFaction(id);

    private Leader FindLeader(KnightId id) => FindVoronation(id.FactionId)?.FindLeader(id);
    private Voronation FindVoronation(FactionId id) => voronations.Find(nation => nation.Id == id);

    private void SyncActiveViews(MatchState state, bool syncPositions)
    {
        foreach (FactionState factionState in state.Factions)
        {
            Voronation faction = FindVoronation(factionState.Id);
            if (faction == null) throw new InvalidOperationException("Missing faction view for " + factionState.Id + ".");
            faction.SyncState(factionState, syncPositions);
        }
    }

    private void ApplyStateCells(MatchState state)
    {
        foreach (FactionState faction in state.Factions)
            foreach (KnightState knight in faction.Knights)
                FindLeader(knight.Id).UpdateVoronoi(ToVector3(knight.Cell));
    }

    private void ApplyCells(IReadOnlyList<CalculatedCell> cells)
    {
        foreach (CalculatedCell cell in cells)
        {
            Leader leader = FindLeader(cell.KnightId);
            if (leader == null) throw new InvalidOperationException("Missing cell view for " + cell.KnightId + ".");
            leader.UpdateVoronoi(ToVector3(cell.Points));
        }
    }

    private static List<Vector3> ToVector3(IReadOnlyList<Vector2> points)
    {
        var result = new List<Vector3>(points.Count);
        for (int i = 0; i < points.Count; i++) result.Add(points[i]);
        return result;
    }

    private static GameState ToGameState(MatchOutcome outcome)
    {
        switch (outcome)
        {
            case MatchOutcome.PlayerWon: return GameState.PLAYER_WON;
            case MatchOutcome.PlayerLost: return GameState.PLAYER_LOOSE;
            case MatchOutcome.Draw: return GameState.DRAW;
            default: return GameState.RUNNING;
        }
    }

    public enum GameState { RUNNING, PLAYER_WON, PLAYER_LOOSE, DRAW }
}
