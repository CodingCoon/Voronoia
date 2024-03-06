using System;
using System.Collections;
using UnityEngine;

public class PreacherKnob : MonoBehaviour, IMouseListener
{   
    private IReligion religion;

    [SerializeField] private Preacher preacher;
    [SerializeField] private SpriteRenderer inner;
    [SerializeField] private new CircleCollider2D collider;
    [SerializeField] private GameObject preview;
    private bool dragged = false;
    private bool isSplit = false;

    [SerializeField] private Transform ringMenu;
    private bool showsRing = false;

    public void Setup(IReligion religion)
    {
        this.religion = religion;
        inner.color = religion.Color;
        preview.SetActive(false);
        preview.transform.position = transform.position;
    }

    private void Awake()
    {
        ringMenu.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (religion.IsAi) return;
        if (Input.GetMouseButtonDown(1))
        {
            if (showsRing)
            {
                showsRing = false;
                StartCoroutine(HideRing());
            }
            else
            {
                showsRing = true;
                StartCoroutine(ShowRing());
            }
        }
        //    else
        //    {
        //        isSplit = Input.GetKey(KeyCode.D);
        //        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //        if (collider.OverlapPoint(worldPosition))
        //        {
        //            dragged = true;
        //        }
        //    }
        //}

        //if (Input.GetMouseButtonUp(0))
        //{
        //    dragged = false;
        //    if (isSplit)
        //    {
        //        preacher.SetAction(new SplitAction(preacher, preview.transform.position));
        //    }
        //    else
        //    {
        //        preacher.SetAction(new MoveAction(this, preview.transform.position));
        //    }
        //}

        //if (dragged)
        //{
        //    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    preview.SetActive(true);
        //    preview.transform.position = new Vector3(worldPosition.x, worldPosition.y, 0);
        //}
    }

    private IEnumerator ShowRing()
    {
        float timeElapsed = 0f;
        float progress = 0f;
        Vector2 startScale = transform.localScale;
        Vector2 endScale = startScale - new Vector2(0.2f, 0.2f);

        while (progress < 1f)
        {
            timeElapsed += Time.deltaTime;
            progress = Mathf.Clamp(timeElapsed / 0.1f, 0f, 1f);
            transform.localScale = Vector2.Lerp(startScale, endScale, progress);
            yield return null;
        }

        progress = 0f;
        timeElapsed = 0f;
        Vector2 ringTargetScale = new Vector2(6f, 6f);

        while (progress < 1f)
        {
            timeElapsed += Time.deltaTime;
            progress = Mathf.Clamp(timeElapsed / 0.5f, 0f, 1f);
            transform.localScale = Vector2.Lerp(endScale, startScale, progress);
            ringMenu.localScale  = Vector2.Lerp(Vector2.zero, ringTargetScale, progress);
            ringMenu.eulerAngles = Vector3.Lerp(Vector3.zero, new Vector3(0, 0, 360f), progress);
            yield return null;
        }
    }

    private IEnumerator HideRing()
    {
        float timeElapsed = 0f;
        float progress = 0f;
        Vector2 startScale = ringMenu.localScale;

        while (progress < 1f)
        {
            timeElapsed += Time.deltaTime;
            progress = Mathf.Clamp(timeElapsed / 0.5f, 0f, 1f);
            ringMenu.localScale = Vector2.Lerp(startScale, Vector2.zero, progress);
            yield return null;
        }
    }

    public void Hide()
    {
        preview.SetActive(false);
    }

    internal void SetColor(float progress)
    {
        inner.color = Color.Lerp(Color.clear, religion.Color, progress);
    }

    public void OnHover(bool hovered)
    {
        if (hovered)
        {
            inner.color = Color.clear;
        }
        else
        {
            inner.color = religion.Color;
        }
    }
}
