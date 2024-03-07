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

    [SerializeField] private TutorialHelper tutorialHelper;

    public bool NextRoundButtonEnabled {  get; private set; }
    public List<ActionType> DisabledActionTypes { get; private set; } = new List<ActionType>();

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
            Hint WELCOME_TO_VORONOI_DESCRIPTION = new Hint(DESCRIPTION_1, true, () =>
            {
                NextRoundButtonEnabled = false;
                DisabledActionTypes.Add(ActionType.IMPROVE_POWER);
                DisabledActionTypes.Add(ActionType.IMPROVE_INFLUENCE);
                DisabledActionTypes.Add(ActionType.SPLIT);
            });
            Hint SHORT_RULE_DESCRIPTION = new Hint(DESCRIPTION_2, true);
            Hint MOVE_LEADER_TASK = new Hint(DESCRIPTION_3, () => tutorialHelper.PlayersLeaderHasAction(), () => NextRoundButtonEnabled = true);
            Hint END_ROUND_TASK = new Hint(DESCRIPTION_4, () => tutorialHelper.IsApplyPhase(), NO_OP);

            hints.Add(WELCOME_TO_VORONOI_DESCRIPTION);
            hints.Add(SHORT_RULE_DESCRIPTION);
            hints.Add(MOVE_LEADER_TASK);
            hints.Add(END_ROUND_TASK);

            ShowNext();
        }
    }

    private void Update()
    {
        if (currentHint != null)
        {
            if (currentHint.IsDone())
            {
                currentHint.Done();
            }
        }
    }

    public bool HasNext()
    {
        return currentHint.isSkippable && hints.Count > 0;
    }

    public void Next()
    {
        ShowNext();
    }

    private void ShowNext()
    {
        currentHint = hints[0];
        hints.RemoveAt(0);
        currentHint.Show();
    }

    internal string GetHint()
    {
        return currentHint.hint;
    }


    // shown directly after 
    // Wenn erste Runde, block Weiter, block andere Aktionen
    private static string DESCRIPTION_1 = "You play Voronation, a strategy game, where you control leaders to claim regions on a square map.";
    private static string DESCRIPTION_2 = "This pulsing dot is you leader and the area around him you land. In a Voronation every land is formed around your leader. Every location in that land has a shorter range to you leader than to any other leader. Your target it to exterminate any other nation.";
    private static string DESCRIPTION_3 = "You can move your leader to effect you land from another position. Click right on your leader and move him to another position.";

    // Wenn erste Runde und Leader hat Aktion
    private static string DESCRIPTION_4 = "Click on 'Next Round' to execute the action.";



    public class Hint
    {
        public string hint;
        private bool firstShow = true;
        private Action firstShowActions;
        
        public readonly bool isSkippable;       // whether it is skippable by a button or a task needs to be done first
        
        private Action doneActions;
        public bool done = false;
        public readonly bool isTask;
        private Func<bool> taskConditions;

        public Hint(string hintDescription, bool skippable) : this(hintDescription, skippable, NO_OP) { }

        public Hint(string hintDescription, Func<bool> taskCondition, Action doneActions)
        {
            this.hint = hintDescription;
            this.isSkippable = false;
            this.firstShowActions = NO_OP;
            this.done = true;
            this.doneActions = doneActions;
            this.isTask = true;
            this.taskConditions = taskCondition;
        }

        public Hint(string hintDescription, bool skippable, Action firstShowActions)
        {
            this.hint = hintDescription;
            this.isSkippable = skippable; 
            this.firstShowActions = firstShowActions;
            this.done = true;
            this.doneActions = NO_OP;
            this.isTask = false;
        }

        public void Show()
        {
            if (firstShow)
            {
                firstShowActions.Invoke();
                firstShow = false;
            }
        }

        public bool IsDone()
        {
            return isTask && taskConditions.Invoke();
        }

        public void Done()
        {
            if (! done)
            {
                doneActions.Invoke();
                done = true;
            }
        }
    }

}
