using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rotator : MonoBehaviour
{
    public float rotationSpeed;
    public RotateType rotateType;
    private Image image;
    
    public enum RotateType
    {
        x,
        y,
        z
    }
    public bool isBlinking;
    public float deltaAlpha;
    private float alpha = 0f;

    public bool reverse;
    public float degrees;
    private float startRotation;
    private void Start()
    {
        image = GetComponent<Image>();
        if (isBlinking)
        {
            Color color = new Color();
            color = image.color;
            color.a = 0;
            alpha = color.a;
            image.color = color;
        }
        startRotation = GetRotation();
    }
    void FixedUpdate()
    {
        if (rotationSpeed != 0)
        {
            if (rotateType == RotateType.x)
                transform.Rotate(Time.deltaTime * rotationSpeed, 0, 0);
            if (rotateType == RotateType.y)
                transform.Rotate(0, Time.deltaTime * rotationSpeed, 0);
            if (rotateType == RotateType.z)
                transform.Rotate(0, 0, Time.deltaTime * rotationSpeed);
        }

        if (isBlinking)
            Blinking();
        if (reverse)
            CheckRotation();
    }

    private bool fading = false;
    private void Blinking()
    {        
        Color color = new Color();
        color = image.color;
        

        if (fading)
        {
            alpha -= deltaAlpha * Time.deltaTime;
        }
        else
        {
            alpha += deltaAlpha * Time.deltaTime;

        }
        color.a = alpha;
        image.color = color;
        if (color.a >= 1)
            fading = true;
        if (color.a <= 0)
            fading = false;
    }

    private void CheckRotation()
    {
        if (degrees < Mathf.Abs(GetRotation() - startRotation))
            rotationSpeed = -rotationSpeed;
    }

    float GetRotation()
    {
        float degrees = 0f;
        Quaternion rotation = gameObject.transform.rotation;
        if (rotateType == RotateType.x)
            degrees = rotation.x;
        if (rotateType == RotateType.y)
            degrees = rotation.y;
        if (rotateType == RotateType.z)
            degrees = rotation.z;
        degrees *= 2 * 57.29577951308f;
        return degrees;
    }
}