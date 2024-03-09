using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leader : MonoBehaviour, ILeader, IVoronoiCellOwner
{
    private static readonly int PRICE = -40;

    [SerializeField] private PreacherKnob knob;
    [SerializeField] private PreacherArea area;

    public IVoronation Voronation { get; private set; }
    public int Number { get; private set; }
    public int RoundsExist { get; private set; } = 0;
    public float Power { get; private set; } = 1f;          // increases range to boundaries
    public float Income { get; private set; } = 1f;      // increases income from area
    public IAction Action { get; private set; }

    private List<IncomePosition> positions = new List<IncomePosition>();


    public override string ToString()
    {
        return "Preacher (" + Voronation + ")";
    }

    public void Setup(int number, IVoronation voronation, Vector2 position)
    {
        this.Number = number;
        this.Voronation = voronation;
        knob.transform.position = new Vector3(position.x, position.y, -5);
        knob.Setup(Voronation);
        area.Setup(Voronation);
    }

    public void Reset()
    {
        positions.Clear();
        Action = NoAction.NO_ACTION;
    }

    public void SetAction(IAction action)
    {
        this.Action = action;
    }

    public bool HasAction()
    {
        return Action != null && (Action is not NoAction);
    }

    public IEnumerator ApplyAction()
    {
        positions.Add(new IncomePosition(Action.Name, Action.GetPrice()));  
        IAction tmpAction = this.Action;
        this.Action = NoAction.NO_ACTION;
        RoundsExist++;
        yield return tmpAction.Execute();
    }

    public void Evaluate()
    {
        positions.Add(new IncomePosition("Income", GetIncome()));
        positions.Add(new IncomePosition("Price", GetPrice()));
    }

    public int GetPrice()
    {
        return PRICE * RoundsExist;
    }

    public Vector2 GetPosition()
    {
        return knob.transform.position;
    }

    public List<IncomePosition> GetPositions()
    {
        return positions;
    }

    private float GetIncome()
    {
        return area.GetArea() * Income;
    }

    public float GetArea()
    {
        return area.GetArea();
    }

    public PreacherKnob Split(Vector2 position)
    {
        Leader preacher = Voronation.AddPreacher(position);
        float power = (Power - 1) / 2;
        float influence = (Income - 1) / 2;
        Power -= power;
        Income -= influence;
        preacher.Power = Power;
        preacher.Income = Income;
        return preacher.knob;
    }

    public void ImprovePower()
    {
        print("Improve Power: " + Power);
        Power += 0.1f;
    }

    public void ImproveInfluence()
    {
        print("Improve Influence: " + Income);
        Income += 0.1f;
    }

    public void HideKnob()
    {
        knob.HidePreview();
    }

    public void UpdateVoronoi(List<Vector3> positions)
    {
        area.SetBounds(positions.ToArray());
    }

    internal void Dissolve(float progress)
    {
        float scale = 1 - Math.Clamp(progress, 0f, 1f);
        transform.localScale = new Vector3(scale, scale);
        area.Dissolve(progress);
    }
}
