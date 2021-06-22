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
    private float A = 0f, B = 0f, C = 0f; //коэффициенты линейного уравнения

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
        //Получаем координаты клика/тапа
        //Debug.Log(eventData.position);
        currentOffset = offset;
        clickPoint = Camera.main.ScreenToWorldPoint(eventData.position);
        //Debug.Log(clickPoint);

        calculateLinearFuncCoefs(staticPoint.position, clickPoint);

    }
    public void OnPointerUp(PointerEventData eventData)
    {
        currentOffset = 0f;
        clickPoint = staticPoint.position;
    }

    private void calculateLinearFuncCoefs(Vector2 point1, Vector2 point2)
    {
        float x1 = point1.x;
        float x2 = point2.x;

        float y1 = point1.y;
        float y2 = point2.y;

        A = y1 - y2;
        B = x2 - x1;
        C = x1 * y2 - x2 * y1;
    }

    private float getLinearFuncY(float x)
    {
        if (B == 0) return 0;
        return (-A * x - C) / B;
    }

    private float getLinearFuncX(float y)
    {
        if (A == 0) return 0;
        return (-B * y - C) / A;
    }

    private void OnDrawGizmos()
    {
        
        float Cy = staticPoint.position.y + currentOffset * direction(clickPoint, staticPoint);
        //координаты новой точки = координаты центральной + смещение с учетом направления
        float Cx = getLinearFuncX(Cy);
        if (Cy == staticPoint.position.y) //есть проблема с начальной отрисовкой
            Cx = staticPoint.position.x;
        //Здесь получается ограничительный квадрат, проверяем, выходит ли за него точка
            if (Cx >= (staticPoint.position.x + offset) //случай когда точка правее offset
            || Cx <= (staticPoint.position.x - offset)) //случай когда точка левее -offset
        {
            // здесь мы проверили, входим ли мы в отрезок [x - offset, x + offset]
            // рассмотрим случай "непринадлежности"
            if (Cx <= (staticPoint.position.x - offset))
            //В случае выхода за левую границу берем staticPoint.position.x - offset
            {
                Cx = staticPoint.position.x - offset;
                Cy = getLinearFuncY(Cx);
            }
            else
            {
                Cx = staticPoint.position.x + offset;
                Cy = getLinearFuncY(Cx);
            }
        }

        Vector3 resultPoint = new Vector3(Cx, Cy, 0f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(staticPoint.position, 0.1f);
        Gizmos.DrawLine(clickPoint, resultPoint);

        /* правильная логика, заменяющая весь код выше и ниже
        //Выпускаем луч из точки касания в нашу "центральную" точку
        Ray ray = new Ray(staticPoint.position, new Vector2(staticPoint.position.x, staticPoint.position.y) - clickPoint); //1 параметр - начало луча, т.А, 2 пар-р - направление (B - A)
        //Получаем конец отрезка, который от центр.точки находится на расстоянии offset
        Vector2 destination = ray.GetPoint(offset);
        Gizmos.color = Color.cyan;
        //Gizmos.DrawLine(clickPoint, resultPoint);
        Gizmos.DrawLine(clickPoint, destination);
        Gizmos.DrawSphere(staticPoint.position, 0.1f);
        */
    }

    private int direction(Vector2 point, Transform parentPoint)
    {
        Vector2 relativeCoordinate = parentPoint.InverseTransformPoint(point); //получаем относительные координаты
        if (relativeCoordinate.y <= 0) 
            return 1; //если тапаем снизу, то строим отрезок вверх
        else return -1; //и соответственно в противоположном случае направление идет вниз
    }


}
