using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

public class UIMainMenu : MonoBehaviour
{
    private void Start()
    {
        Time.timeScale = 1;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1); 
    }
    
    public void ResetSave()
    {
        PlayerPrefs.DeleteAll();
    }

    public void ExitApp()
    {
        Application.Quit();
    }
}
