using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preacher : MonoBehaviour, IPreacher, IVoronoiCellOwner
{
    private static readonly float PRICE = -40;

    [SerializeField] private PreacherKnob knob;
    [SerializeField] private PreacherArea area;

    public float Power { get; private set; } = 1f;          // increases range to boundaries
    public float Influence { get; private set; } = 1f;      // increases income from area

    public IReligion Religion { get; private set; }

    private IAction action;
    private List<IncomePosition> positions = new List<IncomePosition>();


    public override string ToString()
    {
        return "Preacher (" + Religion + ")";
    }

    public void Setup(IReligion religion, Vector2 position)
    {
        this.Religion = religion;
        knob.transform.position = position;
        knob.Setup(Religion);
        area.Setup(Religion);
    }

    public void Reset()
    {
        positions.Clear();
        action = null;
    }

    public void SetAction(IAction action)
    {
        this.action = action;
    }

    public bool HasAction()
    {
        return action != null && (action is not NoAction);
    }

    public IEnumerator ApplyAction()
    {
        positions.Add(new IncomePosition(action.Name, action.GetPrice()));
        IAction tmpAction = this.action;
        this.action = NoAction.NO_ACTION;
        yield return tmpAction.Execute();
    }

    public void Evaluate()
    {
        positions.Add(new IncomePosition("Income", GetIncome()));
        positions.Add(new IncomePosition("Price", PRICE));
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
        return area.GetArea() * Influence;
    }

    public PreacherKnob Split(Vector2 position)
    {
        print("Split to " + position);
        Preacher preacher = Religion.AddPreacher(position);
        float power = (Power - 1) / 2;
        float influence = (Influence - 1) / 2;
        Power -= power; ;
        Influence -= influence;
        preacher.Power = Power;
        preacher.Influence = Influence;
        return preacher.knob;
    }

    public void ImprovePower()
    {
        print("Improve Power: " + Power);
        Power += 0.1f;
    }

    public void ImproveInfluence()
    {
        print("Improve Influence: " + Influence);
        Influence += 0.1f;
    }

    public void HideKnob()
    {
        knob.Hide();
    }

    public void UpdateVoronoi(List<Vector3> positions)
    {
        area.SetBounds(positions.ToArray());
    }
}
