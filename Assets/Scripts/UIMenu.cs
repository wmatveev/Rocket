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

    [SerializeField] private Text lvlInfoLable;

    [SerializeField] private GameObject armaButton;
    private List<Image> armaEffects = new List<Image>();

    [SerializeField] private Text amountOfSelfGuidedRockets;
    [SerializeField] private Text amountOfPlayerRockets;

    public GameObject endLvlFade;
    [HideInInspector] public GameObject losePanel, winPanel;
    private void Start()
    {
        Instance = this;


        losePanel = GameObject.Find("losePanel");
        winPanel = GameObject.Find("winPanel");
        for (int i = 0; i < armaButton.transform.childCount; i++)
        {
            Image tmp = armaButton.transform.GetChild(i).gameObject.GetComponent<Image>(); 
            if (tmp)
            {
                armaEffects.Add(tmp);
                tmp.gameObject.SetActive(false);
            }
        }
        
        losePanel.SetActive(false);
        winPanel.SetActive(false);
        endLvlFade.SetActive(false);

        ResetLvlInfo();

        SetRocketsAmount();
    }

    public void ResetLvlInfo()
    {
        lvlInfoLable.text = "Level " + GameManager.Instance.currentLevel + "\nScore: " + GameManager.Instance.score;
    }

    public void SetRocketsAmount()
    {
        amountOfSelfGuidedRockets.text = GameManager.Instance.amountOfSelfGuidedRockets.ToString();
        //amountOfPlayerRockets.text = GameManager.Instance.amountOfPlayerRockets.ToString();

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
