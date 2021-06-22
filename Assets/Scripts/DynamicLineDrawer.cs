using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicLineDrawer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{

    [SerializeField] private Transform staticPoint;
    [SerializeField] private float offset = 1f;
    private float currentOffset = 0f;
    private Vector2 clickPoint = new Vector2(0, 0);

    void Start()
    {
        clickPoint = staticPoint.position;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        currentOffset = offset;
        clickPoint = Camera.main.ScreenToWorldPoint(eventData.position);

    }
    public void OnPointerUp(PointerEventData eventData)
    {
        currentOffset = 0f;
        clickPoint = staticPoint.position;
    }

    private void OnDrawGizmos()
    {
        //Выпускаем луч из точки касания в нашу "центральную" точку
        Ray ray = new Ray(staticPoint.position, new Vector2(staticPoint.position.x, staticPoint.position.y) - clickPoint); //1 параметр - начало луча, т.А, 2 пар-р - направление (B - A)
        //Получаем конец отрезка, который от центр.точки находится на расстоянии offset
        Vector2 destination = ray.GetPoint(offset);
        Gizmos.color = Color.cyan;
        //Gizmos.DrawLine(clickPoint, resultPoint);
        Gizmos.DrawLine(clickPoint, destination);
        Gizmos.DrawSphere(staticPoint.position, 0.1f);
    }

}
