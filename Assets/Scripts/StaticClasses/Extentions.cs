using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extentions
{
    public static class Rotation
    {
        public static Quaternion LookAt2D(Vector2 startPos, Vector2 endPos)
        {
            Vector2 diff = endPos - startPos;
            diff.Normalize();

            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            return Quaternion.Euler(0f, 0f, rot_z - 90);
        }
    }

    public static class Geometry
    {
        public static Vector2 getPointOnEllipse(float a, float b, float currAngle, Vector2 center, float tiltAngle = 0)
        {
            Vector2 point = new Vector2();
            //Coords before tilt
            point.x = a * Mathf.Cos(Mathf.Deg2Rad * currAngle);
            point.y = b * Mathf.Sin(Mathf.Deg2Rad * currAngle);
            
            if (tiltAngle != 0)
            {
                //after tilt
                float x1 = point.x * Mathf.Cos(Mathf.Deg2Rad * tiltAngle) + point.y * Mathf.Sin(Mathf.Deg2Rad * tiltAngle);
                float y1 = -point.x * Mathf.Sin(Mathf.Deg2Rad * tiltAngle) + point.y * Mathf.Cos(Mathf.Deg2Rad * tiltAngle);
                point.x = x1;
                point.y = y1;
            }
            point.x = center.x - point.x;
            point.y = center.y - point.y;

            return point;
        }
    }
}
