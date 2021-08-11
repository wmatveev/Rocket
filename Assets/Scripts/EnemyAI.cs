using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public static EnemyAI Instance { get; private set; }
    public float timeToReload = 2f;
    private float cooldown = 0f, asteroidCooldown = 0f;
    private float timeBetweenAsteroids = 0f;
    public eMode enemyMode;
    public enum eMode
    {
        Bezier,
        Linear
    }

    private void Awake()
    {
        Instance = this;
        //Object[] sprites = Resources.LoadAll("Sprites/Planets/EnemyPlanets", typeof(Sprite));
        //Sprite sprite = (Sprite)sprites[Random.Range(0, sprites.Length)];
        //gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
        timeBetweenAsteroids = Random.Range(0.2f, 2f);
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.amountOfERocketsOnLevel == 0)
        {
            GameManager.Instance.LevelIsCompleted();
        }
        cooldown += Time.deltaTime;
        asteroidCooldown += Time.deltaTime;
        if (GameManager.Instance.EnemyCanShoot() && cooldown > timeToReload)
            Shoot();
        if (asteroidCooldown > timeBetweenAsteroids)
            Asteroid();
    }

    private void Shoot()
    {       
        ThreeBezierScript bezier = GameManager.Instance.GetEnemyRocketFromPool();
        bezier.SetPoints(GameManager.Instance.enemyPlanet.transform.position, new Vector3(), GameManager.Instance.homePlanet.transform.position);
        bezier.currentMode = RocketLauncher.Mode.enemyAI;
        if (enemyMode == eMode.Bezier)
            bezier.RandomP1(8f, 10f);
        else 
            bezier.RandomP1(1f, 9f);
        cooldown = 0f;
    }

    private void Asteroid()
    {
        asteroidCooldown = 0f;
        timeBetweenAsteroids = Random.Range(2f, 4f);
        if (GameManager.Instance.asteroidsPool.Count == 0)
            return;
        AsteroidMotion asteroid = GameManager.Instance.GetAsteroidFromPool();
        Vector2 startPos = new Vector2();
        Vector2 endPos = new Vector2();
        if (Random.value > 0.5f)
        {
            //First start position and end position are randomized
            //End position is on the opposite side of the screen 
            //In this case, the asteroid flies out from the left side of the screen
            startPos.x = GameManager.Instance.minScreenEdge.x;
            endPos.x = GameManager.Instance.maxScreenEdge.x;
            startPos.y = Random.Range(GameManager.Instance.minScreenEdge.y, GameManager.Instance.maxScreenEdge.y);
            endPos.y = Random.Range(GameManager.Instance.minScreenEdge.y, GameManager.Instance.maxScreenEdge.y);
            //After randomising out path we calculate an angle of rotation
            Vector2 topLeft = new Vector2(GameManager.Instance.minScreenEdge.x, GameManager.Instance.maxScreenEdge.y);
            Vector2 dir1 = topLeft - startPos;
            Vector2 dir2 = endPos - startPos;
            //float angle is angle between left side of screen and asteroid's path
            float angle = Vector2.Angle(dir1, dir2);
            asteroid.gameObject.transform.eulerAngles = new Vector3(0f, 0f, -angle);

        }
        else
        {
            //In this case, the asteroid flies out from the right side of the screen
            endPos.x = GameManager.Instance.minScreenEdge.x;
            startPos.x = GameManager.Instance.maxScreenEdge.x;
            startPos.y = Random.Range(GameManager.Instance.minScreenEdge.y, GameManager.Instance.maxScreenEdge.y);
            endPos.y = Random.Range(GameManager.Instance.minScreenEdge.y, GameManager.Instance.maxScreenEdge.y);
            //права€ верхн€€ точка
            Vector2 topRight = GameManager.Instance.maxScreenEdge;
            Vector2 dir1 = topRight - startPos;
            Vector2 dir2 = endPos - startPos;
            float angle = Vector2.Angle(dir2, dir1);
            //float angle is angle between right side of screen and asteroid's path
            asteroid.gameObject.transform.eulerAngles = new Vector3(0f, 0f, angle);
        }
        asteroid.gameObject.transform.position = startPos;
        asteroid.startPos = startPos;
        asteroid.endPos = endPos;
        //random scale
        float scale = Random.Range(0.05f, 0.5f);
        asteroid.gameObject.transform.localScale = new Vector3(scale, scale, 1);
    }
}
