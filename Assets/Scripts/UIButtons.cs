using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIButtons : MonoBehaviour
{
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
