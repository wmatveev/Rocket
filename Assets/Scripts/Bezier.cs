using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Bezier
{
    public static Vector3 GetTwoPoint(Vector3 p0, Vector3 p1, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        
        return oneMinusT * p0 + t * p1;
    }

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
    //перенести?
    public static void PrepareCoords(int subdivs, Vector3 P0, Vector3 P1, Vector3 P2, ref float[] speedByChordsLengths, ref float totalLength)
    {
        speedByChordsLengths = new float[subdivs];

        Vector3 prevPos = P0;
        for (int i = 0; i < subdivs; i++)
        {
            Vector3 curPos = GetThreePoint(P0, P1, P2, (i + 1) / (float)subdivs);
            float length = Vector3.Magnitude(curPos - prevPos);

            speedByChordsLengths[i] = length;
            totalLength += length;
            prevPos = curPos;
        }

        for (int i = 0; i < subdivs; i++)
        {
            speedByChordsLengths[i] = speedByChordsLengths[i] / totalLength * subdivs;
        }
    }


    public static float GetSpeedByCoordLength(float t, int subdivs, ref float[] speedByChordsLengths)
    {
        int pos = (int)(t * subdivs) - 1;
        pos = Mathf.Clamp(pos, 0, subdivs - 1);
        return speedByChordsLengths[pos];
    }

}
