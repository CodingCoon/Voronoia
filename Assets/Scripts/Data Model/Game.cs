using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game INSTANCE;

    [SerializeField] VoronoiController voronoi; // todo das hätte ich hier gerne raus, das muss das Game nicht wissen

    private List<Religion> religions = new List<Religion>();
    private List<Round> rounds = new List<Round>();


    [SerializeField] private ActionPhase initialPhase; // todo offen gestalten
    private IPhase currentPhase; 
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

    private void NextPhase()
    {
        if (currentPhase != null)
        {
            currentPhase.OnEnd();
        }
        IPhase next = currentPhase == null ? initialPhase : currentPhase.GetNextPhase();

        print("Next Phase " + next.GetPhaseType());
        currentPhase = next;
        PhaseType = currentPhase.GetPhaseType();
        currentPhase.OnStart();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            NextPhase();
        }
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
