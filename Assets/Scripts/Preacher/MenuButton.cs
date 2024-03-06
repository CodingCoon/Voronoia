using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private static Color HOVER_COLOR = Color.gray;
    private static Color DEFAULT_COLOR = Color.white;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private RingMenu menu;
    [SerializeField] private ActionType actionType;

    private void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    menu.Execute(actionType);
        //}
    }
 
    public void OnPointerEnter(PointerEventData eventData)
    {
        spriteRenderer.color = HOVER_COLOR;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        spriteRenderer.color = DEFAULT_COLOR;
    }

    public enum ActionType
    {
        IMPROVE_POWER,
        IMPROVE_INFLUENCE,
        MOVE,
        SPLIT
    }
}
