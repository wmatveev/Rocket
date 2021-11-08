using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Analytics;


/// <summary>
/// This class stores info about all level settings
/// </summary>
 
public class LevelInfo : MonoBehaviour
{
    public static LevelInfo Instance { get; private set; }

    [HideInInspector] public float winrate, speedCoef = 1f;

    public int amountOfSelfGuidedRockets;
    public int eRocketsToLose;
    [HideInInspector]
    public int amountOfERocketsOnLevel = 0, //how many rockets we need to destroy to win
        eRocketsToLaunch = 0, //allowed number enemy rockets at the same time on level
        launchedERockets = 0, //counter of launched rockets
        amountOfPlayerRockets,
        enemyFleetSize, //amount of enemy ships flying off enemy planet (planet attack mode only)
        eShipOnDefence; //allowed number enemy ships at the same time on level

    [HideInInspector] public int currentLevel;
    [HideInInspector] public int catchedERockets = 0;
    [HideInInspector] public int score = 0;

    void Awake()
    {
        Instance = this;

        LevelInfo.Instance.SetCurrentLevel();
        LevelInfo.Instance.SetLevelSettings();
    }

    public void SetEndLevelInfo(float winRate)
    {
        this.winrate = winRate;
    }
   
    public void SetCurrentLevel()
    {
        if (PlayerPrefs.HasKey("currentLevel"))
            currentLevel = PlayerPrefs.GetInt("currentLevel");
        else
        {
            currentLevel = 1;
            PlayerPrefs.SetInt("currentLevel", 1);
        }
    }
    public void SetLevelSettings()
    {
        if (SceneManager.GetActiveScene().name == "PlanetDefence")
            SetPlanetDefenceDifficulty();
        if (SceneManager.GetActiveScene().name == "PlanetAttack")
            SetPlanetAttackDifficulty();
        if (SceneManager.GetActiveScene().name == "ArcadeMode")
            SetArcadeModeDifficulty();           
        if (SceneManager.GetActiveScene().name == "ArcadeAndAttack" || SceneManager.GetActiveScene().name == "ArcadeModeNew")
            SetArcadeAndAttackDifficulty();            
    }

    private void SetPlanetDefenceDifficulty()
    {
        amountOfERocketsOnLevel = 3 * 4 + currentLevel;
        eRocketsToLaunch = (currentLevel / 5) + 4;

        amountOfPlayerRockets = (int)(amountOfERocketsOnLevel * 1.2f) + 10;
        speedCoef = 1f + (currentLevel % 5) / 10f;
        EnemyAttacker.Instance.timeToReload = (float)(1f + UnityEngine.Random.value);//(currentLevel < 5) ? 0 : (float)(1f + Random.value);
        //RegulateDifficultyLevel();
    }

    private void SetPlanetAttackDifficulty()
    {
        eShipOnDefence = 4;
        enemyFleetSize = 2 * 3 + currentLevel;

        amountOfERocketsOnLevel = 20 * eShipOnDefence; 
        eRocketsToLaunch = 100;

        amountOfPlayerRockets = 50;
    }

    private void SetArcadeModeDifficulty()
    {
        amountOfPlayerRockets = 40;
    } 
    
    private void SetArcadeAndAttackDifficulty()
    {
        eShipOnDefence = 4 + currentLevel;
        enemyFleetSize = 2 * 3 + currentLevel;

        amountOfERocketsOnLevel = 20 * eShipOnDefence;
        eRocketsToLaunch = 100;

        amountOfPlayerRockets = 100;
    }

    private void RegulateDifficultyLevel()
    {
        if (PlayerPrefs.HasKey("WinStreak"))
        {
            if (PlayerPrefs.GetInt("WinStreak") > 3)
            {
                int strike = PlayerPrefs.GetInt("WinStreak");
                eRocketsToLaunch += strike / 3;
                speedCoef *= (1f + (float)(strike) % 10);
                if (EnemyAttacker.Instance)
                    EnemyAttacker.Instance.timeToReload *= (1f - (float)strike % 50);
            }
        }
    }

    #region LevelEndInfo

    public void ArcadeModeCompleted()
    {
        UIArcadeMenu.Instance.endLvlFade.SetActive(true);
        Time.timeScale = 0;
        UIArcadeMenu.Instance.attackSceneTransition.SetActive(true);
    }

    public void LevelIsCompleted()
    {
        UIArcadeMenu.Instance.endLvlFade.SetActive(true);
        Time.timeScale = 0;
        AnalyticsManager.Instance.EndlevelAnalytics();
        Text text = UIArcadeMenu.Instance.winPanel.transform.Find("Score").gameObject.GetComponent<Text>();
        text.text = "Score: " + score;
        SetWinStreak(1);
        UIArcadeMenu.Instance.winPanel.SetActive(true);
    }

    public void LevelIsLosed()
    {
        catchedERockets++;
        if (catchedERockets >= eRocketsToLose)
        {
            UIArcadeMenu.Instance.endLvlFade.SetActive(true);
            if (PlayerPrefs.HasKey("LosingOnThisLevel"))
                PlayerPrefs.SetInt("LosingOnThisLevel", PlayerPrefs.GetInt("LosingOnThisLevel") + 1);
            else
                PlayerPrefs.SetInt("LosingOnThisLevel", 1);
            SetWinStreak(-1);
            Time.timeScale = 0;
            UIArcadeMenu.Instance.losePanel.SetActive(true);
        }
    }

    private void SetWinStreak(int i = 1)
    {
        if (PlayerPrefs.HasKey("WinStreak"))
            PlayerPrefs.SetInt("WinStreak", PlayerPrefs.GetInt("WinStreak") + i);
        else
            PlayerPrefs.SetInt("WinStreak", 1);
    }
#endregion
}
