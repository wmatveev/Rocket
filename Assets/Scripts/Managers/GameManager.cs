using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance { get; private set; }
    #endregion

    public List<ThreeBezierScript> playerRocketsPool = new List<ThreeBezierScript>();
    public List<ThreeBezierScript> enemyRocketsPool = new List<ThreeBezierScript>();
    public List<ThreeBezierScript> currEnemyRockets = new List<ThreeBezierScript>();
    public List<ShipController> enemyFleet = new List<ShipController>();   
    public GameObject gameObjectsStorage;
    [HideInInspector] public Vector3 spawnPosition = new Vector3();

    [HideInInspector] public GameObject homePlanet, enemyPlanet;
    public GameObject explosion;
    public GameObject lightning;

    private void Awake()
    {
        Instance = this;

        Time.timeScale = 1;
        homePlanet = GameObject.FindWithTag("HomePlanet");
        enemyPlanet = GameObject.FindWithTag("EnemyPlanet");

    }

    void Start()
    {
        AnalyticsManager.Instance.StartFirebase();

        InitializeStorage();
        createPool("Rocket", LevelInfo.Instance.amountOfSelfGuidedRockets + LevelInfo.Instance.amountOfPlayerRockets + 5);
        createPool("EnemyRocket", LevelInfo.Instance.amountOfERocketsOnLevel);
        createPool("ESpaceShip", LevelInfo.Instance.enemyFleetSize);

        if (lightning)
        {
            ParticleTrigger triggerScript = lightning.GetComponent<ParticleTrigger>();
            if (triggerScript)
                triggerScript.SetTriggers();
        }
    }    
    
    #region RocketsManagement

    public void InitializeStorage()
    {
        gameObjectsStorage = new GameObject();
        gameObjectsStorage.transform.position = spawnPosition;
        gameObjectsStorage.name = "Game Objects Storage";
    }

    private void createPool(string type, int count)
    {
        ///! - > find all
        GameObject rocket = GameObject.FindWithTag(type);
        if (!rocket)
            return;        

        for (int i = 0; i < count; i++)
        {
            var tmp_rocket = Instantiate(rocket, gameObjectsStorage.transform); 
            if (type == "Rocket")
                playerRocketsPool.Add(tmp_rocket.GetComponent<ThreeBezierScript>());
            if (type == "EnemyRocket")
            {
                ThreeBezierScript erocket = tmp_rocket.GetComponent<ThreeBezierScript>();
                erocket.speed *= LevelInfo.Instance.speedCoef;
                enemyRocketsPool.Add(erocket);
            }
            if (type == "ESpaceShip")
                enemyFleet.Add(tmp_rocket.GetComponent<ShipController>());

            tmp_rocket.SetActive(false);
        }
        Destroy(rocket);
    }
    
    public ThreeBezierScript GetRocketFromPool(RocketLauncher.Mode mode)
    {
        if (CanGetRocket(mode)) 
        {
            ThreeBezierScript rocketTmp = playerRocketsPool[0];
            playerRocketsPool.Remove(rocketTmp);
            rocketTmp.gameObject.transform.position = spawnPosition;
            rocketTmp.gameObject.SetActive(true);
            rocketTmp.gameObject.transform.parent = null;
            if (mode == RocketLauncher.Mode.armageddon || mode == RocketLauncher.Mode.rocketGuidance)
            {
                Animator animator = rocketTmp.GetComponent<Animator>();
                animator.runtimeAnimatorController = Resources.Load("Animation/Rockets/Fire2") as RuntimeAnimatorController;
            }
            UIMenu.Instance.SetRocketsAmount();
            return rocketTmp;
        }
        return null;
    }
    private bool CanGetRocket(RocketLauncher.Mode mode)
    {
        if (mode == RocketLauncher.Mode.armageddon || mode == RocketLauncher.Mode.autoGun)
            return true;

        if (mode == RocketLauncher.Mode.rocketGuidance && (LevelInfo.Instance.amountOfSelfGuidedRockets > 0))
        {
            LevelInfo.Instance.amountOfSelfGuidedRockets--;
            return true;
        }
        if ((mode == RocketLauncher.Mode.manualAiming || mode == RocketLauncher.Mode.tapLaunch) && LevelInfo.Instance.amountOfPlayerRockets > 0)
        {
            //amountOfPlayerRockets--;
            return true;
        }
        return false;
    }

    public void RocketBackToPool(ThreeBezierScript rocket)
    {
        rocket.gameObject.transform.SetParent(gameObjectsStorage.transform);
        playerRocketsPool.Add(rocket);
        rocket.gameObject.SetActive(false);
        rocket.gameObject.transform.position = rocket.P0;
    }       
   
    public ShipController GetESpaceshipFromPool()
    {
        if (enemyFleet.Count == 0)
            return null;
        ShipController shipTmp = enemyFleet[0];
        enemyFleet.Remove(shipTmp);
        shipTmp.gameObject.transform.position = spawnPosition;
        shipTmp.gameObject.transform.parent = null;
        shipTmp.gameObject.SetActive(true);
        return shipTmp;
    }

    public void ESpaceshipBackToPool(ShipController ship)
    {
        ship.gameObject.transform.SetParent(gameObjectsStorage.transform);
        enemyFleet.Add(ship);
        ship.gameObject.SetActive(false);
        EnemyDefender.Instance.shipCounter--;
        EnemyDefender.Instance.destroyedShipsCounter++;
        if (EnemyDefender.Instance.destroyedShipsCounter >= LevelInfo.Instance.enemyFleetSize)
            LevelInfo.Instance.LevelIsCompleted();
    }

    public ThreeBezierScript GetEnemyRocketFromPool()
    {
        if (enemyRocketsPool.Count > 0)
        {
            ThreeBezierScript rocketTmp = enemyRocketsPool[0];
            enemyRocketsPool.Remove(rocketTmp);
            rocketTmp.gameObject.transform.parent = null;
            rocketTmp.gameObject.transform.position = spawnPosition;
            rocketTmp.gameObject.gameObject.SetActive(true);
            rocketTmp.isDrawn = false;
            currEnemyRockets.Add(rocketTmp);
            LevelInfo.Instance.launchedERockets++;
            return rocketTmp;
        }
        return null;
    }
    
    public void EnemyRocketBackToPool(ThreeBezierScript enemyRocket, bool isShotDown = true)
    {
        if (currEnemyRockets.Contains(enemyRocket))       
            currEnemyRockets.Remove(enemyRocket);
        if (isShotDown)
        {
            LevelInfo.Instance.score += (int)(100 * (1 - enemyRocket.T));
            UIMenu.Instance.ResetLvlInfo();
            LevelInfo.Instance.amountOfERocketsOnLevel--;
        }

        enemyRocket.gameObject.transform.SetParent(gameObjectsStorage.transform);
        enemyRocketsPool.Add(enemyRocket);
        enemyRocket.gameObject.SetActive(false);
        enemyRocket.gameObject.transform.position = enemyRocket.P0;
        if (currEnemyRockets.Count == 0)
            LevelInfo.Instance.launchedERockets = 0;
    }

    public bool EnemyCanShoot()
    {
        return LevelInfo.Instance.launchedERockets < LevelInfo.Instance.eRocketsToLaunch &&
            LevelInfo.Instance.amountOfERocketsOnLevel - LevelInfo.Instance.launchedERockets > 0;
    }

    public void Explose(Vector3 coord)
    {
        Instantiate(explosion, coord, Quaternion.identity);
    }
    #endregion
   
}