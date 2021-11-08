using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMapMenu : MonoBehaviour
{
    public static UIMapMenu Instance { get; private set; }
    [HideInInspector] public GameObject fogMenu;
    public GameObject playerPlanet_GO;
    [HideInInspector] public GameObject tmpFog, tmpButton;
    [HideInInspector] public bool followPlayerPlanet = false;
    [HideInInspector] public bool isZooming = false;

    [HideInInspector] public bool isPlanetMode = false;
    [SerializeField] private List<GameObject> planetModeObjects = new List<GameObject>();
    [SerializeField] private List<GameObject> mapModeObjects = new List<GameObject>();
    Planet playerPlanet;
    public struct Planet
    {
        public GameObject planet;
        public EllipticalMovement ellipticalMovement;
        public LineRenderer lineRenderer;
    }

    private void Awake()
    {
        Instance = this;
        if (fogMenu) fogMenu.SetActive(false);
        playerPlanet.planet = playerPlanet_GO;
        playerPlanet.ellipticalMovement = playerPlanet_GO.GetComponent<EllipticalMovement>();
        playerPlanet.lineRenderer = playerPlanet_GO.GetComponent<LineRenderer>();
    }

    public void CallDeleteFogMenu(Vector2 position, GameObject fog, GameObject button)
    {
        fogMenu.transform.position = new Vector2(position.x, position.y + 1);
        fogMenu.SetActive(true);
        tmpFog = fog;
        tmpButton = button;
    }

    public void DeleteCurrentFog()
    {
        Destroy(tmpButton);
        Image image = tmpFog.GetComponent<Image>();
        tmpFog = null;
        fogMenu.SetActive(false);
        StartCoroutine(DestroyAfterFading(image));
    }


    [SerializeField] private float deltaAlpha = 1f;
    IEnumerator DestroyAfterFading(Image image)
    {
        Color color = new Color();
        color = image.color;
        float alpha = color.a;
        while (alpha >= 0)
        {
            alpha -= deltaAlpha * Time.deltaTime;
            color = image.color;
            color.a = alpha;
            image.color = color;
            yield return null;
        }
        Destroy(image.gameObject);
    }

    public void SwitchMode()
    {
        if (isZooming)
            return;
        if (isPlanetMode)
        {
            ZoomBackToMap();
            //переходим в режим карты
            playerPlanet.ellipticalMovement.enabled = true;
            playerPlanet.lineRenderer.enabled = true;
            
            foreach (var go in mapModeObjects) //go - gameObject
                go.SetActive(true);
            foreach (var go in planetModeObjects)
                go.SetActive(false);
            isPlanetMode = false;
        }
        else
        {
            ZoomToHomePlanet();
            //переходим в режим планеты
            playerPlanet.ellipticalMovement.enabled = false;
            playerPlanet.lineRenderer.enabled = false;
            //приближение из режима карты к нашей планете
            //переключаем все объекты
            foreach (var go in mapModeObjects)
                go.SetActive(false);
            foreach (var go in planetModeObjects)
                //включить
                go.SetActive(true);
            isPlanetMode = true;
        }
    }

    public void ZoomToPlanet(float size)
    {
        if (isZooming)
            return;
        StartCoroutine(gradualZoomTo(size));
    }
    [SerializeField] public float cameraSizeOnPlanetMode = 2f, cameraSizeOnMapMode = 5f;
    public void ZoomToHomePlanet()
    {
        if (isZooming)
            return;
        //if (Camera.main.orthographicSize > cameraSizeOnPlanetMode)
        //{
            followPlayerPlanet = true;
            StartCoroutine(gradualZoomTo(cameraSizeOnPlanetMode - 0.1f, playerPlanet_GO.transform.position));
        //}
        //else
        //{
            
        //    StartCoroutine(gradualZoomBack(cameraSizeOnMapMode));
        //}
    }
    
    public void ZoomBackToMap()
    {
        if (isZooming)
            return;
        StartCoroutine(gradualZoomBack(cameraSizeOnMapMode));
    }

    [SerializeField] private float zoomSpeed = 0.3f;
    IEnumerator gradualZoomTo(float size, Vector3 position = new Vector3())
    {
        isZooming = true;
        Vector3 velocity = Vector3.zero;
        float f_velocity = 0;

        Vector3 target = new Vector3(position.x, position.y, 
            Camera.main.transform.position.z);

        float smoothTime = zoomSpeed;
        while (Camera.main.orthographicSize > size)
        {            
            Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, target,
                ref velocity, smoothTime);
            Camera.main.orthographicSize = Mathf.SmoothDamp(Camera.main.orthographicSize, size, ref f_velocity, smoothTime);
            smoothTime *= 1.2f;
            yield return new WaitForFixedUpdate();
        }
        isZooming = false;
    }

    IEnumerator gradualZoomBack(float size)
    {
        followPlayerPlanet = false;
        isZooming = true;
        float f_velocity = 0;
        float smoothTime = zoomSpeed;
        while (Camera.main.orthographicSize < size)
        {
            Camera.main.orthographicSize = Mathf.SmoothDamp(Camera.main.orthographicSize, size, ref f_velocity, smoothTime);
            smoothTime *= 1.2f;
            yield return new WaitForFixedUpdate();
        }
        isZooming = false;
    }
}
