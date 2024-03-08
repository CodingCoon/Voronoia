using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game INSTANCE;

    private List<Religion> religions = new List<Religion>();
    private List<Round> rounds = new List<Round>();


    [SerializeField] private AbstractPhase initialPhase; 
    private AbstractPhase currentPhase; 
    public PhaseType PhaseType { get; private set; }
    
    
    private Round curRound = null;
    public Round CurRound => curRound;

    private void Awake()
    {
        INSTANCE = this;
    }

    private void Start()
    {
        NextRound();
    }

    public void NextRound()
    {
        print("New Round");
        Round round = new Round();
        curRound = round;
        rounds.Add(curRound);
        NextPhase();
    }

    public void NextPhase()
    {
        if (currentPhase != null)
        {
            currentPhase.OnEnd();
        }
        AbstractPhase next = currentPhase == null ? initialPhase : currentPhase.GetNextPhase();

        print("Next Phase " + next.GetPhaseType());
        currentPhase = next;
        PhaseType = currentPhase.GetPhaseType();
        currentPhase.OnStart();
    }

    private void Update()
    {
       
    }

    public void AddReligion(Religion religion)
    {
        //print("add " + religion.ReligionName);
        religions.Add(religion);
    }

    public List<Preacher> GetPreachers()
    {
        List<Preacher> preachers = new List<Preacher>();
        foreach (Religion religion in religions)
        {
            foreach (Preacher preacher in religion.GetPreachers())
            {
                preachers.Add(preacher);
            }
        }
        return preachers;
    }

    public List<Religion> GetReligions()
    {
        return religions;
    }
}
