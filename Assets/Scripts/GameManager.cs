using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance { get; private set; }
    #endregion
    
    public List<KeyValuePair<GameObject, ThreeBezierScript>> playerRocketsPool = new List<KeyValuePair<GameObject, ThreeBezierScript>>();
    public List<KeyValuePair<GameObject, ThreeBezierScript>> enemyRocketsPool = new List<KeyValuePair<GameObject, ThreeBezierScript>>();
    public List<KeyValuePair<GameObject, ThreeBezierScript>> currEnemyRockets = new List<KeyValuePair<GameObject, ThreeBezierScript>>();

    public GameObject losePanel, winPanel;
    public GameObject homePlanet, enemyPlanet;
    public int amountOfPlayerRockets;
    public int amountOfEnemyRocketsOnLevel = 0;

    public int currentLevel;
    public int score = 0;
    private void Awake()
    {
        Instance = this;

        Time.timeScale = 1;

        homePlanet = GameObject.FindWithTag("HomePlanet");
        enemyPlanet = GameObject.FindWithTag("EnemyPlanet");

        losePanel = GameObject.Find("losePanel");
        losePanel.SetActive(false);
        winPanel = GameObject.Find("winPanel");
        winPanel.SetActive(false);

        if (PlayerPrefs.HasKey("currentLevel"))
            currentLevel = PlayerPrefs.GetInt("currentLevel");
        else
        {
            currentLevel = 1;
            PlayerPrefs.SetInt("currentLevel", 1);
        }
        amountOfEnemyRocketsOnLevel = 4 + currentLevel;
        createRocketPool("Rocket", amountOfPlayerRockets + 40);
        createRocketPool("EnemyRocket", amountOfEnemyRocketsOnLevel);
    }

    #region RocketsManagement
    private void createRocketPool(string type, int count)
    {
        GameObject rocketsPoolObj = new GameObject();
        rocketsPoolObj.name = type + 's';
        GameObject rocket = GameObject.FindWithTag(type);
        for (int i = 0; i < count; i++)
        {
            var tmp_rocket = Instantiate(rocket, rocketsPoolObj.transform); 
            if (type == "Rocket")
                playerRocketsPool.Add(new KeyValuePair<GameObject, ThreeBezierScript>(tmp_rocket, tmp_rocket.GetComponent<ThreeBezierScript>()));
            if (type == "EnemyRocket") 
                enemyRocketsPool.Add(new KeyValuePair<GameObject, ThreeBezierScript>(tmp_rocket, tmp_rocket.GetComponent<ThreeBezierScript>()));
            tmp_rocket.SetActive(false);
        }
        Destroy(rocket);
    }

    public KeyValuePair<GameObject, ThreeBezierScript> GetRocketFromPool(RocketLauncher.Mode mode)
    {
        KeyValuePair<GameObject, ThreeBezierScript> rocketTmp = new KeyValuePair<GameObject, ThreeBezierScript>();
        if (mode == RocketLauncher.Mode.manualAiming || (amountOfPlayerRockets > 0)) 
        {
            rocketTmp = playerRocketsPool[0];
            playerRocketsPool.Remove(rocketTmp);
            rocketTmp.Key.gameObject.SetActive(true);
            rocketTmp.Key.transform.parent = null;
            if (mode == RocketLauncher.Mode.rocketGuidance)
                amountOfPlayerRockets--;
        }
        return rocketTmp;
    }

    public void RocketBackToPool(KeyValuePair<GameObject, ThreeBezierScript> rocket)
    {
        rocket.Key.transform.SetParent(playerRocketsPool[0].Key.transform.parent);
        rocket.Value.T = 0;
        playerRocketsPool.Add(rocket);
        rocket.Key.SetActive(false);
        rocket.Key.transform.position = rocket.Value.P0;
    }

    public KeyValuePair<GameObject, ThreeBezierScript> GetEnemyRocketFromPool()
    {
        KeyValuePair<GameObject, ThreeBezierScript> rocketTmp = new KeyValuePair<GameObject, ThreeBezierScript>();
        if (enemyRocketsPool.Count > 0)
        {
            rocketTmp = enemyRocketsPool[0];
            rocketTmp.Key.gameObject.SetActive(true);
            enemyRocketsPool.Remove(rocketTmp);
            rocketTmp.Key.transform.parent = null;
            rocketTmp.Value.Speed *= (1 + GameManager.Instance.currentLevel / 4);
            currEnemyRockets.Add(rocketTmp);
        }
        return rocketTmp;
    }
    
    public void EnemyRocketBackToPool(KeyValuePair<GameObject, ThreeBezierScript> enemyRocket)
    {
        if (currEnemyRockets.Contains(enemyRocket))       
            currEnemyRockets.Remove(enemyRocket);
        score += (int)(100 * enemyRocket.Value.T);
        enemyRocket.Value.T = 0;
        enemyRocket.Key.transform.SetParent(enemyRocketsPool[0].Key.transform.parent);
        enemyRocketsPool.Add(enemyRocket);
        enemyRocket.Key.SetActive(false);
        enemyRocket.Key.transform.position = enemyRocket.Value.P0;
        amountOfEnemyRocketsOnLevel--;
    }
    #endregion

   public void LevelIsCompleted()
    {
        Time.timeScale = 0;
        Text text = winPanel.transform.Find("Score").gameObject.GetComponent<Text>();
        text.text += " " + score;
        winPanel.SetActive(true);
    }
    
    public void LevelIsLosed()
    {
        Time.timeScale = 0;
        losePanel.SetActive(true);
    }
}
