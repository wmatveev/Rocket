using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaler : MonoBehaviour
{
    Vector3 scale, startScale;
    void Start()
    {
        scale = gameObject.transform.localScale;
        startScale = scale;
    }

    private bool growing = true;
    void FixedUpdate()
    {
        if (growing)
        {
            float currScale = scale.x;
            currScale += Time.deltaTime / 4;
            scale = new Vector3(currScale, currScale, 1);
            gameObject.transform.localScale = scale;
        }
        else
        {
            float currScale = scale.x;
            currScale -= Time.deltaTime / 4; 
            scale = new Vector3(currScale, currScale, 1);
            gameObject.transform.localScale = scale;
        }
        if (scale.y > startScale.y * 1.3)
            growing = false;   
        if (scale.y < startScale.y)
            growing = true;        
    }
}
