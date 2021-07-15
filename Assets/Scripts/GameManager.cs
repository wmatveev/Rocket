using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Analytics;
public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance { get; private set; }
    #endregion

    public List<ThreeBezierScript> playerRocketsPool = new List<ThreeBezierScript>();
    public List<ThreeBezierScript> enemyRocketsPool = new List<ThreeBezierScript>();
    public List<ThreeBezierScript> currEnemyRockets = new List<ThreeBezierScript>();

    [HideInInspector] public GameObject losePanel, winPanel;
    [HideInInspector] public GameObject homePlanet, enemyPlanet;
   
    public int amountOfSelfGuidedRockets;
    public int amountOfPlayerRockets;

    [HideInInspector] public int currentLevel;
    [HideInInspector] public int score = 0;
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

        SetCurrentLevel();
    }

    void Start()
    {
        SetLevelSettings();

        createRocketPool("Rocket", amountOfSelfGuidedRockets + amountOfPlayerRockets + 5);
        createRocketPool("EnemyRocket", amountOfERocketsOnLevel);        
    }

    #region LevelSettings
    [HideInInspector]
    public int amountOfERocketsOnLevel = 0, //how many rockets we need to destroy to win
        eRocketsToLaunch = 0, //allowed number enemy rockets at the same time on level
        launchedERockets = 0; //counter of launched rockets
    private float speedCoef;
    private void SetCurrentLevel()
    {
        if (PlayerPrefs.HasKey("currentLevel"))
            currentLevel = PlayerPrefs.GetInt("currentLevel");
        else
        {
            currentLevel = 1;
            PlayerPrefs.SetInt("currentLevel", 1);
        }
    }
    private void SetLevelSettings()
    {
        amountOfERocketsOnLevel = 3 + currentLevel;
        eRocketsToLaunch = (currentLevel / 5) + 1; 
        speedCoef = 1f + (currentLevel % 5)/ 10f;
        EnemyAI.Instance.timeToReload = (currentLevel < 5) ? 0 : (float)(1f + Random.value);
    }
    #endregion

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
                playerRocketsPool.Add(tmp_rocket.GetComponent<ThreeBezierScript>());
            if (type == "EnemyRocket")
            {
                ThreeBezierScript erocket = tmp_rocket.GetComponent<ThreeBezierScript>();
                erocket.speed *= speedCoef;
                enemyRocketsPool.Add(erocket);
            }
            tmp_rocket.SetActive(false);
        }
        Destroy(rocket);
    }

    public ThreeBezierScript GetRocketFromPool(RocketLauncher.Mode mode)
    {
        ThreeBezierScript rocketTmp = new ThreeBezierScript();
        if (CanGetRocket(mode)) 
        {
            rocketTmp = playerRocketsPool[0];
            playerRocketsPool.Remove(rocketTmp);
            rocketTmp.gameObject.SetActive(true);
            rocketTmp.gameObject.transform.parent = null;
        }
        UIMenu.Instance.SetRocketsAmount();
        return rocketTmp;
    }
    private bool CanGetRocket(RocketLauncher.Mode mode)
    {
        if (mode == RocketLauncher.Mode.rocketGuidance && (amountOfSelfGuidedRockets > 0))
        {
            amountOfSelfGuidedRockets--;
            return true;
        }
        if ((mode == RocketLauncher.Mode.manualAiming || mode == RocketLauncher.Mode.tapLaunch) && amountOfPlayerRockets > 0)
        {
            amountOfPlayerRockets--;
            return true;
        }
        return false;
    }
    public void RocketBackToPool(ThreeBezierScript rocket)
    {
        rocket.gameObject.transform.SetParent(playerRocketsPool[0].gameObject.transform.parent);
        rocket.T = 0;
        playerRocketsPool.Add(rocket);
        rocket.gameObject.SetActive(false);
        rocket.gameObject.transform.position = rocket.P0;
    }

    public ThreeBezierScript GetEnemyRocketFromPool()
    {
        ThreeBezierScript rocketTmp = new ThreeBezierScript();
        if (enemyRocketsPool.Count > 0)
        {
            rocketTmp = enemyRocketsPool[0];
            rocketTmp.gameObject.gameObject.SetActive(true);
            enemyRocketsPool.Remove(rocketTmp);
            rocketTmp.gameObject.transform.parent = null;
            rocketTmp.isDrawn = false;
            currEnemyRockets.Add(rocketTmp);
            launchedERockets++;
        }
        return rocketTmp;
    }
    
    public void EnemyRocketBackToPool(ThreeBezierScript enemyRocket)
    {
        if (currEnemyRockets.Contains(enemyRocket))       
            currEnemyRockets.Remove(enemyRocket);
        score += (int)(100 * (1 - enemyRocket.T));
        UIMenu.Instance.SetScore();
        enemyRocket.T = 0;
        enemyRocket.gameObject.transform.SetParent(enemyRocketsPool[0].gameObject.transform.parent);
        enemyRocketsPool.Add(enemyRocket);
        enemyRocket.gameObject.SetActive(false);
        enemyRocket.gameObject.transform.position = enemyRocket.P0;
        amountOfERocketsOnLevel--;
        if (currEnemyRockets.Count == 0)
            launchedERockets = 0;
    }

    public bool EnemyCanShoot()
    {
        return launchedERockets < eRocketsToLaunch && amountOfERocketsOnLevel - launchedERockets > 0;
    }
    #endregion

   public void LevelIsCompleted()
    {
        Time.timeScale = 0;
        SentEndlevelAnalytics();
        Text text = winPanel.transform.Find("Score").gameObject.GetComponent<Text>();
        text.text = "Score: " + score;
        winPanel.SetActive(true);
    }
    
    public void LevelIsLosed()
    {
        if (PlayerPrefs.HasKey("LosingOnThisLevel"))
            PlayerPrefs.SetInt("LosingOnThisLevel", PlayerPrefs.GetInt("LosingOnThisLevel") + 1);
        else
            PlayerPrefs.SetInt("LosingOnThisLevel", 1);

        Time.timeScale = 0;
        losePanel.SetActive(true);
    }

    private void SentEndlevelAnalytics()
    {
        float winRate;
        if (!PlayerPrefs.HasKey("LosingOnThisLevel"))
            winRate = 1f;
        else winRate = 1f / (1f + (float)PlayerPrefs.GetInt("LosingOnThisLevel"));

        string param = "winRateOnLevel" + currentLevel.ToString();

        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelEnd,
            new Parameter(FirebaseAnalytics.ParameterLevel, currentLevel), 
            new Parameter(param, winRate));

        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventPostScore, FirebaseAnalytics.ParameterScore, score);
    }
}