using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    public static CameraFollowing Instance { get; private set; }
    public Transform target;
    public Vector3 Offset;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        Offset = new Vector3(gameObject.transform.position.x - target.position.x, 
            gameObject.transform.position.y - target.position.y, 
            transform.position.z);
    }

    void Update()
    {
        transform.position = target.position + Offset;
    }
}
