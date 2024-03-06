using UnityEngine;

public class MouseEventManager : MonoBehaviour
{
    private GameObject _actualObject;
    private Vector2 point;
    private RaycastHit2D hit;

    void Update()
    {
        point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        hit = Physics2D.Raycast(point, Vector2.zero, 0f);

        if (hit.collider != null)
        {
            if (_actualObject == null)
            {
                _actualObject = hit.collider.gameObject;
                _actualObject.GetComponent<IMouseListener>()?.OnHover(true);
            }
            else if (_actualObject != hit.collider.gameObject)
            {
                _actualObject.GetComponent<IMouseListener>()?.OnHover(false);
                _actualObject = hit.collider.gameObject;
                _actualObject.GetComponent<IMouseListener>()?.OnHover(true);
            }
        }
        else if (_actualObject != null)
        {
            _actualObject.GetComponent<IMouseListener>()?.OnHover(false);
            _actualObject = null;
        }
    }
}