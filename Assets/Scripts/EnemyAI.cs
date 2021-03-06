using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public static EnemyAI Instance { get; private set; }
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
    void Update()
    {
        if (GameManager.Instance.amountOfERocketsOnLevel == 0)
        {
            GameManager.Instance.LevelIsCompleted();
        }
        cooldown += Time.deltaTime;
        if (GameManager.Instance.EnemyCanShoot() && cooldown > timeToReload)
            Shoot();
    }

    private void Shoot()
    {       
        ThreeBezierScript bezier = GameManager.Instance.GetEnemyRocketFromPool();

        bezier.SetPoints(GameManager.Instance.enemyPlanet.transform.position, new Vector3(), GameManager.Instance.homePlanet.transform.position);
        bezier.currentMode = RocketLauncher.Mode.enemyAI;
        //bezier.RandomP1(8f, 10f);
        bezier.RandomP1(1f, 9f);
        cooldown = 0f;
    }
}
