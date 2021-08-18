using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidMotion : MonoBehaviour
{
    public float speed;
    public Vector2 startPos, endPos;
    private float t = 0;

    void FixedUpdate()
    {
        MoveForward();
    }

    private void MoveForward()
    {
        t += Time.deltaTime * speed;

        gameObject.transform.position = Bezier.GetTwoPoint(startPos, endPos, t);
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 22);

        if (t >= 1)
            GameManager.Instance.AsteroidBackToPool(this);
    }

    private void OnEnable()
    {
        t = 0;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == gameObject.layer)
                return;
        if (col.gameObject.layer == 6)
        {
            GameManager.Instance.Explose(gameObject.transform.position);
            GameManager.Instance.AsteroidBackToPool(this);
        }
    }
}
