using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PreacherKnob : MonoBehaviour, IMouseListener
{   
    private IVoronation voronation;

    private bool hovered;

    [SerializeField] private Leader preacher;
    [SerializeField] private SpriteRenderer inner;
    [SerializeField] private SpriteRenderer number;
    [SerializeField] private new CircleCollider2D collider;
    [SerializeField] private GameObject preview;
    [SerializeField] private PreacherArea area;
    [SerializeField] private TrailRenderer trail;

    private PreviewType previewType = PreviewType.NONE;

    [SerializeField] private RingMenu ringMenu;
    private bool showsRing = false;

    public void Setup(IVoronation religion)
    {
        this.voronation = religion;
        inner.color = religion.Color;
        preview.SetActive(false);
        preview.transform.position = transform.position;
    }

    private void Awake()
    {
        ringMenu.transform.localScale = Vector3.zero;
        ActivateTrail(false);
    }

    private void Update()
    {
        if (voronation.IsAi) return;

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
            if (Game.INSTANCE.PhaseType != PhaseType.ACTION) return;
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
        inner.color = Color.Lerp(Color.clear, voronation.Color, progress);
    }

    public void OnHover(bool hovered)
    {
        if (voronation.IsAi) return;
        this.hovered = hovered;
        if (hovered)
        {
            LeaderSelectionManager.INSTANCE.UpdateLeader(preacher);
            inner.color = Color.clear;
            number.color = voronation.Color;
        }
        else
        {
            LeaderSelectionManager.INSTANCE.UpdateLeader(null);
            inner.color = voronation.Color;
            number.color = Color.black;
        }
    }

    public void ActivateTrail(bool trail)
    {
        this.trail.enabled = trail; 
    }

    public enum PreviewType
    {
        NONE, MOVE, SPLIT, SET
    }
}
