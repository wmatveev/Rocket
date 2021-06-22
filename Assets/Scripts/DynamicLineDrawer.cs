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
        //��������� ��� �� ����� ������� � ���� "�����������" �����
        Ray ray = new Ray(staticPoint.position, new Vector2(staticPoint.position.x, staticPoint.position.y) - clickPoint); //1 �������� - ������ ����, �.�, 2 ���-� - ����������� (B - A)
        //�������� ����� �������, ������� �� �����.����� ��������� �� ���������� offset
        Vector2 destination = ray.GetPoint(offset);
        Gizmos.color = Color.cyan;
        //Gizmos.DrawLine(clickPoint, resultPoint);
        Gizmos.DrawLine(clickPoint, destination);
        Gizmos.DrawSphere(staticPoint.position, 0.1f);
    }

}
