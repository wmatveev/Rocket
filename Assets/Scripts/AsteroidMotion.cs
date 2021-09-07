using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidMotion : MonoBehaviour
{
    public float speed;
    public Vector2 startPos, endPos;
    private float t = 0;
    private bool isExploding = false;

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
            AsteroidGenerator.Instance.AsteroidBackToPool(this);
    }

    private void OnEnable()
    {
        t = 0;
        isExploding = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == gameObject.layer)
                return;
        if (col.gameObject.layer == 6)
        {
            if (!isExploding)
            {
                isExploding = true;
                GameManager.Instance.Explose(gameObject.transform.position);
                AsteroidGenerator.Instance.SpawnMineralAt(gameObject.transform.position);
                AsteroidGenerator.Instance.AsteroidBackToPool(this);
            }
            else return;
        }
    }
}
