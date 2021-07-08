using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AutoGun : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    Vector3 newPosition = new Vector3();
    void Start()
    {


    }
    void Update()
    {
    }

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    Debug.Log("OnPointerDown");
    //    newPosition = Camera.main.ScreenToWorldPoint(eventData.position);
    //}

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        newPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        gameObject.transform.position = newPosition;
    }
    public void OnBeginDrag(PointerEventData eventData)

    {
            Debug.Log("OnBeginDrag");

    }
    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    Debug.Log("OnPointerUp");

    //}
}
