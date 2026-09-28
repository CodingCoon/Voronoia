using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoronationCore;

public class Voronation : MonoBehaviour, IVoronation
{
    private int leaderCount = 1;
    [SerializeField] private Leader preacherPrefab;
    private readonly List<Leader> leaders = new List<Leader>();
    private float displayedMoney;

    public FactionId Id { get; private set; }
    public bool HasIdentity { get; private set; }
    public string Name { get; private set; }
    public Color Color { get; private set; }
    public float Money => displayedMoney;
    public float DebtRelief { get; private set; }
    public ITactic Tactic { get; private set; }
    public bool IsPlayer => Tactic == null;
    public bool IsAi => Tactic != null;
    public override string ToString() => Name;

    public void Setup(string nationName, Color color, ITactic tactic, float money)
    {
        if (!PolygonGeometry.IsFinite(money)) throw new ArgumentException("Non-finite starting balance.");
        Name = nationName;
        Color = color;
        Tactic = tactic;
        displayedMoney = money;
    }

    internal void AssignIdentity(FactionId id)
    {
        if (HasIdentity && Id != id) throw new InvalidOperationException("Faction identity cannot change.");
        Id = id;
        HasIdentity = true;
    }

    public Leader AddPreacher(Vector2 position)
    {
        if (!HasIdentity) throw new InvalidOperationException("Faction must be registered before adding knights.");
        Leader leader = Instantiate(preacherPrefab, Vector2.zero, Quaternion.identity, transform);
        leader.Setup(leaderCount++, this, position);
        leaders.Add(leader);
        return leader;
    }

    internal Leader AddPreacherView(KnightState state)
    {
        Leader existing = leaders.Find(leader => leader.Id == state.Id);
        if (existing != null) return existing;
        Leader leader = Instantiate(preacherPrefab, Vector2.zero, Quaternion.identity, transform);
        leader.Setup(state.Id.Number, this, state.Position);
        leader.SyncState(state, true);
        leaders.Add(leader);
        leaderCount = Mathf.Max(leaderCount, state.Id.Number + 1);
        return leader;
    }

    internal FactionState CreateState()
    {
        if (!HasIdentity) throw new InvalidOperationException("Faction has no stable identity.");
        return new FactionState(Id, Name, Color, IsPlayer, displayedMoney, leaderCount,
            leaders.Select(leader => leader.CreateState()));
    }

    internal void SyncState(FactionState state, bool syncPositions)
    {
        displayedMoney = state.Money;
        leaderCount = state.NextKnightNumber;
        foreach (KnightState knight in state.Knights)
            AddPreacherView(knight).SyncState(knight, syncPositions);
    }

    internal void SetDisplayedMoney(float value) { displayedMoney = value; }
    internal void SetDebtRelief(float value) { DebtRelief = value; }

    public IEnumerable<Leader> GetLeaders() => leaders;
    internal Leader FindLeader(KnightId id) => leaders.Find(leader => leader.Id == id);
    internal bool RemoveLeaderView(Leader leader) => leaders.Remove(leader);

    public void Reset()
    {
        DebtRelief = 0;
        Tactic?.Clear();
        leaders.ForEach(leader => leader.Reset());
    }

    internal int GetLeaderCount() => leaders.Count;
}
