using DG.Tweening;
using TMPro;
using UnityEngine;
using VoronationCore;

public class PreacherKnob : MonoBehaviour, IMouseListener
{
    private IVoronation voronation;
    private bool hovered;
    [SerializeField] private Leader preacher;
    [SerializeField] private SpriteRenderer inner;
    [SerializeField] private TextMeshPro numberLabel;
    [SerializeField] private new CircleCollider2D collider;
    [SerializeField] private GameObject preview;
    [SerializeField] private PreacherArea area;
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private RingMenu ringMenu;
    private PreviewType previewType;
    private bool showsRing;
    private Tween ringTween;
    private Tween actionTween;
    private Vector3 restScale;
    private bool targetValid;

    public void Setup(IVoronation religion)
    {
        voronation = religion;
        inner.color = religion.Color;
        preview.SetActive(false);
        preview.transform.position = transform.position;
    }

    private void Awake()
    {
        restScale = transform.localScale;
        ringMenu.transform.localScale = Vector3.zero;
        ActivateTrail(false);
    }

    private void Update()
    {
        if (voronation == null || voronation.IsAi || Game.INSTANCE == null || !Game.INSTANCE.CanPlan) return;
        if (previewType == PreviewType.MOVE || previewType == PreviewType.SPLIT)
        {
            if (!area.HasArea) { CancelInteraction(); return; }
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 closest;
            try
            {
                closest = area.ClosestPointInside(worldPosition, preacher.GetPosition(),
                    RoundResolver.TargetMargin + PolygonGeometry.Epsilon * 2);
            }
            catch (System.InvalidOperationException)
            {
                targetValid = false;
                preview.SetActive(false);
                return;
            }
            targetValid = previewType != PreviewType.SPLIT ||
                Vector2.Distance(closest, preacher.GetPosition()) >= RoundResolver.TargetMargin;
            preview.SetActive(targetValid);
            preview.transform.position = new Vector3(closest.x, closest.y, 0);
            if (targetValid) PlannedActionController.INSTANCE.UpdatePosition(closest);
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (showsRing) HideRing();
            if (previewType == PreviewType.MOVE && targetValid)
            {
                preacher.SetAction(new MoveAction(this, preview.transform.position));
                previewType = PreviewType.SET;
                PlannedActionController.INSTANCE.UnPlan();
            }
            else if (previewType == PreviewType.SPLIT && targetValid)
            {
                preacher.SetAction(new SplitAction(preacher, preview.transform.position));
                previewType = PreviewType.SET;
                PlannedActionController.INSTANCE.UnPlan();
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (showsRing) HideRing();
            else if (hovered) ShowRing();
            if (previewType == PreviewType.SET) return;
            previewType = PreviewType.NONE;
            HidePreview();
            PlannedActionController.INSTANCE.UnPlan();
        }
    }

    internal void StartDrag(PreviewType type)
    {
        if (!Game.INSTANCE.CanPlan || !area.HasArea) return;
        previewType = type;
        targetValid = false;
    }

    private void ShowRing()
    {
        ringTween?.Kill();
        showsRing = true;
        ringMenu.OnShow();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(restScale - new Vector3(0.2f, 0.2f, 0), 0.1f));
        sequence.Append(transform.DOScale(restScale, 0.5f));
        sequence.Join(ringMenu.transform.DOScale(new Vector3(6, 6, 1), 0.5f));
        sequence.Join(ringMenu.transform.DOLocalRotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360));
        ringTween = sequence.SetLink(gameObject);
    }

    private void HideRing()
    {
        ringTween?.Kill();
        showsRing = false;
        ringTween = DOTween.Sequence()
            .Append(ringMenu.transform.DOScale(Vector3.zero, 0.2f))
            .Join(transform.DOScale(restScale, 0.2f))
            .SetLink(gameObject);
    }

    public Tween AnimateMove(Vector2 target, float duration)
    {
        actionTween?.Kill();
        ActivateTrail(true);
        actionTween = transform.DOMove(new Vector3(target.x, target.y, transform.position.z), duration)
            .SetEase(Ease.Linear).SetLink(gameObject)
            .OnKill(() => { ActivateTrail(false); HidePreview(); });
        return actionTween;
    }

    public Tween AnimateAppearance(float duration)
    {
        actionTween?.Kill();
        SetColor(0);
        actionTween = DOVirtual.Float(0, 1, duration, SetColor).SetEase(Ease.Linear).SetLink(gameObject);
        return actionTween;
    }

    public void CancelInteraction()
    {
        ringTween?.Kill();
        showsRing = hovered = false;
        previewType = PreviewType.NONE;
        targetValid = false;
        ringMenu.transform.localScale = Vector3.zero;
        ringMenu.transform.localRotation = Quaternion.identity;
        transform.localScale = restScale;
        HidePreview();
        if (voronation != null) inner.color = voronation.Color;
        numberLabel.color = Color.black;
    }

    public void CancelAnimations()
    {
        actionTween?.Kill();
        CancelInteraction();
        ActivateTrail(false);
    }

    private void OnDisable() { CancelAnimations(); }
    public void HidePreview() { preview.SetActive(false); }
    internal void SetColor(float progress) { inner.color = Color.Lerp(Color.clear, voronation.Color, progress); }

    public void OnHover(bool value)
    {
        if (voronation == null || voronation.IsAi) return;
        hovered = value && Game.INSTANCE != null && Game.INSTANCE.CanPlan;
        if (hovered)
        {
            LeaderSelectionManager.INSTANCE.UpdateLeader(preacher);
            inner.color = Color.clear;
            numberLabel.color = voronation.Color;
        }
        else
        {
            if (LeaderSelectionManager.INSTANCE != null && LeaderSelectionManager.INSTANCE.Leader == preacher)
                LeaderSelectionManager.INSTANCE.UpdateLeader(null);
            inner.color = voronation.Color;
            numberLabel.color = Color.black;
        }
    }

    public void ActivateTrail(bool active) { trail.enabled = active; }
    public enum PreviewType { NONE, MOVE, SPLIT, SET }
}
