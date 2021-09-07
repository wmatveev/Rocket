using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint1, spawnPoint2;
    [SerializeField] private float timeToReload;
    [SerializeField] private float speed;
    private float cooldown = 0f;
    private Transform target;
    private Health health;

    [HideInInspector] public bool isMoving = true;
    private Vector2 startPos, endPos;
    private float t = 0;
    void Start()
    {
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0);
        health = GetComponent<Health>();
        if (gameObject.layer == 7)
            target = GameObject.FindGameObjectWithTag("AutoGun").transform;
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            Move();
            return;
        }
        cooldown += Time.deltaTime;
        if (cooldown > timeToReload)
        {
            cooldown = 0f;
            if (!spawnPoint2 || !spawnPoint1)
            {
                AutoGunRocketLaunch(gameObject.transform);
            }
            else
            {
                AutoGunRocketLaunch(spawnPoint1);
                AutoGunRocketLaunch(spawnPoint2);
            }
        }
    }


    private void Move()
    {
        if (endPos == startPos)
        {
            isMoving = false;
            return;
        }
        t += Time.deltaTime * speed;
        gameObject.transform.position = Bezier.GetTwoPoint(startPos, endPos, t);

        if (t >= 1)
        {
            isMoving = false;
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
    }

    public void SetMovement(Vector2 start, Vector2 end)
    {
        startPos = start;
        endPos = end;
        isMoving = true;
        transform.rotation = Extentions.Rotation.LookAt2D(startPos, endPos);
    }

    private void AutoGunRocketLaunch(Transform spawnPos)
    {
        if (gameObject.layer == 7)
        {
            EnemyAutoGun(spawnPos);
        }
        if (gameObject.layer == 6)
        {
            PlayerAutoGun(spawnPos);
        }
    }

    private void PlayerAutoGun(Transform spawnPos)
    {
        if (GameManager.Instance.playerRocketsPool.Count == 0)
            return;
        ThreeBezierScript bezier = GameManager.Instance.GetRocketFromPool(RocketLauncher.Mode.autoGun);
        bezier.currentMode = RocketLauncher.Mode.autoGun;
        Vector3 rayUp = new Vector3(spawnPos.position.x, spawnPos.position.y + 5f, 0);
        Ray ray = new Ray(spawnPos.position, rayUp - spawnPos.position);
        bezier.SetPoints(spawnPos.position, ray.GetPoint(5f * 100), ray.GetPoint(5f * 500));
    }

    private void EnemyAutoGun(Transform spawnPos)
    {
        if (GameManager.Instance.enemyRocketsPool.Count == 0)
            return;
        ThreeBezierScript bezier = GameManager.Instance.GetEnemyRocketFromPool();
        Ray ray = new Ray(spawnPos.position, target.position - spawnPos.position);
        bezier.currentMode = RocketLauncher.Mode.enemyAI;
        bezier.SetPoints(spawnPos.position,
                        ray.GetPoint(Vector2.Distance(spawnPos.position, target.position)),
                        ray.GetPoint(Vector2.Distance(spawnPos.position, target.position) * 2));
    }

    private void OnEnable()
    {
        t = 0;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == gameObject.layer || col.gameObject.layer == 8)
            return;

        if (gameObject.layer == 7)
        {
            health.TakeHit(10);
            if (health.currentHealth <= 0)
            {
                GameManager.Instance.Explose(gameObject.transform.position);
                EnemyDefender.Instance.lastShipPositions.Add(endPos);
                GameManager.Instance.ESpaceshipBackToPool(this);
            }
        }
        if (gameObject.tag == "AutoGun")
        {
            health.TakeHit(10);
            if (health.currentHealth <= 0)
            {
                GameManager.Instance.Explose(gameObject.transform.position);
                LevelInfo.Instance.LevelIsLosed();
            }
        }
    }
}
