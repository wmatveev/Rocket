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
    //offset is used to draw aim line straight up for a sertain distances
    [SerializeField] private float offset = 3f;
    private float currentOffset = 0f;
    //clickOffset is used to draw aim line straight down independently of start tap position
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
        tapLaunch,
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
        if (currentMode == Mode.tapLaunch)
        {
            clickPoint = Camera.main.ScreenToWorldPoint(eventData.position);
        }
        if (currentMode == Mode.manualAiming)
        {
            clickPoint = launchPoint.position;
            clickPoint.y -= currentOffset;
            clickOffset = Camera.main.ScreenToWorldPoint(eventData.position);
            clickOffset = new Vector2(clickOffset.x - clickPoint.x, clickOffset.y - clickPoint.y);
            OnDrag(eventData);
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (currentMode == Mode.manualAiming)
        {
            clickPoint = Camera.main.ScreenToWorldPoint(eventData.position);
            clickPoint = new Vector2(clickPoint.x - clickOffset.x, clickPoint.y - clickOffset.y);
            Ray ray = new Ray(launchPoint.position, new Vector2(launchPoint.position.x, launchPoint.position.y) - clickPoint);
            Vector2 destination = ray.GetPoint(offset);
            lineRenderer.SetPosition(0, new Vector3(clickPoint.x, clickPoint.y, 2));
            lineRenderer.SetPosition(1, new Vector3(destination.x, destination.y, 2));
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (currentMode == Mode.manualAiming || currentMode == Mode.tapLaunch)
            RocketLaunch();

        lineRenderer.SetPosition(0, new Vector3(0, 0, 2));
        lineRenderer.SetPosition(1, new Vector3(0, 0, 2));
        currentOffset = 0f;
        clickPoint = launchPoint.position;
    }

    private void RocketLaunch()
    {
        ThreeBezierScript bezier = GameManager.Instance.GetRocketFromPool(currentMode);

        if (currentMode == Mode.none)
            Destroy(gameObject);
        if (currentMode == Mode.rocketGuidance || currentMode == Mode.armageddon)
        {
            if (GameManager.Instance.currEnemyRockets.Count > 0)
            {
                ThreeBezierScript target = GameManager.Instance.currEnemyRockets[armageddonCounter];
                bezier.SetPoints(target.P2, target.P1, target.P0);
                //GameManager.Instance.currEnemyRockets.RemoveAt(0);
                amountOfPlayerRockets.text = GameManager.Instance.amountOfPlayerRockets.ToString();
            }
            else GameManager.Instance.RocketBackToPool(bezier);
        }
        if (currentMode == Mode.manualAiming || currentMode == Mode.tapLaunch)
        {
            SetPoints(bezier);
        }
    }

    private void SetPoints(ThreeBezierScript bezier)
    {
        bezier.SetPointP1(new Vector3());
        if (currentMode == Mode.manualAiming)
        {
            Ray ray = new Ray(launchPoint.position, new Vector2(launchPoint.position.x, launchPoint.position.y) - clickPoint);
            bezier.SetPoints(launchPoint.position, ray.GetPoint(currentOffset), ray.GetPoint(currentOffset * 100));
        }
        if (currentMode == Mode.tapLaunch)
        {
            Ray ray = new Ray(launchPoint.position, clickPoint - new Vector2(launchPoint.position.x, launchPoint.position.y));
            bezier.SetPoints(launchPoint.position, clickPoint, ray.GetPoint(currentOffset * 100)); 
        }
    }

    public void changeModeToManualAiming()
    {
        currentMode = Mode.manualAiming;
    }

    public void changeModeToTapLaunch()
    {
        currentMode = Mode.tapLaunch;
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
}