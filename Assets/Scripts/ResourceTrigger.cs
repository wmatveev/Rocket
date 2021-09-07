using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(ThreeBezierScript))]
public class ResourceTrigger : MonoBehaviour
{
    public GameObject endPos;
    private CircleCollider2D collider;
    private ThreeBezierScript bezier;
    private float startGravity;

    void Awake()
    {
        collider = GetComponent<CircleCollider2D>();
        bezier = GetComponent<ThreeBezierScript>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "AutoGun")
        {
            collider.enabled = false;
            bezier.enabled = true;
            bezier.SetPoints(new Vector3(transform.position.x, transform.position.y, 0), new Vector2(), endPos.transform.position);
            bezier.RandomP1(0, Vector2.Distance(transform.position, endPos.transform.position) / 5);
        }
        else return;
    }

    private void OnDisable()
    {
        if (!collider.enabled)
            collider.enabled = true;
        AsteroidGenerator.Instance.MineralBackToPool(gameObject);
    }

    private void OnEnable()
    {
        if (bezier.enabled)
            bezier.enabled = false;        
    }

    private void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }

}
