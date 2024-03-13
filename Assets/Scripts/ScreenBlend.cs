using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

public class ScreenBlend : MonoBehaviour
{
    public static ScreenBlend INSTANCE { get; private set; } 

    [Range(0.1f, 5f)][SerializeField] private float duration;
    [SerializeField] private Gradient colorGradient;
    [SerializeField] private List<SpriteShapeRenderer> cells;
    [SerializeField] private Material material;

    private bool initialOn; 
    private bool shown;

    private void Awake()
    {
        INSTANCE = this;
    }

    private void Start()
    {
        initialOn = GameManager.Instance.FadeOver;

        shown = initialOn;
        foreach (var item in cells)
        {
            item.gameObject.SetActive(initialOn);
        }

        AssignColor();
        // AssignBorder();
    }

    public void FadeOut(Action onEnd)
    {
        shown = true;
        StartCoroutine(Show(onEnd));
    }


    void Update()
    {
        if (initialOn)
        {
            StartCoroutine(Hide());
            initialOn = false;
        }
    }

    private void AssignColor()
    {
        float ratio = 0;
        for (int i = 0; i < cells.Count; i++)
        {
            ratio = (float) i / cells.Count;
            Color color = colorGradient.Evaluate(ratio);
            cells[i].color = color;
        }
    }

    private void AssignBorder()
    {
        foreach (var item in cells)
        {
            var ssc = item.GetComponent<SpriteShapeController>();
            LineRenderer lr = item.AddComponent<LineRenderer>();
            lr.startColor = Color.white;
            lr.endColor = Color.white;
            lr.loop = true;
            lr.sortingLayerName = "UI";
            lr.sortingOrder = 5;
            lr.useWorldSpace = false;
            lr.startWidth = 0.04f;
            lr.endWidth = 0.04f;
            lr.material = material;

            lr.positionCount = ssc.spline.GetPointCount();
            for (int i = 0; i < lr.positionCount; i++)
            {
                lr.SetPosition(i, ssc.spline.GetPosition(i));
            }
        }
    }

    private IEnumerator Show(Action onEnd)
    {
        float timeElapsed = 0;
        float progress = timeElapsed / duration;
        while (progress < 1) 
        {
            timeElapsed += Time.deltaTime;
            progress = timeElapsed / duration;
            if (progress > 1) progress = 1;

            // todo super inperformant aber ich will was sehen
            int show = (int) (progress * cells.Count);
            for (int i = 0; i < show; i++)
            {
                cells[i].gameObject.SetActive(true);
            }
            yield return null;
        }
        yield return new WaitForSeconds(2);
        onEnd.Invoke();
    }

    private IEnumerator Hide()
    {
        float timeElapsed = 0;
        float progress = timeElapsed / duration;
        while (progress < 1)
        {
            timeElapsed += Time.deltaTime;
            progress = timeElapsed / duration;
            if (progress > 1) progress = 1;

            // todo super inperformant aber ich will was sehen
            int show = (int)(progress * cells.Count);
            for (int i = 0; i < show; i++)
            {
                cells[i].gameObject.SetActive(false);
            }
            yield return null;
        }
    }
}
