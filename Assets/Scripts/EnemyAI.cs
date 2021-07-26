using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public static EnemyAI Instance { get; private set; }
    public float timeToReload = 2f;
    private float cooldown = 0f;

    public eMode enemyMode;
    public enum eMode
    {
        Bezier,
        Linear
    }

    private void Awake()
    {
        Instance = this;
    }

    void FixedUpdate()
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
        if (enemyMode == eMode.Bezier)
            bezier.RandomP1(3f, 6f);
        else 
            bezier.RandomP1(1f, 9f);
        cooldown = 0f;
    }

    private void SelectRandomSprite()
    {
        Object[] sprites = Resources.LoadAll("Sprites/Planets/EnemyPlanets", typeof(Sprite));
        Sprite sprite = (Sprite)sprites[Random.Range(0, sprites.Length)];
        gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
    }
}
