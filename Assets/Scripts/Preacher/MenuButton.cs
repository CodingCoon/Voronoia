using UnityEngine;

public class MenuButton : MonoBehaviour, IMouseListener
{
    private static Color DISABLED_COLOR = Color.black;
    private static Color HOVER_COLOR = Color.gray;
    private static Color DEFAULT_COLOR = Color.white;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private RingMenu menu;
    [SerializeField] private ActionType actionType;

    private bool interactable = true;
    private bool hovered = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && hovered && interactable)
        {
            menu.Execute(actionType);
        }
    }

    public void OnHover(bool hovered)
    {
        if (!interactable) return; 
        this.hovered = hovered;
        spriteRenderer.color = hovered ? HOVER_COLOR : DEFAULT_COLOR;
    }

    public ActionType GetActionType()
    {
        return actionType;
    }

    public void SetInteractable(bool interactable)
    {
        this.interactable = interactable;
        spriteRenderer.color = interactable ? DEFAULT_COLOR : DISABLED_COLOR;
    }

    public enum ActionType
    {
        IMPROVE_POWER,
        IMPROVE_INFLUENCE,
        MOVE,
        SPLIT
    }
}
