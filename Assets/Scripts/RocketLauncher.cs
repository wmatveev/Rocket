using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RocketLauncher : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Text amountOfPlayerRockets;
    [SerializeField] private Transform launchPoint;
    
    #region Singleton
    public static RocketLauncher Instance { get; private set; }
    #endregion
    
    [SerializeField] private float offset = 2f;
    private float currentOffset = 0f;
    private Vector2 clickOffset = new Vector2(), 
                    clickPoint = new Vector2();

    private LineRenderer lineRenderer;
    private int armageddonCounter = 0;
    public enum Mode
    {
        none,
        loop,
        rocketGuidance,
        manualAiming,
        enemyAI,
        armageddon
    }

    private Mode currentMode;

    void Start()
    {
        Instance = this;

        lineRenderer = GetComponent<LineRenderer>();

        amountOfPlayerRockets.text = GameManager.Instance.amountOfPlayerRockets.ToString();
        launchPoint = GameManager.Instance.homePlanet.transform;
        clickPoint = launchPoint.position;
        lineRenderer.SetPosition(0, new Vector3(0, 0, 2));
        lineRenderer.SetPosition(1, new Vector3(0, 0, 2));
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        currentOffset = offset;
        clickPoint = launchPoint.position;
        clickPoint.y -= currentOffset;
        clickOffset = Camera.main.ScreenToWorldPoint(eventData.position);
        clickOffset = new Vector2(clickOffset.x - clickPoint.x, clickOffset.y - clickPoint.y);

        OnDrag(eventData);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        clickPoint = Camera.main.ScreenToWorldPoint(eventData.position);
        clickPoint = new Vector2(clickPoint.x - clickOffset.x, clickPoint.y - clickOffset.y);

        Ray ray = new Ray(launchPoint.position, new Vector2(launchPoint.position.x, launchPoint.position.y) - clickPoint);
        Vector2 destination = ray.GetPoint(offset);
        if (currentMode == Mode.manualAiming)
        {
            lineRenderer.SetPosition(0, new Vector3(clickPoint.x, clickPoint.y, 2));
            lineRenderer.SetPosition(1, new Vector3(destination.x, destination.y, 2));
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (currentMode == Mode.manualAiming)
            RocketLaunch();
        currentOffset = 0f;
        clickPoint = launchPoint.position;
        lineRenderer.SetPosition(0, new Vector3(0, 0, 2));
        lineRenderer.SetPosition(1, new Vector3(0, 0, 2));
    }

    private void RocketLaunch()
    {
        KeyValuePair<GameObject, ThreeBezierScript> rocketInfo = GameManager.Instance.GetRocketFromPool(currentMode);
        GameObject rocket = rocketInfo.Key;
        ThreeBezierScript bezier = rocketInfo.Value;

        if (currentMode == Mode.none)            
            currentMode = bezier.currentMode;        
        else bezier.currentMode = currentMode;
        
        if (currentMode == Mode.rocketGuidance || currentMode == Mode.armageddon)
        {
            if (GameManager.Instance.currEnemyRockets.Count > 0)
            {
                ThreeBezierScript target = GameManager.Instance.currEnemyRockets[armageddonCounter].Value;
                bezier.SetPoints(target.P2, target.P1, target.P0);
                //GameManager.Instance.currEnemyRockets.RemoveAt(0);
                amountOfPlayerRockets.text = GameManager.Instance.amountOfPlayerRockets.ToString();
            }
            else GameManager.Instance.RocketBackToPool(rocketInfo);
            //GameObject P2 = GameManager.Instance.currEnemyRockets.Count > 0 ? GameManager.Instance.currEnemyRockets[0].Key 
            //: GameManager.Instance.enemyPlanet;
            //bezier.SetPointP2(P2.transform);
        }
        if (currentMode == Mode.manualAiming)
        {
            SetP1(bezier);
        }
    }

    private void SetP1(ThreeBezierScript bezier)
    {
        GameObject P1 = new GameObject();
        bezier.SetPointP1(P1.transform.position);
        if (currentMode == Mode.manualAiming)
        {
            Ray ray = new Ray(launchPoint.position, new Vector2(launchPoint.position.x, launchPoint.position.y) - clickPoint);
            //bezier.SetPointP1(ray.GetPoint(currentOffset));

            //GameObject P2 = new GameObject();
            //P2.transform.position = ray.GetPoint(currentOffset * 10);
            //bezier.SetPointP2(P2.transform.position);

            bezier.SetPoints(launchPoint.position, ray.GetPoint(currentOffset), ray.GetPoint(currentOffset * 10));
        }

        //if (currentMode == Mode.rocketGuidance)
        //{
        //    ThreeBezierScript enemyBezier = new ThreeBezierScript();
        //    try
        //    {
        //        enemyBezier = bezier.target.GetComponent<ThreeBezierScript>();
        //    if (enemyBezier.T >= 2f/3f)
        //    {
        //        Ray ray = new Ray(launchPoint.position, new Vector2(bezier.P0.position.x - launchPoint.position.x, bezier.P0.position.y - launchPoint.position.y));
        //        float distanceY = bezier.P0.position.y - launchPoint.position.y;
        //        bezier.P1.position = ray.GetPoint(distanceY / 2);
        //    }
        //    }
        //    catch
        //    {//
        //        Debug.Log("No tbs");
        //    }

        //    if (enemyBezier == null)
        //    {
        //        bezier.P1 = P1.transform; 
        //        bezier.RandomP1(0.5f, 3);
        //        if (bezier.P1.position.y < 0) bezier.P1.position = new Vector2(bezier.P1.position.x, Mathf.Abs(bezier.P1.position.y));
        //    }

        //   amountOfPlayerRockets.text = GameManager.Instance.amountOfPlayerRockets.ToString();
        //}
    }

    public void changeModeToManualAiming()
    {
        currentMode = Mode.manualAiming;
    }

    public void launchGuidanceRocket()
    {
        currentMode = Mode.rocketGuidance;
        RocketLaunch();
    }

    public void launchArmageddon()
    {
        currentMode = Mode.armageddon;
        int countOfTargets = GameManager.Instance.currEnemyRockets.Count;
        Debug.Log(countOfTargets);
        for (int i = 0; i < countOfTargets; i++)
        {
            RocketLaunch();
            armageddonCounter++;
        }
        armageddonCounter = 0;
    }

    //private void OnDrawGizmos()
    //{
    //    Ray ray = new Ray(launchPoint.position, new Vector2(launchPoint.position.x, launchPoint.position.y) - clickPoint);
    //    Vector2 destination = ray.GetPoint(offset);
    //    Gizmos.color = Color.cyan;
    //    Gizmos.DrawLine(clickPoint, destination);
    //    Gizmos.DrawSphere(launchPoint.position, 0.1f);
    //}
}