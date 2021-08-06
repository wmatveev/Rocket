using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public virtual void OnPointerDown(PointerEventData eventData)
    { 
        OnDrag(eventData);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
       
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        LaserAttack.Instance.Aim(newPosition);
    }
}
