using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIArcadeMenu : MonoBehaviour
{

    #region Singleton
    public static UIArcadeMenu Instance { get; private set; }
    #endregion

    [SerializeField] private Text lvlInfoLable;

    [SerializeField] private GameObject armaButton;
    private List<Image> armaEffects = new List<Image>();

    [SerializeField] private Text amountOfSelfGuidedRockets;
    [SerializeField] private Text amountOfPlayerRockets;

    public GameObject endLvlFade;
    [HideInInspector] public GameObject losePanel, winPanel;
    public GameObject attackSceneTransition;
    private void Start()
    {
        Instance = this;

        losePanel = GameObject.Find("losePanel");
        winPanel = GameObject.Find("winPanel");
        if (armaButton)
            for (int i = 0; i < armaButton.transform.childCount; i++)
            {
                Image tmp = armaButton.transform.GetChild(i).gameObject.GetComponent<Image>();
                if (tmp)
                {
                    armaEffects.Add(tmp);
                    tmp.gameObject.SetActive(false);
                }
            }

        if (losePanel) losePanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
        if (attackSceneTransition)
            attackSceneTransition.SetActive(false);
        if (endLvlFade) endLvlFade.SetActive(false);

        ResetLvlInfo();

        SetRocketsAmount();
    }

    public void ResetLvlInfo()
    {
        lvlInfoLable.text = "Level " + LevelInfo.Instance.currentLevel + "\nScore: " + LevelInfo.Instance.score;
    }

    public void SetRocketsAmount()
    {
        if (amountOfSelfGuidedRockets)
            amountOfSelfGuidedRockets.text = LevelInfo.Instance.amountOfSelfGuidedRockets.ToString();
        //amountOfPlayerRockets.text = GameManager.Instance.amountOfPlayerRockets.ToString();

    }

    public void NextLevel()
    {
        PlayerPrefs.SetInt("currentLevel", LevelInfo.Instance.currentLevel + 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ResetSave()
    {
        PlayerPrefs.DeleteAll(); 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void armaLightUp()
    {
        StartCoroutine(LightIpUp());
    }

    [SerializeField] private float deltaAlpha = 1f;
    IEnumerator LightIpUp()
    {
        Color color = new Color();

        foreach (var image in armaEffects)
        {
            image.gameObject.SetActive(true);
            color = image.color;
            color.a = 0;
            image.color = color;
        }
        float alpha = color.a;
        while (color.a <= 1)
        {
            alpha += deltaAlpha * Time.deltaTime;
            foreach (var image in armaEffects)
            {
                color = image.color; 
                color.a = alpha;
                image.color = color;
            }
            yield return null;
        }
        while (color.a >= 0)
        {
            alpha -= deltaAlpha * Time.deltaTime;
            foreach (var image in armaEffects)
            {
                color = image.color;
                color.a = alpha;
                image.color = color;
            }
            yield return null;
        }
    }
}
