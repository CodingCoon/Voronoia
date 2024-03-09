using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PreacherKnob : MonoBehaviour, IMouseListener
{   
    private IVoronation religion;

    private bool hovered;

    [SerializeField] private Leader preacher;
    [SerializeField] private SpriteRenderer inner;
    [SerializeField] private new CircleCollider2D collider;
    [SerializeField] private GameObject preview;
    [SerializeField] private PreacherArea area;

    private PreviewType previewType = PreviewType.NONE;

    [SerializeField] private RingMenu ringMenu;
    private bool showsRing = false;

    public void Setup(IVoronation religion)
    {
        this.religion = religion;
        inner.color = religion.Color;
        preview.SetActive(false);
        preview.transform.position = transform.position;
    }

    private void Awake()
    {
        ringMenu.transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (religion.IsAi) return;

        if (Input.GetMouseButtonDown(0) && showsRing)
        {
            showsRing = false;
            StartCoroutine(HideRing());
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (previewType == PreviewType.MOVE)
            {
                preacher.SetAction(new MoveAction(this, preview.transform.position));
                previewType = PreviewType.SET;
            }
            else if (previewType == PreviewType.SPLIT)
            {
                preacher.SetAction(new SplitAction(preacher, preview.transform.position));
                previewType = PreviewType.SET;

            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (showsRing)
            {
                showsRing = false;
                StartCoroutine(HideRing());
            }
            else if (hovered)
            {
                showsRing = true;
                StartCoroutine(ShowRing());
            }
            
            if (previewType == PreviewType.SET) return;
            previewType = PreviewType.NONE;
            HidePreview();
        }
        
        if (previewType == PreviewType.SPLIT || previewType == PreviewType.MOVE)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            preview.SetActive(true);

            Vector2 closestPoint = area.ClosestPoint(worldPosition);
            PlannedActionController.INSTANCE.UpdatePosition(closestPoint);
            preview.transform.position = new Vector3(closestPoint.x, closestPoint.y, 0);
        }
    }


    internal void StartDrag(PreviewType type)
    {
        previewType = type;
    }

    private IEnumerator ShowRing()
    {
        ringMenu.OnShow();
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
            ringMenu.transform.localScale  = Vector2.Lerp(Vector2.zero, ringTargetScale, progress);
            ringMenu.transform.eulerAngles = Vector3.Lerp(Vector3.zero, new Vector3(0, 0, 360f), progress);
            yield return null;
        }
    }

    private IEnumerator HideRing()
    {
        float timeElapsed = 0f;
        float progress = 0f;
        Vector2 startScale = ringMenu.transform.localScale;

        while (progress < 1f)
        {
            timeElapsed += Time.deltaTime;
            progress = Mathf.Clamp(timeElapsed / 0.5f, 0f, 1f);
            ringMenu.transform.localScale = Vector2.Lerp(startScale, Vector2.zero, progress);
            yield return null;
        }
    }

    public void HidePreview()
    {
        preview.SetActive(false);
    }

    internal void SetColor(float progress)
    {
        inner.color = Color.Lerp(Color.clear, religion.Color, progress);
    }

    public void OnHover(bool hovered)
    {
        this.hovered = hovered;
        if (hovered)
        {
            LeaderSelectionManager.INSTANCE.UpdateLeader(preacher);
            inner.color = Color.clear;
        }
        else
        {
            LeaderSelectionManager.INSTANCE.UpdateLeader(null);
            inner.color = religion.Color;
        }
    }

    public enum PreviewType
    {
        NONE, MOVE, SPLIT, SET
    }
}
