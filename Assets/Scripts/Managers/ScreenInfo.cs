using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenInfo : MonoBehaviour
{
    public static ScreenInfo Instance { get; private set; }
    #region ScreenSettings
    private float distance;


    private void Awake()
    {
        Instance = this;
        SetScreenEdges();
        distance = Camera.main.orthographicSize;
    }

    private void OnDrawGizmos()
    {
        Vector3 p1 = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, distance));
        Vector3 p2 = Camera.main.ScreenToWorldPoint(new Vector3(0f, Camera.main.pixelHeight, distance));
        Vector3 p3 = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, distance));
        Vector3 p4 = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, 0f, distance));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);
    }
    public Vector2 minScreenEdge, maxScreenEdge;
    public Vector2 centerPoint;
    public void SetScreenEdges()
    {
        Vector3 bottomLeft, topRight;
        bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, distance));
        topRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, distance));

        minScreenEdge = new Vector2(bottomLeft.x, bottomLeft.y);
        maxScreenEdge = new Vector2(topRight.x, topRight.y);
        centerPoint = new Vector2(bottomLeft.x + (topRight.x - bottomLeft.x)/2, bottomLeft.y + (topRight.y - bottomLeft.y)/2);

        if (GameManager.Instance) 
            GameManager.Instance.spawnPosition = new Vector3(minScreenEdge.x - 100, minScreenEdge.y - 100, 0);
    }
    static public Vector2 GetCurrMinScreenEdge()
    {
        float distance = Camera.main.orthographicSize;
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, distance));
        Vector2 minScreenEdge = new Vector2(bottomLeft.x, bottomLeft.y);
        return minScreenEdge;
    }
   
    static public Vector2 GetCurrMaxScreenEdge()
    {
        float distance = Camera.main.orthographicSize;
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, distance));
        Vector2 maxScreenEdge = new Vector2(topRight.x, topRight.y);
        return maxScreenEdge;
    }

    static public Vector2 GetCurrMinScreenEdgeWithOffset(float offset)
    {
        float distance = Camera.main.orthographicSize + offset;
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, distance));
        Vector2 minScreenEdge = new Vector2(bottomLeft.x, bottomLeft.y);
        return minScreenEdge;
    }

    static public Vector2 GetCurrMaxScreenEdgeWithOffset(float offset)
    {
        float distance = Camera.main.orthographicSize + offset;
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, distance));
        Vector2 maxScreenEdge = new Vector2(topRight.x, topRight.y);
        return maxScreenEdge;
    }

    public bool IsOnTheScreen(Vector3 position)
    {
        if (minScreenEdge.x < position.x && position.x < maxScreenEdge.x &&
            minScreenEdge.y < position.y && position.y < maxScreenEdge.y)
            return true;
        return false;
    }

    public bool IsOnTheScreen(Vector3 position, float orthographicSize)
    {
        Vector2 minScreenEdge = GetCurrMinScreenEdgeWithOffset(orthographicSize);
        Vector2 maxScreenEdge = GetCurrMaxScreenEdgeWithOffset(orthographicSize);
        if (minScreenEdge.x < position.x && position.x < maxScreenEdge.x &&
            minScreenEdge.y < position.y && position.y < maxScreenEdge.y)
            return true;
        return false;
    }
    #endregion
}
