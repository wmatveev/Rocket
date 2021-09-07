using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMapMenu : MonoBehaviour
{
    public static UIMapMenu Instance { get; private set; }
    public GameObject fogMenu;

    private void Awake()
    {
        Instance = this;
    }

    public void LoadHomePlanetScene()
    {
        SceneManager.LoadScene("PlanetDefence");
    }

    public void LoadAttackScene(int levelIndex)
    {
        PlayerPrefs.SetInt("currentLevel", levelIndex);
        SceneManager.LoadScene("ArcadeModeNew");
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
