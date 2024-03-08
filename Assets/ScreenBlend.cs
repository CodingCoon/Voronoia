using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.U2D;

public class ScreenBlend : MonoBehaviour
{
    [Range(0.1f, 5f)][SerializeField] private float duration;
    [SerializeField] private Gradient colorGradient;
    [SerializeField] private List<SpriteShapeRenderer> cells;

    private bool initialOn; 
    private bool shown; 

    private void Start()
    {
        initialOn = GameManager.Instance.FadeOver;

        shown = initialOn;
        foreach (var item in cells)
        {
            item.enabled = initialOn;
        }

        AssignColor();
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
                cells[i].enabled = true;
            }
            yield return null;
        }
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
                cells[i].enabled = false;
            }
            yield return null;
        }
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Show"))
        {
            if (shown)
            {
                shown = false;
                StartCoroutine(Hide());
            }
            else
            {
                shown = true;
                StartCoroutine(Show(() => { }));
            }
        }
    }
}
