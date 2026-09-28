using System;
using System.Collections.Generic;
using UnityEngine;

namespace VoronationCore
{
    public enum MatchOutcome { Running, PlayerWon, PlayerLost, Draw }

    public sealed class KnightState
    {
        private Vector2[] cell;

        public KnightId Id { get; }
        public Vector2 Position { get; internal set; }
        public float Power { get; internal set; }
        public float IncomeFactor { get; internal set; }
        public int Age { get; internal set; }
        public IReadOnlyList<Vector2> Cell => cell;
        public float Area { get; internal set; }

        public KnightState(KnightId id, Vector2 position, float power = 1f, float incomeFactor = 1f,
            int age = 0, IReadOnlyList<Vector2> cell = null, float area = 0f)
        {
            Id = id;
            Position = position;
            Power = power;
            IncomeFactor = incomeFactor;
            Age = age;
            SetCell(cell, area);
        }

        internal void SetCell(IReadOnlyList<Vector2> points, float area)
        {
            if (points == null || points.Count == 0) cell = Array.Empty<Vector2>();
            else
            {
                cell = new Vector2[points.Count];
                for (int i = 0; i < points.Count; i++) cell[i] = points[i];
            }
            Area = area;
        }

        internal KnightState DeepCopy() => new KnightState(Id, Position, Power, IncomeFactor, Age, cell, Area);
    }

    public sealed class FactionState
    {
        private readonly List<KnightState> knights;

        public FactionId Id { get; }
        public string Name { get; }
        public Color Color { get; }
        public bool IsPlayer { get; }
        public float Money { get; internal set; }
        public int NextKnightNumber { get; internal set; }
        public IReadOnlyList<KnightState> Knights => knights;

        public FactionState(FactionId id, string name, Color color, bool isPlayer, float money,
            int nextKnightNumber, IEnumerable<KnightState> knights)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Color = color;
            IsPlayer = isPlayer;
            Money = money;
            NextKnightNumber = nextKnightNumber;
            this.knights = new List<KnightState>(knights ?? throw new ArgumentNullException(nameof(knights)));
            this.knights.Sort((left, right) => left.Id.CompareTo(right.Id));
        }

        public KnightState FindKnight(KnightId id) => knights.Find(knight => knight.Id == id);
        internal void AddKnight(KnightState knight) => knights.Add(knight);
        internal bool RemoveKnight(KnightId id) => knights.RemoveAll(knight => knight.Id == id) == 1;

        internal FactionState DeepCopy()
        {
            var copies = new List<KnightState>(knights.Count);
            foreach (KnightState knight in knights) copies.Add(knight.DeepCopy());
            return new FactionState(Id, Name, Color, IsPlayer, Money, NextKnightNumber, copies);
        }
    }

    public sealed class MatchState
    {
        private readonly List<FactionState> factions;

        public int RoundNumber { get; internal set; }
        public int Seed { get; }
        public float HalfMapSize { get; }
        public FactionId HumanFactionId { get; }
        public MatchOutcome Outcome { get; internal set; }
        public IReadOnlyList<FactionState> Factions => factions;

        public MatchState(int roundNumber, int seed, float halfMapSize, FactionId humanFactionId,
            MatchOutcome outcome, IEnumerable<FactionState> factions)
        {
            RoundNumber = roundNumber;
            Seed = seed;
            HalfMapSize = halfMapSize;
            HumanFactionId = humanFactionId;
            Outcome = outcome;
            this.factions = new List<FactionState>(factions ?? throw new ArgumentNullException(nameof(factions)));
            this.factions.Sort((left, right) => left.Id.CompareTo(right.Id));
        }

        public FactionState FindFaction(FactionId id) => factions.Find(faction => faction.Id == id);
        public KnightState FindKnight(KnightId id) => FindFaction(id.FactionId)?.FindKnight(id);
        internal bool RemoveFaction(FactionId id) => factions.RemoveAll(faction => faction.Id == id) == 1;

        public MatchState DeepCopy()
        {
            var copies = new List<FactionState>(factions.Count);
            foreach (FactionState faction in factions) copies.Add(faction.DeepCopy());
            return new MatchState(RoundNumber, Seed, HalfMapSize, HumanFactionId, Outcome, copies);
        }
    }
}
