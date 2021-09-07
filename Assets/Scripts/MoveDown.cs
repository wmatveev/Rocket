using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveDown : MonoBehaviour
{
    [SerializeField] private StopCondition stopCondition;
    [SerializeField] private GameObject conditionGO;
    public float speed;
    private float screenHeight, pathLength;
    private float startYPosition;

    private enum StopCondition
    {
        none,
        gameObjIsOnTop,
        gameObjIsOnBottom
    }

    void Start()
    {
        if (stopCondition != StopCondition.none && !conditionGO)
            conditionGO = gameObject;
    }

    void FixedUpdate()
    {
        float newY = gameObject.transform.localPosition.y - Time.deltaTime * speed;
        if (stopCondition != StopCondition.none)// && newY < startYPosition - pathLength + screenHeight)
        {
            if (stopCondition == StopCondition.gameObjIsOnTop)
            {
                if (conditionGO.transform.position.y <= ScreenInfo.Instance.maxScreenEdge.y)
                {
                    if (SceneManager.GetActiveScene().name == "ArcadeModeNew")
                    {
                        GameManager.Instance.enemyPlanet.GetComponent<EnemyDefender>().enabled = true;
                        Destroy(AsteroidGenerator.Instance);
                    }
                    Destroy(this);
                }
            }
            if (stopCondition == StopCondition.gameObjIsOnBottom)
            {
                if (conditionGO.transform.position.y <= ScreenInfo.Instance.minScreenEdge.y)
                {
                    if (SceneManager.GetActiveScene().name == "ArcadeModeNew")
                    {
                        GameManager.Instance.enemyPlanet.GetComponent<EnemyDefender>().enabled = true;
                        Destroy(AsteroidGenerator.Instance);
                    }
                    Destroy(this);
                }
            }
        }
        gameObject.transform.localPosition = new Vector2(gameObject.transform.localPosition.x, newY);
    }
}
