using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Bezier
{
    public static Vector3 GetThreePoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        
        return Mathf.Pow(oneMinusT, 2) * p0 +
            2 * oneMinusT * t * p1 + Mathf.Pow(t, 2) * p2;
    }

    public static Vector3 GetFourPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;

        return Mathf.Pow(oneMinusT, 3) * p0 +
            3f * Mathf.Pow(oneMinusT, 2) * t * p1 +
            3f * oneMinusT * Mathf.Pow(t, 2) * p2 +
            Mathf.Pow(t, 3) * p3;
    }

    // (2*p2-4*p1+2*p0)*t+2*p1-2*p0
    public static Vector2 GetFirstDerivativeForThreePoints(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        Vector3 returnRotation;

        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;

        returnRotation = (2 * p2 - 4 * p1 + 2 * p0) * t + 2 * p1 - 2 * p0;

        return returnRotation;
    }

    public static Vector3 GetFirstDerivativeForFourPoints(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;

        return 3f * Mathf.Pow(oneMinusT, 2) * (p1 - p0) +
            6f * oneMinusT * t * (p2 - p1) +
            3f * Mathf.Pow(t, 2) * (p3 - p2);
    }
}
