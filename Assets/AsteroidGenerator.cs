using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidGenerator : MonoBehaviour
{
    public float timeToReload;
    private float cooldown = 0f;
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        cooldown += Time.deltaTime;
        if (cooldown > timeToReload)
        {
            cooldown = 0f;
            LaunchAsteroid();
        }
    }
    private void LaunchAsteroid()
    {
        if (GameManager.Instance.asteroidsPool.Count == 0)
            return;
        AsteroidMotion asteroid = GameManager.Instance.GetAsteroidFromPool();
        Vector2 startPos = new Vector2();
        Vector2 endPos = new Vector2();

        startPos.y = GameManager.Instance.maxScreenEdge.y;
        endPos.y = GameManager.Instance.minScreenEdge.y;
        startPos.x = Random.Range(GameManager.Instance.minScreenEdge.x, GameManager.Instance.maxScreenEdge.x);
        endPos.x = startPos.x;

        asteroid.startPos = startPos;
        asteroid.endPos = endPos;
    }
}
