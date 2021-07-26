using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotationSpeed;
    public RotateType rotateType;
    public enum RotateType
    {
        x,
        y,
        z
    }
    void Update()
    {
        if (rotateType == RotateType.x)
            transform.Rotate(Time.deltaTime * rotationSpeed, 0, 0);
        if (rotateType == RotateType.y)
            transform.Rotate(0, Time.deltaTime * rotationSpeed, 0);
        if (rotateType == RotateType.z)
            transform.Rotate(0, 0, Time.deltaTime * rotationSpeed);
    }
}
