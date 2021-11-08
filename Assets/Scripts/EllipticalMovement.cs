using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EllipticalMovement : MonoBehaviour
{
    [SerializeField] private float a = 1, b = 1, tiltAngle = 1, speed = 1, minScale = 0.5f, maxScale = 1f;
    [HideInInspector] public int lineSegments = 100;
    private float currAngle = 0f, currentScale, scaleSpeed = 0.006f;
    private Vector2 centerPos;
    private LineRenderer lineRenderer;

    private void Start()
    {
        Draw();
        currentScale = gameObject.transform.localScale.x;
    }

    public void Draw()
    {
        centerPos = gameObject.transform.position;

        //draw trajectory
        if (!lineRenderer)
            if (GetComponent<LineRenderer>())
                lineRenderer = GetComponent<LineRenderer>();
            else return;
        
        lineRenderer.positionCount = 1 + lineSegments;
        for (int i = 0; i < lineSegments + 1; i++)
        {
            Vector3 pointOnEllipse = Extentions.Geometry.getPointOnEllipse(a, b, currAngle, centerPos, tiltAngle);
            lineRenderer.SetPosition(i, pointOnEllipse);
            currAngle += (360f / lineSegments);
        }        
        currAngle = Random.Range(10, 350);
    }

    public void SetProperties(float a, float b, float tiltAngle, float speed)
    {
        this.a = a;
        this.b = b;
        this.tiltAngle = tiltAngle;
        this.speed = speed;
    }

    private void OnDisable()
    {
        
    }

    void FixedUpdate()
    {
        currAngle += Time.deltaTime * speed;
        gameObject.transform.position = Extentions.Geometry.getPointOnEllipse(a, b, currAngle, centerPos,tiltAngle);
        if ((currAngle % 360) < 180)
            currentScale += Time.deltaTime * (speed * scaleSpeed);
        else currentScale -= Time.deltaTime * (speed * scaleSpeed);

        currentScale = Mathf.Clamp(currentScale, minScale, maxScale);
        gameObject.transform.localScale = new Vector2(currentScale, currentScale);
    }

    public void SetAngle(float angle)
    {
        currAngle = angle;
    }
}
