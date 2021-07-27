using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{

    #region Singleton
    public static UIMenu Instance { get; private set; }
    #endregion

    [SerializeField] private Text levelLable;
    [SerializeField] private Text scoreLable;

    [SerializeField] private Text amountOfSelfGuidedRockets;
    [SerializeField] private Text amountOfPlayerRockets;
    private void Start()
    {
        Instance = this;

        levelLable.text += GameManager.Instance.currentLevel;
        scoreLable.text += GameManager.Instance.score;

        SetRocketsAmount();
    }

    public void SetScore()
    {
        scoreLable.text = "Score: " + GameManager.Instance.score;
    }

    public void SetRocketsAmount()
    {
        amountOfSelfGuidedRockets.text = GameManager.Instance.amountOfSelfGuidedRockets.ToString();
        amountOfPlayerRockets.text = GameManager.Instance.amountOfPlayerRockets.ToString();

    }
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void NextLevel()
    {
        PlayerPrefs.SetInt("currentLevel", GameManager.Instance.currentLevel + 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ResetSave()
    {
        PlayerPrefs.DeleteAll(); 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
