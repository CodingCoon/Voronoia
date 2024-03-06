﻿using System.Collections.Generic;
using UnityEngine;

public class Religion : MonoBehaviour, IReligion
{
    [SerializeField] private Preacher preacherPrefab;

    public string Name {  get; private set; }
    public Color Color { get; private set; }
    public float Faith { get; private set; } = 100;
    public ITactic Tactic { get; private set; }
    public bool IsPlayer => Tactic == null;
    public bool IsAi => Tactic != null;


    private List<Preacher> preachers = new List<Preacher>();

    public override string ToString()
    {
        return Name;
    }

    public void Setup(string religionName, Color color, ITactic tactic)
    {
        this.Name = religionName;
        this.Color = color;
        this.Tactic = tactic;
    }

    public Preacher AddPreacher(Vector2 position)
    {
        Preacher preacher = GameObject.Instantiate(preacherPrefab, Vector2.zero, Quaternion.identity, transform);
        preacher.Setup(this, position);
        preachers.Add(preacher);
        return preacher;
    }

    public IEnumerable<Preacher> GetPreachers()
    {
        return preachers;
    }

    public void Reset()
    {
        preachers.ForEach(p => p.Reset());
    }

    public void UpdateIncome()
    {
        print("\tIncome " + this);
        foreach (Preacher preacher in preachers)
        {
            print("\t\t Preacher");
            foreach (IncomePosition income in preacher.GetPositions())
            {
                print("\t\t\t" + income);
                Faith += income.Value;
            }
        }
        print("\t--------------- " + Faith);
    }
}