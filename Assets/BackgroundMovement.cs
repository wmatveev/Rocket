using UnityEngine;
using System.Collections;

public class BackgroundMovement : MonoBehaviour
{
    public float speed;
    private Vector2 offset = Vector2.zero;
    private Material material;
    private void Start()
    {
        var height = Camera.main.orthographicSize * 2f;
        var width = height * Screen.width / Screen.height;

        transform.localScale = new Vector3(width, height, 0);

        material = GetComponent<Renderer>().material;
        offset = material.GetTextureOffset("_MainTex");
    }

    private void Update()
    {
        offset.y += speed * Time.deltaTime;
        material.SetTextureOffset("_MainTex", offset);
    }
}