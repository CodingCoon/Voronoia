using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using static MenuButton;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    private static Action NO_OP = () => { };
    private static string DESCRIPTION_1 = "You play Voronation, a strategy game, where you control leaders to claim regions on a square map.";
    private static string DESCRIPTION_2 = "This pulsing dot is you leader and the area around him you land. In a Voronation every land is formed around your leader. Every location in that land has a shorter range to you leader than to any other leader. Your target it to exterminate any other nation.";
    private static string DESCRIPTION_3 = "You can move your leader to effect you land from another position. Click right on your leader and move him to another position.";
    private static string DESCRIPTION_4 = "Click on 'Apply actions' to execute the action.";

    private static string DESCRIPTION_5 = "Now all leaders are doing their action...";
    private static string DESCRIPTION_6 = "...and the land around them changes depending on the Voronation rules.";
    private static string DESCRIPTION_7 = "After that every Voronation gets income depending on the size of their land. In addition you have to pay for actions and the leader itself. Every round a leader gets more expensive.";
    private static string DESCRIPTION_8 = "If you can`t pay a leader, he will leave you and the land around him is lost. If the last leader has left you are game over. Click 'Next Round'!";
    private static string DESCRIPTION_9 = "As another action you may raise the power of your leader, which forces more pressure against other leader or you may increase the income of a leader to get more money from the land around him. Choose one of these actions!";

    private static string DESCRIPTION_10 = "Again all actions get applied, the land is divided up and income is collected.";
    private static string DESCRIPTION_11 = "But another player can't pay his leader. The player is out of the game. The land stays unclaimed till next round.";
    private static string DESCRIPTION_12 = "As the last action you are able to split your leader to have more than one. The newly created will gain its own land.";
    private static string DESCRIPTION_13 = "Do a split action with your leader! But be careful the new leader costs also money, which increases by time.";

    private static string DESCRIPTION_14 = "Now you have learned all actions. Go back to the menu to play a normal game or fight this tutorial until the bitter end. Good luck.";

    [SerializeField] private TutorialHelper tutorialHelper;

    public bool NextRoundButtonEnabled {  get; private set; }
    public List<ActionType> DisabledActionTypes { get; private set; } = new List<ActionType>();

    public bool TutorialDone { get; private set; } = false;

    private List<Hint> hints = new List<Hint>();
    private Hint currentHint;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (GameManager.Instance.IsTutorial)
        {
            Hint WELCOME_TO_VORONOI_DESCRIPTION = Hint.OfSkippable(DESCRIPTION_1, () =>
            {
                DisabledActionTypes.Clear();
                NextRoundButtonEnabled = false;
                DisabledActionTypes.Add(ActionType.IMPROVE_POWER);
                DisabledActionTypes.Add(ActionType.IMPROVE_INFLUENCE);
                DisabledActionTypes.Add(ActionType.SPLIT);
            });
            Hint SHORT_RULE_DESCRIPTION = Hint.OfSkippable(DESCRIPTION_2);
            Hint MOVE_LEADER_TASK = Hint.OfTask(DESCRIPTION_3, () => tutorialHelper.PlayersLeaderHasAction());
            Hint END_ROUND_TASK = Hint.OfTask(DESCRIPTION_4, () => NextRoundButtonEnabled = true, () => tutorialHelper.IsApplyPhase());
            
            Hint SEE_APPLY_PHASE = Hint.OfTask(DESCRIPTION_5, () => tutorialHelper.IsEvaluationPhase());
            Hint SEE_VORONOI_PHASE= Hint.OfTask(DESCRIPTION_6, () => tutorialHelper.IsEvaluationPhase());
            Hint SEE_EVALUATION_PHASE = Hint.OfSkippable(DESCRIPTION_7, () => NextRoundButtonEnabled = false);
            Hint LEARN_GAME_OVER_CONDITION = Hint.OfTask(DESCRIPTION_8, () => NextRoundButtonEnabled = true, () => tutorialHelper.IsActionPhase());
            Hint IMPROVE_LEADER_TASK = Hint.OfTask(DESCRIPTION_9, () => 
            {
                DisabledActionTypes.Clear();
                NextRoundButtonEnabled = false;
                DisabledActionTypes.Add(ActionType.MOVE);
                DisabledActionTypes.Add(ActionType.SPLIT);
            },
            () => tutorialHelper.PlayersLeaderHasAction());
            Hint SEE_ALL_PHASES = Hint.OfTask(DESCRIPTION_10, () => tutorialHelper.IsEvaluationPhase());
            Hint SEE_DEATH = Hint.OfTask(DESCRIPTION_11, () => tutorialHelper.IsActionPhase());

            Hint SPLIT_DESCRIPTION = Hint.OfSkippable(DESCRIPTION_12, () =>
            {
                print("SPLIT LEADER !!!");
                DisabledActionTypes.Clear();
                NextRoundButtonEnabled = false;
                DisabledActionTypes.Add(ActionType.MOVE);
                DisabledActionTypes.Add(ActionType.IMPROVE_POWER);
                DisabledActionTypes.Add(ActionType.IMPROVE_INFLUENCE);
            });
            Hint SPLIT_LEADER_TASK = Hint.OfTask(DESCRIPTION_13, () => tutorialHelper.PlayersLeaderHasAction());

            Hint DONE = Hint.OfSkippable(DESCRIPTION_14, () => TutorialDone = true);


            hints.Add(WELCOME_TO_VORONOI_DESCRIPTION);
            hints.Add(SHORT_RULE_DESCRIPTION);
            hints.Add(MOVE_LEADER_TASK);
            hints.Add(END_ROUND_TASK);

            hints.Add(SEE_APPLY_PHASE);
            hints.Add(SEE_VORONOI_PHASE);
            hints.Add(SEE_EVALUATION_PHASE);
            hints.Add(LEARN_GAME_OVER_CONDITION);
            hints.Add(IMPROVE_LEADER_TASK);
            hints.Add(END_ROUND_TASK);

            hints.Add(SEE_ALL_PHASES);
            hints.Add(SEE_DEATH);
            hints.Add(SPLIT_DESCRIPTION);
            hints.Add(SPLIT_LEADER_TASK);
            hints.Add(END_ROUND_TASK);
            hints.Add(SEE_ALL_PHASES);
            hints.Add(DONE);

            ShowNext();
        }
    }

    private void Update()
    {
        if (currentHint != null)
        {
            if (currentHint.IsDone())
            {
                ShowNext();
            }
        }
    }

    public bool HasNext()
    {
        return currentHint.IsSkippable;
    }

    public void Next()
    {
        ShowNext();
    }

    private void ShowNext()
    {
        if (hints.Count > 0)
        {
            currentHint = hints[0];
            hints.RemoveAt(0);
            currentHint.Show();
        }
        else
        {
            GameManager.Instance.IsTutorial = false;

        }
    }

    internal string GetHint()
    {
        return currentHint.hint;
    }

    public class Hint
    {
        public readonly string hint;
        private readonly Action onShow;
        
        public readonly bool isTask;
        public bool IsSkippable => !isTask;
        
        private readonly Func<bool> taskConditions;

        public static Hint OfSkippable(string hintDescription)
        {
            return OfSkippable(hintDescription, NO_OP);
        }

        public static Hint OfSkippable(string hintDescription, Action onShow)
        {
            return new Hint(hintDescription, onShow, false, () => false);
        }

        public static Hint OfTask(string hintDescription, Func<bool> taskConditions)
        {
            return OfTask(hintDescription, NO_OP, taskConditions);
        }

        public static Hint OfTask(string hintDescription, Action onShow, Func<bool> taskConditions)
        {
            return new Hint(hintDescription, onShow, true, taskConditions);
        }

        private Hint(string hintDescription, Action onShow, bool isTask, Func<bool> taskCondition)   
        {
            this.hint = hintDescription;
            this.onShow = onShow;
            this.isTask = isTask;
            this.taskConditions = taskCondition;
        }

        public void Show()
        {
            onShow.Invoke();
        }

        public bool IsDone()
        {
            return isTask && taskConditions.Invoke();
        }
    }
}
