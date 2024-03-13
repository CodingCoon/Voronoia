using System;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game INSTANCE;

    private List<Voronation> voronations = new List<Voronation>();

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
        NextPhase();
    }

    public void NextPhase()
    {
        if (currentPhase != null)
        {
            if (currentPhase.GetPhaseType() == PhaseType.DEATH)
            {
                NewRound();        
            }
            currentPhase.OnEnd();
        }

        AbstractPhase next = currentPhase == null ? initialPhase : currentPhase.GetNextPhase();

        currentPhase = next;
        PhaseType = currentPhase.GetPhaseType();
        currentPhase.OnStart();
    }

    private void NewRound()
    {
        voronations.ForEach(v => v.Reset());
    }

    public void AddReligion(Voronation religion)
    {
        //print("add " + religion.ReligionName);
        voronations.Add(religion);
    }

    internal void RemoveReligion(Voronation diedReligion)
    {
        print("Religion GameOver " + diedReligion);
        voronations.Add(diedReligion);
        UpdateGameState();
    }

    private void UpdateGameState()
    {
        bool playerActive = false;
        bool aiActive = false;

        foreach (Voronation religion in voronations)
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
        foreach (Voronation religion in voronations)
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
        return voronations;
    }

    internal bool IsOver()
    {
        return State != GameState.RUNNING;
    }

    internal Voronation GetHumanPlayer()
    {
        return voronations[0]; // todo sollte hinhauen, kann man aber sicher auch lazy cachen
    }

    public enum GameState
    {
        RUNNING,
        PLAYER_WON,
        PLAYER_LOOSE,
        DRAW
    }
}
