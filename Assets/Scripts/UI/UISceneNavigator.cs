using UnityEngine;
using UnityEngine.SceneManagement;

public class UISceneNavigator : MonoBehaviour
{
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void GoToWorldMap()
    {
        SceneManager.LoadScene(1);
    }   
    
    public void LoadArcadeMode()
    {
        SceneManager.LoadScene(2);
    }
    public void LoadAttackScene(int levelIndex)
    {
        PlayerPrefs.SetInt("currentLevel", levelIndex);
        SceneManager.LoadScene("ArcadeModeNew");
    }

    //public void LoadHomePlanetScene()
    //{
    //    SceneManager.LoadScene("PlanetDefence");
    //}
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void LoadAttackScene()
    {
        SceneManager.LoadScene("PlanetAttack");
    }
}
