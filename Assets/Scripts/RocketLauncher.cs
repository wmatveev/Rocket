using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RocketLauncher : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    #region Singleton
    public static RocketLauncher Instance { get; private set; }
    #endregion
    //offset is used to draw aim line straight up for a sertain distances
    [SerializeField] private float offset = 3f;
    private float defaultOffset = 0f;
    //clickOffset is used to draw aim line straight down independently of start tap position
    private Vector3 clickOffset = new Vector2(),
                    clickPoint = new Vector2();
    private Transform launchPoint;
    private bool isDraged = false;
    private LineRenderer lineRenderer;
    [HideInInspector] public int rocketGuidedRCounter = 0;
    [SerializeField] private GameObject aim;

    //for autoGun movement
    [SerializeField] private GameObject autoGun;
    GraphicRaycaster raycaster;
    List<RaycastResult> results = new List<RaycastResult>();
    public enum Mode
    {
        none,
        loop,
        rocketGuidance,
        manualAiming,
        tapLaunch,
        enemyAI,
        armageddon,
        autoGun
    }

    private Mode currentMode, lastAimMode;

    void Start()
    {
        Instance = this;

        lineRenderer = GetComponent<LineRenderer>();
        raycaster = GetComponent<GraphicRaycaster>();

        currentMode = Mode.tapLaunch;
        lastAimMode = currentMode;

        defaultOffset = offset;
        launchPoint = GameManager.Instance.homePlanet.transform;
        clickPoint = launchPoint.position;
        lineRenderer.SetPosition(0, new Vector3(0, 0, 2));
        lineRenderer.SetPosition(1, new Vector3(0, 0, 2));

        aim.SetActive(false);
        autoGun.transform.position = new Vector3(autoGun.transform.position.x, autoGun.transform.position.y, launchPoint.position.z);
    }

    private float cooldown = 0f, timeToReload = 0.5f;
    void FixedUpdate()
    {
        cooldown += Time.deltaTime;
        //if (cooldown > timeToReload)
        //{
        //    cooldown = 0f;
        //    //AutoGunRocketLaunch();
        //}
    }

    private bool CanShoot()
    {
        return cooldown >= timeToReload;
    }
    public virtual void OnPointerDown(PointerEventData eventData)
    { 
        //This code was used to move auto gun
        //results.Clear();
        //raycaster.Raycast(eventData, results);
        //foreach (RaycastResult result in results)
        //{
        //    if (result.gameObject.transform.tag == "AutoGun")
        //    {
        //        isDraged = true;
        //        OnDrag(eventData);
        //        break;
        //    }
        //}

        if (currentMode == Mode.tapLaunch)
        {
            aim.SetActive(true);
            //OnDrag(eventData);
        }
        if (currentMode == Mode.manualAiming)
            OnDrag(eventData);
    }
    public virtual void OnDrag(PointerEventData eventData)
    {
        if (isDraged == true)
        {
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(eventData.position);
            newPosition = new Vector2(newPosition.x, autoGun.transform.position.y);
            autoGun.transform.position = newPosition;
        }

        if (currentMode == Mode.manualAiming)
        {
            clickPoint = Camera.main.ScreenToWorldPoint(eventData.position);
            clickPoint = new Vector2(clickPoint.x - clickOffset.x, clickPoint.y - clickOffset.y);
            //Ray ray = new Ray(launchPoint.position, new Vector2(launchPoint.position.x, launchPoint.position.y) - clickPoint);
            Vector2 destination = new Vector2();            
            Ray ray = new Ray(launchPoint.position, clickPoint - launchPoint.position);
            destination = ray.GetPoint(Vector3.Distance(launchPoint.position, clickPoint) + offset);
            //lineRenderer.SetPosition(0, new Vector3(clickPoint.x, clickPoint.y, 2));
            //lineRenderer.SetPosition(1, new Vector3(destination.x, destination.y, 2));
            lineRenderer.SetPosition(0, new Vector3(launchPoint.position.x, launchPoint.position.y, 2));
            lineRenderer.SetPosition(1, new Vector3(destination.x, destination.y, 2));
        }

        if (currentMode == Mode.tapLaunch)
        {
            //Vector3 aimPosition = Camera.main.ScreenToWorldPoint(eventData.position);
            Ray ray = Camera.main.ScreenPointToRay(eventData.position);
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
            Vector3 aimPosition = ray.GetPoint(10);
            aimPosition.y += offset;
            clickPoint = aimPosition;
            aimPosition.z = launchPoint.position.z - 2;
            aim.transform.position = aimPosition;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (CanShoot() && (currentMode == Mode.manualAiming || currentMode == Mode.tapLaunch))
            RocketLaunch();

        lineRenderer.SetPosition(0, new Vector3(0, 0, 2));
        lineRenderer.SetPosition(1, new Vector3(0, 0, 2));
        clickPoint = launchPoint.position;
        isDraged = false;
        aim.SetActive(false);
    }

    private void RocketLaunch()
    {
        ThreeBezierScript bezier = GameManager.Instance.GetRocketFromPool(currentMode);
        bezier.currentMode = currentMode;

        if (currentMode == Mode.rocketGuidance || currentMode == Mode.armageddon)
        {
            if (GameManager.Instance.currEnemyRockets.Count > rocketGuidedRCounter)
            {
                ThreeBezierScript target = GameManager.Instance.currEnemyRockets[rocketGuidedRCounter];
                Vector3 startPos = GameManager.Instance.homePlanet.transform.position;
                Ray ray = new Ray(startPos, target.gameObject.transform.position - startPos);
                bezier.SetPoints(startPos,
                                 ray.GetPoint(Vector3.Distance(startPos, target.gameObject.transform.position) / 2),
                                 target.gameObject.transform);
            }
            else GameManager.Instance.RocketBackToPool(bezier);
        }
        if (currentMode == Mode.manualAiming || currentMode == Mode.tapLaunch)
        {
            SetPoints(bezier);
        }
        cooldown = 0f;
    }

    
    private void SetPoints(ThreeBezierScript bezier)
    {
        bezier.SetPointP1(new Vector3());
        if (currentMode == Mode.manualAiming)
        {
            Ray ray = new Ray(launchPoint.position, clickPoint - launchPoint.position);//new Vector2(launchPoint.position.x, launchPoint.position.y) - clickPoint);
            Vector2 P3 = new Vector2();
            if (offset == 0)
                P3 = ray.GetPoint(300);
            else
                P3 = ray.GetPoint(offset * 100);

            bezier.SetPoints(launchPoint.position, clickPoint, P3);
        }
        if (currentMode == Mode.tapLaunch)
        {
            //Ray ray = new Ray(launchPoint.position, clickPoint - new Vector2(launchPoint.position.x, launchPoint.position.y));
            Vector3 clickPoint = new Vector3(this.clickPoint.x, this.clickPoint.y, launchPoint.transform.position.z);
            bezier.SetPoints(launchPoint.position, clickPoint, clickPoint);
        }
    }
    
    private void AutoGunRocketLaunch()
    {
        ThreeBezierScript bezier = GameManager.Instance.GetRocketFromPool(currentMode);
        bezier.currentMode = Mode.autoGun;
        Vector3 rayUp = new Vector3(autoGun.transform.position.x, autoGun.transform.position.y + offset, 0);
        Ray ray = new Ray(autoGun.transform.position, rayUp - autoGun.transform.position);
        bezier.SetPoints(autoGun.transform.position, ray.GetPoint(offset * 10), ray.GetPoint(offset * 100));
    }

    public void changeAimMode()
    {
        if (currentMode == Mode.manualAiming)
            currentMode = Mode.tapLaunch;
        else if (currentMode == Mode.tapLaunch)
            currentMode = Mode.manualAiming;
    }
    public void changeModeToManualAiming()
    {
        if (offset == defaultOffset)
        {
            offset = 0f;
        }
        else
            offset = defaultOffset;
        currentMode = Mode.manualAiming;
    }

    public void changeModeToTapLaunch()
    {
        offset = defaultOffset;
        currentMode = Mode.tapLaunch;
    }

    public void launchGuidanceRocket()
    {
        lastAimMode = currentMode;
        currentMode = Mode.rocketGuidance;
        RocketLaunch();
        rocketGuidedRCounter++;
        currentMode = lastAimMode;
    }

    public void launchArmageddon()
    {
        lastAimMode = currentMode;
        currentMode = Mode.armageddon;
        int countOfTargets = GameManager.Instance.currEnemyRockets.Count;
        for (int i = 0; i < countOfTargets; i++)
        {
            RocketLaunch();
            rocketGuidedRCounter++;
        }
        rocketGuidedRCounter = 0;
        currentMode = lastAimMode;
    }
}