using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RocketLauncher : MonoBehaviour
{
    #region Singleton
    public static RocketLauncher Instance { get; private set; }
    #endregion
    //offset is used to draw aim line straight up for a sertain distances
    [SerializeField] private float offset = 3f;
    private float defaultOffset = 0f;
    //clickOffset is used to draw aim line straight down independently of start tap position
    private Vector2 clickOffset = new Vector2(),
                    clickPoint = new Vector2();
    private Vector3 launchPoint = new Vector3();
    private bool isDraged = false;
    private LineRenderer lineRenderer;
    [HideInInspector] public int rocketGuidedRCounter = 0;
    [SerializeField] private GameObject aim;
    [SerializeField] private ParticleSystem lightning;

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

    public Mode currentMode, lastAimMode;

    void Start()
    {
        Instance = this;

        lineRenderer = GetComponent<LineRenderer>();
        raycaster = GetComponent<GraphicRaycaster>();

        currentMode = Mode.tapLaunch;
        lastAimMode = currentMode;

        defaultOffset = offset;
        launchPoint = GameManager.Instance.homePlanet.transform.position;
        launchPoint.y += 10;
        
        clickPoint = launchPoint;
        lineRenderer.SetPosition(0, new Vector3(0, 0, 2));
        lineRenderer.SetPosition(1, new Vector3(0, 0, 2));

        aim.SetActive(false);
        lightning.Stop();
        autoGun.transform.position = new Vector3(autoGun.transform.position.x, autoGun.transform.position.y, launchPoint.z);
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
    public void _OnPointerDown(PointerEventData eventData)
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
            Vector2 aimPosition = Camera.main.ScreenToWorldPoint(eventData.position);
            aimPosition.y += offset;
            clickPoint = aimPosition;
            aim.transform.position = aimPosition;
            aim.SetActive(true);
            //_OnDrag(eventData);
        }
        //if (currentMode == Mode.manualAiming)
        //    _OnDrag(eventData);
    }

    public virtual void _OnDrag(PointerEventData eventData)
    {
        if (isDraged == true)
        {
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(eventData.position);
            newPosition = new Vector2(newPosition.x, autoGun.transform.position.y);
            autoGun.transform.position = newPosition;
        }

        if (currentMode == Mode.manualAiming) //line aim
        {
            clickPoint = Camera.main.ScreenToWorldPoint(eventData.position);
            clickPoint = new Vector2(clickPoint.x - clickOffset.x, clickPoint.y - clickOffset.y);
            //Ray ray = new Ray(launchPoint, new Vector2(launchPoint.x, launchPoint.y) - clickPoint);
            Vector2 destination = new Vector2();            
            Ray ray = new Ray(launchPoint, clickPoint - new Vector2(launchPoint.x, launchPoint.y));
            destination = ray.GetPoint(Vector3.Distance(launchPoint, clickPoint) + offset);
            //lineRenderer.SetPosition(0, new Vector3(clickPoint.x, clickPoint.y, 2));
            //lineRenderer.SetPosition(1, new Vector3(destination.x, destination.y, 2));
            lineRenderer.SetPosition(0, new Vector3(launchPoint.x, launchPoint.y, 2));
            lineRenderer.SetPosition(1, new Vector3(destination.x, destination.y, 2));
        }

        if (currentMode == Mode.tapLaunch)
        {
            Vector2 aimPosition = Camera.main.ScreenToWorldPoint(eventData.position);
            aimPosition.y += offset;
            clickPoint = aimPosition;
            aim.transform.position = aimPosition;
            Ray ray = new Ray(Aim.Instance.position, aimPosition - Aim.Instance.position);
            aimPosition = ray.GetPoint(Vector2.Distance(aimPosition, Aim.Instance.position) - (offset*2)/5);
            Aim.Instance.PutAimPoints(aimPosition);
        }
    }

    public void _OnPointerUp(PointerEventData eventData)
    {
        if (CanShoot() && (currentMode == Mode.manualAiming || currentMode == Mode.tapLaunch))
        {
            RocketLaunch();
            Lightning();
        }

        lineRenderer.SetPosition(0, new Vector3(0, 0, 2));
        lineRenderer.SetPosition(1, new Vector3(0, 0, 2));
        clickPoint = launchPoint;
        isDraged = false;
        aim.SetActive(false);
        Aim.Instance.DeactivateAim();
    }

    private void RocketLaunch()
    {
        if (currentMode == Mode.rocketGuidance || currentMode == Mode.armageddon)
        {
            if (GameManager.Instance.currEnemyRockets.Count > rocketGuidedRCounter)
            {
                ThreeBezierScript bezier = GameManager.Instance.GetRocketFromPool(currentMode);
                bezier.currentMode = currentMode;
                ThreeBezierScript target = GameManager.Instance.currEnemyRockets[rocketGuidedRCounter];
                bezier.speed = target.speed * 1.2f;
                Ray ray = new Ray(launchPoint, target.gameObject.transform.position - launchPoint);
                bezier.SetPoints(launchPoint,
                                 ray.GetPoint(Vector3.Distance(launchPoint, target.gameObject.transform.position) / 2),
                                 target.gameObject.transform);
                rocketGuidedRCounter++;
            }
        }
        if (currentMode == Mode.manualAiming || currentMode == Mode.tapLaunch)
        {
            ThreeBezierScript bezier = GameManager.Instance.GetRocketFromPool(currentMode);
            bezier.currentMode = currentMode;
            SetPoints(bezier);
        }
        cooldown = 0f;
    }
    
    private void SetPoints(ThreeBezierScript bezier)
    {
        bezier.SetPointP1(new Vector3());
        if (currentMode == Mode.manualAiming || currentMode == Mode.tapLaunch)
        {
            Ray ray = new Ray(launchPoint, clickPoint - new Vector2(launchPoint.x, launchPoint.y));//new Vector2(launchPoint.x, launchPoint.y) - clickPoint);
            Vector2 P3 = new Vector2();
            P3 = ray.GetPoint(Vector2.Distance(ScreenInfo.Instance.maxScreenEdge, ScreenInfo.Instance.minScreenEdge));
            bezier.SetPoints(launchPoint, clickPoint, P3);
        }
    }
    
    private void Lightning()
    {
        if (!lightning)
            return;
        Vector2 pos = lightning.transform.position;
        lightning.transform.rotation = Quaternion.LookRotation(clickPoint - pos);
        lightning.startSize = Vector2.Distance(lightning.transform.position, clickPoint) / 2;
        lightning.Play();
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
        currentMode = lastAimMode;
    }
    
    private IEnumerator timer(float time)
    {
        yield return new WaitForSeconds(time);
    }
}