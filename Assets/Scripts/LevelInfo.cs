using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;


/// <summary>
/// This class is experimental. It stores info for Unity analytics
/// </summary>
public class LevelInfo : MonoBehaviour
{
    public static LevelInfo Instance { get; private set; }
    public int currentLevel;
    public float winrate;
    public int score;
    void Start()
    {
        Instance = this;
        currentLevel = GameManager.Instance.currentLevel;
    }
    public void SetEndLevelInfo(float winRate)
    {
        score = GameManager.Instance.score;
        this.winrate = winRate;
    }

}
