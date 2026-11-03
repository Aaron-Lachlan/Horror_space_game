using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CursorDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] CursorManager.CursorType cursorType;
    void OnMouseEnter()
    {
        CursorManager.Instace.SetActiveCursorType(cursorType);
    }

    void OnMouseExit()
    {
        CursorManager.Instace.SetActiveCursorType(CursorManager.CursorType.Arrow);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //print("Mouse Over Button");
        CursorManager.Instace.SetActiveCursorType(cursorType);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        CursorManager.Instace.SetActiveCursorType(CursorManager.CursorType.Arrow);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        CursorManager.Instace.SetActiveCursorType(CursorManager.CursorType.ButtonClicked);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorManager.Instace.SetActiveCursorType(CursorManager.CursorType.Arrow);
    }
}
