using System;

namespace VoronationCore
{
    public enum SessionPhase { Planning, Resolving, Presenting, AwaitingContinue, Ended, Faulted }

    public sealed class ActionPlannedEvent
    {
        public int RoundNumber { get; }
        public ActionCommand Command { get; }
        public ActionPlannedEvent(int roundNumber, ActionCommand command)
        {
            RoundNumber = roundNumber;
            Command = command;
        }
    }

    public sealed class MatchSession
    {
        private readonly RoundResolver resolver;
        public MatchState State { get; private set; }
        public RoundPlan Plan { get; private set; }
        public RoundResult LastResult { get; private set; }
        public SessionPhase Phase { get; private set; }
        public event Action<ActionPlannedEvent> ActionPlanned;
        public event Action<RoundResult> RoundResolved;

        public MatchSession(MatchState initialState, RoundResolver resolver)
        {
            State = initialState?.DeepCopy() ?? throw new ArgumentNullException(nameof(initialState));
            this.resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            Plan = new RoundPlan(State.RoundNumber);
            Phase = State.Outcome == MatchOutcome.Running ? SessionPhase.Planning : SessionPhase.Ended;
        }

        public CommandValidation PlanAction(ActionCommand command)
        {
            if (Phase != SessionPhase.Planning)
                return CommandValidation.Rejected(State.Outcome == MatchOutcome.Running
                    ? CommandRejection.WrongRound : CommandRejection.MatchEnded, command?.KnightId);
            CommandValidation validation = resolver.ValidateCommand(State, Plan.StateVersion, command);
            if (!validation.IsValid) return validation;
            RoundPlan candidate = Plan.Snapshot();
            candidate.AddOrReplace(command);
            validation = resolver.ValidateProjectedPositions(State, candidate, command.KnightId);
            if (!validation.IsValid) return validation;
            Plan.AddOrReplace(command);
            ActionPlanned?.Invoke(new ActionPlannedEvent(State.RoundNumber, command));
            return validation;
        }

        public bool RemovePlan(KnightId id) => Phase == SessionPhase.Planning && Plan.Remove(id);

        public RoundResult Resolve()
        {
            if (Phase != SessionPhase.Planning) throw new InvalidOperationException("Round cannot be resolved in " + Phase + ".");
            Phase = SessionPhase.Resolving;
            try
            {
                RoundResult result = resolver.Resolve(State, Plan);
                State = result.After.DeepCopy();
                LastResult = result;
                Phase = SessionPhase.Presenting;
                RoundResolved?.Invoke(result);
                return result;
            }
            catch
            {
                Phase = SessionPhase.Faulted;
                throw;
            }
        }

        public void PresentationCompleted()
        {
            if (Phase != SessionPhase.Presenting) throw new InvalidOperationException("Presentation is not active.");
            Phase = State.Outcome == MatchOutcome.Running ? SessionPhase.AwaitingContinue : SessionPhase.Ended;
        }

        public void Continue()
        {
            if (Phase != SessionPhase.AwaitingContinue) throw new InvalidOperationException("Round is not ready to continue.");
            Plan = new RoundPlan(State.RoundNumber);
            LastResult = null;
            Phase = SessionPhase.Planning;
        }
    }
}
