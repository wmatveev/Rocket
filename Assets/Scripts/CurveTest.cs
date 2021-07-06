#define Debug
using System.Collections.Generic;
using UnityEngine;

public class CurveTest : MonoBehaviour
{
    public Transform tP0;
    public Transform tP1;
    public Transform tP2;

    public float speed = 10f;

    public int subdivs = 20;

    private float[] _speedsByChordsLengths;
    private float _totalLength;

    private Vector3 _p0;
    private Vector3 _p1;
    private Vector3 _p2;

    private float _t;

#if Debug
    private Vector3 _prevPos;
    private List<Vector3> _testPositions = new List<Vector3>();
#endif

    void Start()
    {
        _p0 = tP0.position;
        _p1 = tP1.position;
        _p2 = tP2.position;

        PrepareCoords();
    }

    void Update()
    {
        _t += Time.deltaTime * speed / _totalLength / GetSpeedByCoordLength(_t);
        _t = Mathf.Clamp01(_t);

        Vector3 b = GetQuadBezierPoint(_p0, _p1, _p2, _t);

        transform.position = b;

#if Debug
        DrawPoints();
#endif
    }

    Vector3 GetQuadBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float invT = 1 - t;
        return (invT * invT * p0) + (2 * invT * t * p1) + (t * t * p2);
    }

    private void PrepareCoords()
    {
        _speedsByChordsLengths = new float[subdivs];

        Vector3 prevPos = _p0;
        for (int i = 0; i < subdivs; i++)
        {
            Vector3 curPos = GetQuadBezierPoint(_p0, _p1, _p2, (i + 1) / (float)subdivs);
            float length = Vector3.Magnitude(curPos - prevPos);

#if Debug
            Debug.DrawLine(curPos, prevPos, Color.yellow, 30f);
#endif

            _speedsByChordsLengths[i] = length;
            _totalLength += length;
            prevPos = curPos;
        }

        for (int i = 0; i < subdivs; i++)
        {
            _speedsByChordsLengths[i] = _speedsByChordsLengths[i] / _totalLength * subdivs;
        }
    }


    float GetSpeedByCoordLength(float t)
    {
        int pos = (int)(t * subdivs) - 1;
        pos = Mathf.Clamp(pos, 0, subdivs - 1);
        return _speedsByChordsLengths[pos];
    }

#if Debug
    void DrawPoint(Vector3 p)
    {
        Debug.DrawRay(p + Vector3.down * 0.5f * 0.1f, Vector3.up * 0.1f);
        Debug.DrawRay(p + -Vector3.forward * 0.5f * 0.1f, Vector3.forward * 0.1f);
    }

    void DrawPoints()
    {
        _testPositions.Add(transform.position);

        foreach (Vector3 t in _testPositions)
        {
            DrawPoint(t);
        }
    }
#endif
}