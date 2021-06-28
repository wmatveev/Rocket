using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicLineDrawer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private Transform staticPoint;
    [SerializeField] private float offset = 2f;

    private float currentOffset = 0f;
    private Vector2 clickOffset = new Vector2(), 
                    clickPoint = new Vector2();

    void Start()
    {
        clickPoint = staticPoint.position;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        currentOffset = offset;
        clickPoint = staticPoint.position;
        clickPoint.y -= currentOffset;
        clickOffset = Camera.main.ScreenToWorldPoint(eventData.position);
        clickOffset = new Vector2(clickOffset.x - clickPoint.x, clickOffset.y - clickPoint.y);

        OnDrag(eventData);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        clickPoint = Camera.main.ScreenToWorldPoint(eventData.position);
        clickPoint = new Vector2(clickPoint.x - clickOffset.x, clickPoint.y - clickOffset.y);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        RocketLaunch();
        currentOffset = 0f;
        clickPoint = staticPoint.position;
    }

    private void RocketLaunch()
    {        
        Ray ray = new Ray(staticPoint.position, new Vector2(staticPoint.position.x, staticPoint.position.y) - clickPoint);
        Vector2 P1_pos = ray.GetPoint(currentOffset);
        GameObject P1 = new GameObject();
        P1.transform.position = P1_pos;

        GameObject rocket = GameManager.Instance.GetRocketFromPool();
        ThreeBezierScript bezier = rocket.GetComponent<ThreeBezierScript>();
        bezier.SetPointP1(P1.transform);
        rocket.transform.position = staticPoint.position;
        bezier.move = true;
        bezier.currentMode = ThreeBezierScript.Mode.rocketGuidance;
    }

    private void OnDrawGizmos()
    {
        Ray ray = new Ray(staticPoint.position, new Vector2(staticPoint.position.x, staticPoint.position.y) - clickPoint); //1 параметр - начало луча, т.ј, 2 пар-р - направление (B - A)
        Vector2 destination = ray.GetPoint(offset);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(clickPoint, destination);
        Gizmos.DrawSphere(staticPoint.position, 0.1f);
    }
}