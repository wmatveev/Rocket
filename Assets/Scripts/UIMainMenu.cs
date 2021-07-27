using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{
    private void Start()
    {
        Time.timeScale = 1;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("FreeBezier");
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
