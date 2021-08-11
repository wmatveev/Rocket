using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;

//analytics
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using UnityEngine.Analytics;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance { get; private set; }
    #endregion

    public List<ThreeBezierScript> playerRocketsPool = new List<ThreeBezierScript>();
    public List<ThreeBezierScript> enemyRocketsPool = new List<ThreeBezierScript>();
    public List<ThreeBezierScript> currEnemyRockets = new List<ThreeBezierScript>();
    public List<AsteroidMotion> asteroidsPool = new List<AsteroidMotion>();

    [HideInInspector] public GameObject homePlanet, enemyPlanet;
    public GameObject explosion;
    public GameObject lightning;
   
    public int amountOfSelfGuidedRockets;
    public int eRocketsToLose;
    [HideInInspector] public int currentLevel;
    [HideInInspector] public int catchedERockets = 0;
    [HideInInspector] public int score = 0;

    private void Awake()
    {
        Instance = this;

        Time.timeScale = 1;
        homePlanet = GameObject.FindWithTag("HomePlanet");
        enemyPlanet = GameObject.FindWithTag("EnemyPlanet");

        SetCurrentLevel();
    }

    void Start()
    {
        SetScreenEdges();
        SetLevelSettings();
        StartFirebase();

        createRocketPool("Rocket", amountOfSelfGuidedRockets + amountOfPlayerRockets + 5);
        createRocketPool("EnemyRocket", amountOfERocketsOnLevel);
        createRocketPool("Asteroid", 10);
        ParticleTrigger triggerScript = lightning.GetComponent<ParticleTrigger>();
        if (triggerScript)
            triggerScript.SetTriggers();

    }
    #region ScreenSettings
    [SerializeField]
    private float distance;
    private void OnDrawGizmos()
    {
        Vector3 p1 = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, distance));
        Vector3 p2 = Camera.main.ScreenToWorldPoint(new Vector3(0f, Camera.main.pixelHeight, distance));
        Vector3 p3 = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, distance));
        Vector3 p4 = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, 0f, distance));
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);
    }
    public Vector2 minScreenEdge, maxScreenEdge;
    private void SetScreenEdges()
    {
        Vector3 bottomLeft, topRight;
        bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, distance));
        topRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, distance));

        minScreenEdge = new Vector2(bottomLeft.x, bottomLeft.y);
        maxScreenEdge = new Vector2(topRight.x, topRight.y);
    }

    public bool IsOnTheScreen(Vector3 position)
    {
        if (minScreenEdge.x < position.x && position.x < maxScreenEdge.x &&
            minScreenEdge.y < position.y && position.y < maxScreenEdge.y)
            return true;
        return false;
    }
    #endregion
    #region LevelSettings
    [HideInInspector]
    public int amountOfERocketsOnLevel = 0, //how many rockets we need to destroy to win
        eRocketsToLaunch = 0, //allowed number enemy rockets at the same time on level
        launchedERockets = 0, //counter of launched rockets
        amountOfPlayerRockets;
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
        amountOfPlayerRockets = (int)(amountOfERocketsOnLevel * 1.2f) + 4;
        eRocketsToLaunch = (currentLevel / 5) + 1; 
        speedCoef = 1f + (currentLevel % 5)/ 10f;
        EnemyAI.Instance.timeToReload = (currentLevel < 5) ? 0 : (float)(1f + Random.value);
    }
    #endregion

    #region Analytics
    //firebase info
    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    private void StartFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
                Debug.LogError("Firebase analytics failed: " + dependencyStatus);
        });
    }

    void InitializeFirebase()
    {
        //Enabling data collection
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        
        //We can add it later:
        //Set the user's sign up method
        //FirebaseAnalytics.SetUserProperty(FirebaseAnalytics.UserPropertySignUpMethod, "Google");
        //Set the user ID
        //FirebaseAnalytics.SetUserId("uber_user_510");
    }

    private void EndlevelAnalytics()
    {
        float winRate;
        if (!PlayerPrefs.HasKey("LosingOnThisLevel"))
            winRate = 1f;
        else
        {
            winRate = 1f / (1f + (float)PlayerPrefs.GetInt("LosingOnThisLevel"));
            PlayerPrefs.DeleteKey("LosingOnThisLevel");
        }

        LevelInfo.Instance.SetEndLevelInfo(winRate);

        string param = "winRateOnLevel" + currentLevel.ToString();

        Analytics.CustomEvent("level " + currentLevel + " end", new Dictionary<string, object>
        {
            { param, winRate },
            { "score", score }
        });

        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelEnd, FirebaseAnalytics.ParameterLevel, currentLevel);
        FirebaseAnalytics.LogEvent("winrate", param, winRate);
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventPostScore, FirebaseAnalytics.ParameterScore, score);
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
            if (type == "Asteroid") 
                asteroidsPool.Add(tmp_rocket.GetComponent<AsteroidMotion>());
            tmp_rocket.SetActive(false);
        }
        Destroy(rocket);
    }

    public AsteroidMotion GetAsteroidFromPool()
    {
        AsteroidMotion asteroidTmp = new AsteroidMotion();
        asteroidTmp = asteroidsPool[0];
        asteroidsPool.Remove(asteroidTmp);
        asteroidTmp.gameObject.SetActive(true);
        asteroidTmp.gameObject.transform.parent = null;
        return asteroidTmp;
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
            if (mode == RocketLauncher.Mode.armageddon || mode == RocketLauncher.Mode.rocketGuidance)
            {
                Animator animator = rocketTmp.GetComponent<Animator>();
                animator.runtimeAnimatorController = Resources.Load("Animation/Rockets/Fire2") as RuntimeAnimatorController;
            }
        }
        UIMenu.Instance.SetRocketsAmount();
        return rocketTmp;
    }
    private bool CanGetRocket(RocketLauncher.Mode mode)
    {
        if (mode == RocketLauncher.Mode.armageddon)
            return true;

        if (mode == RocketLauncher.Mode.rocketGuidance && (amountOfSelfGuidedRockets > 0))
        {
            amountOfSelfGuidedRockets--;
            return true;
        }
        if ((mode == RocketLauncher.Mode.manualAiming || mode == RocketLauncher.Mode.tapLaunch) && amountOfPlayerRockets > 0)
        {
            //amountOfPlayerRockets--;
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

    public void AsteroidBackToPool(AsteroidMotion asteroid)
    {
        asteroid.gameObject.transform.SetParent(asteroidsPool[0].gameObject.transform.parent);
        asteroidsPool.Add(asteroid);
        asteroid.gameObject.SetActive(false);
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
        UIMenu.Instance.ResetLvlInfo();
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

    public void Explose(Vector3 coord)
    {
        Instantiate(explosion, coord, Quaternion.identity);
    }
    #endregion

   public void LevelIsCompleted()
    {
        UIMenu.Instance.endLvlFade.SetActive(true);
        Time.timeScale = 0;
        EndlevelAnalytics();
        Text text = UIMenu.Instance.winPanel.transform.Find("Score").gameObject.GetComponent<Text>();
        text.text = "Score: " + score;
        UIMenu.Instance.winPanel.SetActive(true);
    }
    
    public void LevelIsLosed()
    {
        catchedERockets++;
        if (catchedERockets >= eRocketsToLose)
        {
            UIMenu.Instance.endLvlFade.SetActive(true);
            if (PlayerPrefs.HasKey("LosingOnThisLevel"))
                PlayerPrefs.SetInt("LosingOnThisLevel", PlayerPrefs.GetInt("LosingOnThisLevel") + 1);
            else
                PlayerPrefs.SetInt("LosingOnThisLevel", 1);

            Time.timeScale = 0;
            UIMenu.Instance.losePanel.SetActive(true);
        }
    }
}