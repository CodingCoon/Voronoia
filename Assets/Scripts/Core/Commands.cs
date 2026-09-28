using System;
using System.Collections.Generic;
using UnityEngine;

namespace VoronationCore
{
    public enum ActionKind { None, Move, ImprovePower, ImproveIncome, Split }

    public sealed class ActionCommand
    {
        public FactionId OwnerId { get; }
        public KnightId KnightId { get; }
        public ActionKind Kind { get; }
        public Vector2? Target { get; }

        public ActionCommand(FactionId ownerId, KnightId knightId, ActionKind kind, Vector2? target = null)
        {
            OwnerId = ownerId;
            KnightId = knightId;
            Kind = kind;
            Target = target;
        }
    }

    public sealed class RoundPlan
    {
        private readonly List<ActionCommand> commands = new List<ActionCommand>();
        public int StateVersion { get; }
        public IReadOnlyList<ActionCommand> Commands => commands;

        public RoundPlan(int stateVersion) { StateVersion = stateVersion; }

        public void AddOrReplace(ActionCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            int index = commands.FindIndex(existing => existing.KnightId == command.KnightId);
            if (index < 0) commands.Add(command);
            else commands[index] = command;
        }

        public bool Remove(KnightId id) => commands.RemoveAll(command => command.KnightId == id) > 0;
        public ActionCommand Find(KnightId id) => commands.Find(command => command.KnightId == id);

        internal RoundPlan Snapshot()
        {
            var copy = new RoundPlan(StateVersion);
            foreach (ActionCommand command in commands) copy.commands.Add(command);
            return copy;
        }
    }

    public enum CommandRejection
    {
        None,
        WrongRound,
        MatchEnded,
        UnknownFaction,
        UnknownKnight,
        WrongOwner,
        InvalidAction,
        MissingTarget,
        UnexpectedTarget,
        NonFiniteTarget,
        TargetOutsideOwnCell,
        TargetTooCloseToBoundary,
        SplitTooCloseToParent,
        ConflictingEndPosition
    }

    public readonly struct CommandValidation
    {
        public bool IsValid => Reason == CommandRejection.None;
        public CommandRejection Reason { get; }
        public KnightId? KnightId { get; }

        private CommandValidation(CommandRejection reason, KnightId? knightId)
        {
            Reason = reason;
            KnightId = knightId;
        }

        public static CommandValidation Valid() => new CommandValidation(CommandRejection.None, null);
        public static CommandValidation Rejected(CommandRejection reason, KnightId? id = null) =>
            new CommandValidation(reason, id);
    }
}
