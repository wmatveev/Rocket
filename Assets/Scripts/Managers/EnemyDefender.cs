using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDefender : MonoBehaviour
{
    public static EnemyDefender Instance { get; private set; }
    public Vector2 planetCenter;
    public float radius;
    [SerializeField] private Transform spawnLine; //y

    [SerializeField] private float timeToReload;
    private float cooldown = 0f;

    [HideInInspector] public int shipCounter = 0, destroyedShipsCounter = 0;
    [HideInInspector] public List<Vector2> lastShipPositions = new List<Vector2>();
    [HideInInspector] public List<Vector2> shipPositions = new List<Vector2>();

    void Start()
    {
        Instance = this;
        planetCenter = gameObject.transform.position;
        if (timeToReload == 0)
            timeToReload = 2f;
        SetShipPositions();
    }

    void FixedUpdate()
    {
        cooldown += Time.deltaTime;
        if (cooldown > timeToReload)
        {
            cooldown = 0f;
            if (shipCounter < LevelInfo.Instance.eShipOnDefence && (shipCounter + destroyedShipsCounter) < LevelInfo.Instance.enemyFleetSize)
                SpawnSpaceship();
        }
    }

    private void SpawnSpaceship(Vector2 pos = new Vector2())
    {
        ShipController spaceShip = GameManager.Instance.GetESpaceshipFromPool();
        if (!spaceShip)
            return;
        if (lastShipPositions.Count > 0)
        {
            spaceShip.SetMovement(planetCenter, lastShipPositions[0]);
            lastShipPositions.Remove(lastShipPositions[0]);
        }
        else
            spaceShip.SetMovement(planetCenter, GetShipPosition());
        shipCounter++;
    }

    private void SetShipPositions()
    {
        for (int i = 0; i < LevelInfo.Instance.eShipOnDefence; i++)
        {
            Vector2 pos = new Vector2();
            if (i > 3)
            {
                float offset = (ScreenInfo.Instance.maxScreenEdge.x - ScreenInfo.Instance.minScreenEdge.x) * 0.1f;
                pos.y = spawnLine.position.y - offset * 2;
                pos.x = (ScreenInfo.Instance.maxScreenEdge.x - 2 * offset - ScreenInfo.Instance.minScreenEdge.x) /
                    (LevelInfo.Instance.eShipOnDefence - 5);
                pos.x *= (i - 4);
                pos.x = ScreenInfo.Instance.minScreenEdge.x + offset + pos.x;
                shipPositions.Add(pos);
            }
            else
            {
                float offset = (ScreenInfo.Instance.maxScreenEdge.x - ScreenInfo.Instance.minScreenEdge.x) * 0.2f;
                pos.y = spawnLine.position.y;
                pos.x = (ScreenInfo.Instance.maxScreenEdge.x - 2 * offset - ScreenInfo.Instance.minScreenEdge.x) / 
                    (LevelInfo.Instance.eShipOnDefence < 4 ? LevelInfo.Instance.eShipOnDefence : 3);
                pos.x *= i;
                pos.x = ScreenInfo.Instance.minScreenEdge.x + offset + pos.x;
                shipPositions.Add(pos);
            }
        }
    }
    private Vector2 GetShipPosition()
    {
        Vector2 pos = new Vector2();
        pos = shipPositions[0];
        shipPositions.Remove(pos);
        return pos;
    }

    private Vector2 CalculateShipPosition()
    {
        Vector2 pos = new Vector2();
        float offset = (ScreenInfo.Instance.maxScreenEdge.x - ScreenInfo.Instance.minScreenEdge.x) * 0.1f;
        pos.y = spawnLine.position.y;
        pos.x = (ScreenInfo.Instance.maxScreenEdge.x - offset - (ScreenInfo.Instance.minScreenEdge.x  + offset)) / (LevelInfo.Instance.eShipOnDefence - 1);
        pos.x *= shipCounter;
        pos.x = ScreenInfo.Instance.minScreenEdge.x + offset + pos.x; 
        return pos;
    }

    private Vector2 RandomShipPosition()
    {
        Vector2 randPos = planetCenter + Random.insideUnitCircle.normalized * radius;
        randPos.y = randPos.y > planetCenter.y ? randPos.y - 2 * (randPos.y - planetCenter.y) : randPos.y;
        return randPos;
    }

    private Vector2 GetShipPositionOnCircle()
    {
        float x;
        x = ((planetCenter.x + radius) - (planetCenter.x - radius)) / (LevelInfo.Instance.eShipOnDefence - 1);
        x *= shipCounter;
        x = planetCenter.x - radius + x;

        return new Vector2(x, GetYOnCircle(x));
    }

    private float GetYOnCircle(float x)
    {
        float x0 = planetCenter.x;
        float y0 = planetCenter.y;
        float y = y0 - Mathf.Sqrt(radius * radius - Mathf.Pow(x - x0, 2));
        return y;
    }
}
