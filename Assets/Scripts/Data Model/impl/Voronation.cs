using System;
using System.Collections.Generic;
using Unity.Hierarchy;
using UnityEngine;

public class Voronation : MonoBehaviour, IVoronation
{
    private int leaderCount = 1;

    [SerializeField] private Leader preacherPrefab;

    public string Name {  get; private set; }
    public Color Color { get; private set; }
    public float Money { get; private set; }
    public ITactic Tactic { get; private set; }
    public bool IsPlayer => Tactic == null;
    public bool IsAi => Tactic != null;


    private List<Leader> leaders = new List<Leader>();

    public override string ToString()
    {
        return Name;
    }

    public void Setup(string nationName, Color color, ITactic tactic, float money)
    {
        this.Name = nationName;
        this.Color = color;
        this.Tactic = tactic;
        this.Money = money;
    }

    public Leader AddPreacher(Vector2 position)
    {
        Leader preacher = GameObject.Instantiate(preacherPrefab, Vector2.zero, Quaternion.identity, transform);
        preacher.Setup(leaderCount++, this, position);
        leaders.Add(preacher);
        return preacher;
    }

    public IEnumerable<Leader> GetLeaders()
    {
        return leaders;
    }

    public void Reset()
    {
        leaders.ForEach(p => p.Reset());
    }

    public void UpdateIncome()
    {
        foreach (Leader leader in leaders)
        {
            foreach (IncomePosition income in leader.GetPositions())
            {
                Money += income.Value;
            }
        }
    }

    internal Leader ReleaseMostExpensiveLeader()
    {
        int highestPrice = 0;
        Leader mostExpensiveLeader = null;

        foreach (Leader p in leaders)
        {
            int curPrice = p.GetPrice();
            if (mostExpensiveLeader == null || curPrice > highestPrice)
            {
                highestPrice = curPrice;
                mostExpensiveLeader = p;
            }
        }

        leaders.Remove(mostExpensiveLeader);
        Money = highestPrice;
        return mostExpensiveLeader;
    }

    internal int GetLeaderCount()
    {
        return leaders.Count;
    }

    internal void GameOver()
    {
        Game.INSTANCE.RemoveReligion(this);
        GameObject.Destroy(this.gameObject, 0f);
    } 
}