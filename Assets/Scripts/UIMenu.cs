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

    [SerializeField] public Text levelLable;
    [SerializeField] public Text scoreLable;
    private void Start()
    {
        Instance = this;

        levelLable.text += GameManager.Instance.currentLevel;
        scoreLable.text += GameManager.Instance.score;
    }

    public void SetScore()
    {
        scoreLable.text = "Score: " + GameManager.Instance.score;
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
}
