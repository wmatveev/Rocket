using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenInfo : MonoBehaviour
{
    public static ScreenInfo Instance { get; private set; }
    #region ScreenSettings
    [SerializeField]
    private float distance;


    private void Awake()
    {
        Instance = this;
        SetScreenEdges();
    }

    private void OnDrawGizmos()
    {
        Vector3 p1 = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, distance));
        Vector3 p2 = Camera.main.ScreenToWorldPoint(new Vector3(0f, Camera.main.pixelHeight, distance));
        Vector3 p3 = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, distance));
        Vector3 p4 = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, 0f, distance));
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);
    }
    public Vector2 minScreenEdge, maxScreenEdge;
    public void SetScreenEdges()
    {
        Vector3 bottomLeft, topRight;
        bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, distance));
        topRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, distance));

        minScreenEdge = new Vector2(bottomLeft.x, bottomLeft.y);
        maxScreenEdge = new Vector2(topRight.x, topRight.y);

        GameManager.Instance.spawnPosition = new Vector3(minScreenEdge.x - 100, minScreenEdge.y - 100, 0);
    }

    public bool IsOnTheScreen(Vector3 position)
    {
        if (minScreenEdge.x < position.x && position.x < maxScreenEdge.x &&
            minScreenEdge.y < position.y && position.y < maxScreenEdge.y)
            return true;
        return false;
    }
    #endregion
}
