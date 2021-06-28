using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance { get; private set; }
    #endregion

    public List<GameObject> enemyRocket;
    public List<GameObject> playerRocketsPool = new List<GameObject>();
    public int amountOfRockets;
    void Start()
    {
        Instance = this;

        GameObject rocketsPoolObj = new GameObject();
        GameObject rocket = GameObject.FindWithTag("Rocket");
        for (int i = 0; i < amountOfRockets; i++)
        {
            var tmp_rocket = Instantiate(rocket, rocketsPoolObj.transform); 
            playerRocketsPool.Add(tmp_rocket);
            tmp_rocket.SetActive(false);
        }
        Destroy(rocket);
    }

    public GameObject GetRocketFromPool()
    {        
        if (playerRocketsPool.Count > 0)
        {
            var rocketTmp = playerRocketsPool[0];
            playerRocketsPool.Remove(rocketTmp);
            rocketTmp.gameObject.SetActive(true);
            rocketTmp.transform.parent = null;
            return rocketTmp;
        }
        return null;
    }
}
