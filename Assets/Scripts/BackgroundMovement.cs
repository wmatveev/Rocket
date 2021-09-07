using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BackgroundMovement : MonoBehaviour
{
    public float speed;
    public float timeToWin;
    private float time = 0f;
    private Vector2 offset = Vector2.zero;
    private Material material;
    private void Start()
    {
        var height = Camera.main.orthographicSize * 2f;
        var width = height * Screen.width / Screen.height;
        transform.localScale = new Vector3(width, height, 0);

        material = GetComponent<Renderer>().material;
        offset = material.GetTextureOffset("_MainTex");

        timeToWin += PlayerPrefs.GetInt("currentLevel");
    }

    private void FixedUpdate()
    {
        time += Time.deltaTime;
        offset.y += speed * Time.deltaTime;
        material.SetTextureOffset("_MainTex", offset);
        if (SceneManager.GetActiveScene().name == "ArcadeMode" && time > timeToWin)
            LevelInfo.Instance.ArcadeModeCompleted();
        if (GameManager.Instance.enemyPlanet && GameManager.Instance.enemyPlanet.gameObject.transform.localScale.x >= 2f)
            Destroy(this);
    }
}