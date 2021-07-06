using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public static EnemyAI Instance { get; private set; }
    public GameObject rocket;
    public ThreeBezierScript threeBezierScript;
    public float timeToReload = 2f;
    private float cooldown = 0f;

    private void Awake()
    {
        Instance = this;
        Object[] sprites = Resources.LoadAll("Sprites/Planets/EnemyPlanets", typeof(Sprite));
        Sprite sprite = (Sprite)sprites[Random.Range(0, sprites.Length)];
        gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
    }

    void Start()
    {
        
    }

    void Update()
    {
        cooldown += Time.deltaTime;
        //if (cooldown > timeToReload ) 
        if (GameManager.Instance.currEnemyRockets.Count == 0)
            Shoot();
    }

    private void Shoot()
    {       
        if (GameManager.Instance.amountOfEnemyRocketsOnLevel == 0)
        {
            GameManager.Instance.LevelIsCompleted();
        }

        KeyValuePair<GameObject, ThreeBezierScript> enemyRocketInfo;
        enemyRocketInfo = GameManager.Instance.GetEnemyRocketFromPool();
        rocket = enemyRocketInfo.Key;
        ThreeBezierScript bezier = enemyRocketInfo.Value;

        bezier.SetPoints(GameManager.Instance.enemyPlanet.transform.position, new Vector3(), GameManager.Instance.homePlanet.transform.position);
        bezier.RandomP1();
        bezier.currentMode = RocketLauncher.Mode.enemyAI;
        cooldown = 0f;
    }


}
