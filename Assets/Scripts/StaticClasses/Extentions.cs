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
}
