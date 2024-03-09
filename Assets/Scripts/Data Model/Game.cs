using System;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game INSTANCE;

    private List<Voronation> religions = new List<Voronation>();
    private List<Round> rounds = new List<Round>();


    [SerializeField] private AbstractPhase initialPhase; 
    private AbstractPhase currentPhase; 
    public PhaseType PhaseType { get; private set; }
    
    public GameState State { get; private set; }

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

    public void AddReligion(Voronation religion)
    {
        //print("add " + religion.ReligionName);
        religions.Add(religion);
    }

    internal void RemoveReligion(Voronation diedReligion)
    {
        print("Religion GameOver " + diedReligion);

        religions.Add(diedReligion);
        UpdateGameState();
    }

    private void UpdateGameState()
    {
        bool playerActive = false;
        bool aiActive = false;

        foreach (Voronation religion in religions)
        {
            aiActive = aiActive || religion.IsAi;
            playerActive = playerActive || religion.IsPlayer;
        }

        if (playerActive)
        {
            State = aiActive ? GameState.RUNNING : GameState.PLAYER_WON;
        }
        else
        {
            State = aiActive ? GameState.PLAYER_LOOSE : GameState.PLAYER_WON;
        }
    }

    public List<Leader> GetPreachers()
    {
        List<Leader> preachers = new List<Leader>();
        foreach (Voronation religion in religions)
        {
            foreach (Leader preacher in religion.GetLeaders())
            {
                preachers.Add(preacher);
            }
        }
        return preachers;
    }

    public List<Voronation> GetVoronations()
    {
        return religions;
    }

    internal bool IsOver()
    {
        return State != GameState.RUNNING;
    }

    internal Voronation GetHumanPlayer()
    {
        return religions[0]; // todo sollte hinhauen, kann man aber sicher auch lazy cachen
    }

    public enum GameState
    {
        RUNNING,
        PLAYER_WON,
        PLAYER_LOOSE,
        DRAW
    }
}
