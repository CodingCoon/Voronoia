using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Leader : MonoBehaviour, ILeader, IVoronoiCellOwner
{
    private static readonly Color NEGATIV = new Color(0.7882354f, 0.2745098f, 0.3960785f);
    private static readonly Color POSITIVE = new Color(0.2352941f, 0.5607843f, 0.482353f);

    private static readonly int PRICE = -40;

    [SerializeField] private PreacherKnob knob;
    [SerializeField] private PreacherArea area;
    [SerializeField] private Animator animator;
    [SerializeField] private Prototype<VFX> vfxPrototype;
    [SerializeField] private SpriteRenderer numberRenderer;
    [SerializeField] private TextMeshPro income;

    public NumberSprites numberSprites; 

    public IVoronation Voronation { get; private set; }
    public int Number { get; private set; }
    public int RoundsExist { get; private set; } = 0;
    public float Power { get; private set; } = 1f;          // increases range to boundaries
    public float Income { get; private set; } = 1f;      // increases income from area
    public IAction Action { get; private set; } = NoAction.NO_ACTION;

    private List<IncomePosition> positions = new List<IncomePosition>();


    public override string ToString()
    {
        return "Preacher (" + Voronation + ")";
    }

    public void Setup(int number, IVoronation voronation, Vector2 position)
    {
        name = "Leader #" + number; 
        this.Number = number;
        this.Voronation = voronation;
        knob.transform.position = new Vector3(position.x, position.y, -5);
        knob.Setup(Voronation);
        area.Setup(Voronation);
        UpdateAnimator();
        UpdateNumberRenderer();
        income.gameObject.SetActive(false);

    }

    public void Reset()
    {
        positions.Clear();
        Action = NoAction.NO_ACTION;
        UpdateAnimator();
        income.gameObject.SetActive(false);
    }

    public void SetAction(IAction action)
    {
        this.Action = action;
        UpdateAnimator();
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
        positions.Add(new IncomePosition("Leader", GetPrice()));
        positions.Add(new IncomePosition("Action", Action.GetPrice()));

        float income = GetIncome() + GetPrice() + Action.GetPrice();
        StartCoroutine(ShowIncome((int) income));
    }

    private IEnumerator ShowIncome(int incomeValue)
    {
        this.income.gameObject.SetActive(true);

        this.income.color = incomeValue < 0 ? NEGATIV : POSITIVE;

        float timeElapsed = 0;
        float duration = 1.2f;
        float progress = 0;
        Vector2 targetPos = new Vector2(0, 2f); 
        float fromFontSize = 10;
        float toFontSize = 18;


        while (progress < 1)
        {
            timeElapsed += Time.deltaTime;
            progress = timeElapsed / duration;
            progress = Math.Clamp(progress, 0, 1);

            this.income.text = "" + (int) Mathf.Lerp(0, incomeValue, progress);
            this.income.fontSize = Mathf.Lerp(fromFontSize, toFontSize, progress);
            this.income.rectTransform.anchoredPosition = Vector2.Lerp(Vector2.zero, targetPos, progress);
            yield return null;
        }

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
        Power += 0.1f;
    }

    public void ImproveInfluence()
    {
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

    // The pulsing animator is shown, when its the leader belongs to the human player, the action is null or it is the NO_ACTION
    private void UpdateAnimator()
    {
        if (Voronation.IsAi)
        {
            animator.gameObject.SetActive(false);
        }
        else
        {
            animator.gameObject.SetActive(Action == null || Action == NoAction.NO_ACTION);
        }
    }

    private void UpdateNumberRenderer()
    {
        if (Voronation.IsAi)
        {
            numberRenderer.sprite = null;
        }
        else
        {
            numberRenderer.sprite = numberSprites.Get(Number);
        }
    }

    public IEnumerator ShowVFX(string name)
    {
        VFX vfx = vfxPrototype.Create(transform, knob.transform.position);
        vfx.Play(name);
        yield return new WaitForSeconds(1f);
    }


    [Serializable]
    public class NumberSprites
    {
        [SerializeField] internal Sprite one;
        [SerializeField] internal Sprite two;
        [SerializeField] internal Sprite three;
        [SerializeField] internal Sprite four;
        [SerializeField] internal Sprite five;

        public Sprite Get(int number)
        {
            switch (number % 5)
            {
                case 1: return one;
                case 2: return two;
                case 3: return three;
                case 4: return four;
                case 5: return five;
            }
            throw new System.Exception("snh: " + number);
        }
    }
}
