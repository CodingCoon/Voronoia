using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using VoronationCore;

public class Leader : MonoBehaviour, ILeader, IVoronoiCellOwner
{
    private static readonly Color NEGATIV = new Color(0.7882354f, 0.2745098f, 0.3960785f);
    private static readonly Color POSITIVE = new Color(0.2352941f, 0.5607843f, 0.482353f);
    private const int PRICE = -40;
    [SerializeField] private PreacherKnob knob;
    [SerializeField] private PreacherArea area;
    [SerializeField] private Animator animator;
    [SerializeField] private Prototype<VFX> vfxPrototype;
    [SerializeField] private TextMeshPro numberLabel;
    [SerializeField] private TextMeshPro income;
    private Tween incomeTween;
    private Tween dissolveTween;
    private float power = 1f;
    private float incomeFactor = 1f;
    private int age;
    private readonly List<IncomePosition> positions = new List<IncomePosition>();

    public KnightId Id { get; private set; }
    public IVoronation Voronation { get; private set; }
    public int Number => Id.Number;
    public int RoundsExist => age;
    public float Power => power;
    public float Income => incomeFactor;
    public IAction Action { get; private set; } = NoAction.NO_ACTION;
    public float RoundBalance { get; private set; }

    public override string ToString() => "Ritter " + Number + " (" + Voronation + ")";

    public void Setup(int number, IVoronation voronation, Vector2 position)
    {
        if (number < 1 || !PolygonGeometry.IsFinite(position) || !(voronation is Voronation faction) || !faction.HasIdentity)
            throw new ArgumentException("Invalid knight setup.");
        Id = new KnightId(faction.Id, number);
        name = "Leader #" + number;
        Voronation = voronation;
        knob.transform.position = new Vector3(position.x, position.y, -5);
        knob.Setup(voronation);
        area.Setup(voronation);
        numberLabel.text = number.ToString();
        numberLabel.gameObject.SetActive(!voronation.IsAi);
        UpdateAnimator();
        income.gameObject.SetActive(false);
    }

    internal KnightState CreateState() => new KnightState(Id, GetPosition(), power, incomeFactor, age,
        ToVector2(area.CopyBounds()), area.GetArea());

    internal void SyncState(KnightState state, bool syncPosition)
    {
        if (state.Id != Id) throw new InvalidOperationException("Cannot bind a different knight identity.");
        power = state.Power;
        incomeFactor = state.IncomeFactor;
        age = state.Age;
        if (syncPosition) knob.transform.position = new Vector3(state.Position.x, state.Position.y, knob.transform.position.z);
        UpdateAnimator();
    }

    public void Reset()
    {
        incomeTween?.Kill();
        positions.Clear();
        RoundBalance = 0;
        Action = NoAction.NO_ACTION;
        UpdateAnimator();
        income.gameObject.SetActive(false);
    }

    public void SetAction(IAction action)
    {
        action ??= NoAction.NO_ACTION;
        if (Game.INSTANCE != null)
        {
            if (!Game.INSTANCE.CanPlan) return;
            CommandValidation validation = Game.INSTANCE.PlanAction(ToCommand(action));
            if (!validation.IsValid) return;
        }
        Action = action;
        UpdateAnimator();
    }

    internal void ClearAction()
    {
        Action = NoAction.NO_ACTION;
        UpdateAnimator();
    }

    public bool HasAction() => Action != null && Action is not NoAction;

    internal void SetRoundAccounting(IEnumerable<RoundBooking> bookings)
    {
        positions.Clear();
        RoundBalance = 0;
        foreach (RoundBooking booking in bookings)
        {
            positions.Add(new IncomePosition(booking.Label, booking.Amount));
            RoundBalance += booking.Amount;
        }
    }

    public void Evaluate() { }

    public Tween ShowIncome()
    {
        incomeTween?.Kill();
        income.gameObject.SetActive(true);
        income.color = RoundBalance < 0 ? NEGATIV : POSITIVE;
        incomeTween = DOVirtual.Float(0, 1, 1.2f, progress =>
        {
            income.text = ((int)(RoundBalance * progress)).ToString();
            income.fontSize = Mathf.Lerp(10, 18, progress);
            income.rectTransform.anchoredPosition = Vector2.Lerp(Vector2.zero, new Vector2(0, 2), progress);
        }).SetEase(Ease.Linear).SetLink(gameObject);
        return incomeTween;
    }

    public int GetPrice() => PRICE * age;
    public Vector2 GetPosition() => knob.transform.position;
    public List<IncomePosition> GetPositions() => positions;
    public float GetArea() => area.GetArea();
    internal Vector3[] CopyBounds() => area.CopyBounds();

    public void HideKnob() { knob.HidePreview(); }
    public void UpdateVoronoi(List<Vector3> points) { area.SetBounds(points.ToArray()); }
    internal Tween AnimateMove(Vector2 target, float duration) => knob.AnimateMove(target, duration);
    internal Tween AnimateAppearance(float duration) => knob.AnimateAppearance(duration);

    public Tween AnimateRemoval()
    {
        CancelInteraction();
        dissolveTween?.Kill();
        dissolveTween = DOVirtual.Float(0, 1, 1, Dissolve).SetEase(Ease.Linear).SetLink(gameObject);
        return dissolveTween;
    }

    internal void Dissolve(float progress)
    {
        float scale = 1 - Mathf.Clamp01(progress);
        knob.transform.localScale = new Vector3(scale, scale, 1);
        area.Dissolve(progress);
    }

    public void CancelInteraction()
    {
        knob.CancelInteraction();
        animator.gameObject.SetActive(false);
    }

    public void CancelAnimations()
    {
        incomeTween?.Kill();
        dissolveTween?.Kill();
        knob.CancelAnimations();
        CancelInteraction();
    }

    private void OnDisable() { CancelAnimations(); }

    private void UpdateAnimator()
    {
        animator.gameObject.SetActive(!Voronation.IsAi && (Action == null || Action == NoAction.NO_ACTION));
    }

    public IEnumerator ShowVFX(string effectName)
    {
        VFX vfx = vfxPrototype.Create(transform, knob.transform.position);
        vfx.Play(effectName);
        yield return new WaitForSeconds(1f);
    }

    private ActionCommand ToCommand(IAction action)
    {
        ActionKind kind;
        Vector2? target = null;
        if (action is MoveAction move) { kind = ActionKind.Move; target = move.Target; }
        else if (action is SplitAction split) { kind = ActionKind.Split; target = split.Target; }
        else if (action is ImprovePowerAction) kind = ActionKind.ImprovePower;
        else if (action is IncreaseIncomeAction) kind = ActionKind.ImproveIncome;
        else if (action is NoAction) kind = ActionKind.None;
        else throw new ArgumentException("Unknown action type.", nameof(action));
        return new ActionCommand(Id.FactionId, Id, kind, target);
    }

    private static Vector2[] ToVector2(IReadOnlyList<Vector3> points)
    {
        var result = new Vector2[points.Count];
        for (int i = 0; i < points.Count; i++) result[i] = points[i];
        return result;
    }
}
