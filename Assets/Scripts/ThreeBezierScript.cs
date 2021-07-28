using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteAlways]
public class ThreeBezierScript : MonoBehaviour
{
    [SerializeField] private Vector3 p0;
    [SerializeField] private Vector3 p1;
    [SerializeField] private Vector3 p2;

    public Vector3 P0 { get { return p0; } }
    public Vector3 P1 { get { return p1; } }
    public Vector3 P2 { get { return p2; } }
    //for self guidance mode
    [SerializeField] private Transform target;

    [SerializeField] private AnimationCurve health;

    [SerializeField] protected int subdivs = 20;
    [SerializeField] public float speed;
    [Range(0, 1)]
    [SerializeField] protected float t = 0f;
    public float T { set { t = T; } get { return t; } }

    [HideInInspector] public RocketLauncher.Mode currentMode;
    private LineRenderer lineRenderer;
    public bool isDrawn = false;

    public float[] speedByChordsLengths;
    protected float totalLength;

    void Start() 
    {
        ResetCoords();
        lineRenderer = GetComponent<LineRenderer>();
    }

    void FixedUpdate()
    {
        try
        {
            if (isDrawn == false && lineRenderer != null)
                DrawPath();
            if (currentMode == RocketLauncher.Mode.enemyAI && EnemyAI.Instance.enemyMode == EnemyAI.eMode.Linear)
                MoveForward();
            else
                MoveBezier();
        } 
        catch
        {
            Debug.Log("error");
            Destroy(gameObject);
        }        
    }
    //For spiral
    //float t_ellipse = 0;
    //public float a, b, t_speed;
    private void MoveBezier()
    {
        //Code for a spiral movement:
        //transform.position = Bezier.SpiralThreePointBezier(P0, P1, P2, t, t_ellipse, a, b);
        //Vector3 relativePos = Bezier.SpiralThreePointBezier(P0, P1, P2,
        //t + Time.deltaTime * speed / totalLength / Bezier.GetSpeedByCoordLength(t + Time.deltaTime, subdivs, ref speedByChordsLengths),
        //t_ellipse + Time.deltaTime, a, b) - transform.position;
        //Quaternion rotation = Quaternion.LookRotation(relativePos);
        //transform.rotation = rotation * Quaternion.Euler(90, 0, 0);
        //t_ellipse += Time.deltaTime * t_speed;

        t += Time.deltaTime * speed / totalLength / Bezier.GetSpeedByCoordLength(t, subdivs, ref speedByChordsLengths);
        t = Mathf.Clamp01(t);

        if ((currentMode == RocketLauncher.Mode.rocketGuidance || currentMode == RocketLauncher.Mode.armageddon) && target != null)
            p2 = target.transform.position;

        gameObject.transform.position = Bezier.GetThreePoint(P0, P1, P2, t);
        gameObject.transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, 1),
            Bezier.GetFirstDerivativeForThreePoints(P0, P1, P2, t));
        if (t >= 1)
        {
            if (currentMode == RocketLauncher.Mode.loop)
            {
                t = 0;
                RandomP1();
            }
            else
                Destroy(gameObject);
        }
    }
    private void MoveForward()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
        if (!GameManager.Instance.IsOnTheScreen(transform.position))
        {
            SetPoints(transform.position,
                GameManager.Instance.homePlanet.transform.position, GameManager.Instance.homePlanet.transform.position);
        }
    }

    private void SetForwardRotation()
    {
        Vector3 relativePos = P2 - P0;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        transform.rotation = rotation * Quaternion.Euler(90, 0, 0);
    }
    public void RandomP1(float minOffset = 3f, float maxOffset = 7f)
    {
        if (currentMode == RocketLauncher.Mode.enemyAI && EnemyAI.Instance.enemyMode == EnemyAI.eMode.Linear)
        {
            float x = Random.value >= 0.5 ? GameManager.Instance.minScreenEdge.x : GameManager.Instance.maxScreenEdge.x;
            float y = P0.y - Random.Range(minOffset, maxOffset);
            p1 = new Vector2(x, y);
            p2 = new Vector2(x, y);
            return;
        }
        else
        {
            p1 = new Vector3(P0.x + Random.Range(minOffset, maxOffset) * randomPlusMinus(),
                            P0.y + Random.Range(minOffset, maxOffset) * randomPlusMinus(),
                            P0.z);
        }
        ResetCoords();
    }

    private int randomPlusMinus()
    {
        return Random.value >= 0.5 ? 1 : -1;
    }

    private bool isLinearMotion()
    {
        return false;
    }
    private void DrawPath()
    {
        int sigmentNumbers = 20;
        lineRenderer.positionCount = sigmentNumbers + 1;
        Vector3 preveousePoint = P0;
        for (int i = 0; i < sigmentNumbers + 1; i++)
        {
            float parameter = (float)i / sigmentNumbers;
            Vector3 point = Bezier.GetThreePoint(P0, P1, P2, parameter);
            point.z = 1;
            lineRenderer.SetPosition(i, point);
            preveousePoint = point;
        }
        isDrawn = true;
    }

    public void SetPoints(Vector3 P0, Vector3 P1, Vector3 P2)
    {
        this.p0 = P0;
        this.p1 = P1;
        this.p2 = P2;
        ResetCoords();
    }
    public void SetPoints(Vector3 P0, Vector3 P1, Transform target)
    {
        this.p0 = P0;
        this.p1 = P1;
        this.p2 = target.transform.position;
        this.target = target;
        ResetCoords();
    }
    public void SetPointP1(Vector3 P1)
    {
        this.p1 = P1;
        ResetCoords();
    }

    public void SetPointP2(Vector3 P2)
    {
        this.p2 = P2;
        ResetCoords();
    }

    public void ResetCoords()
    {
        if (currentMode == RocketLauncher.Mode.enemyAI && EnemyAI.Instance.enemyMode == EnemyAI.eMode.Linear)
            SetForwardRotation();
        else
            Bezier.PrepareCoords(subdivs, P0, P1, P2, ref speedByChordsLengths, ref totalLength);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (currentMode != RocketLauncher.Mode.loop)
        {
            if (col.gameObject.layer == gameObject.layer)
                return;
            if (currentMode == RocketLauncher.Mode.rocketGuidance)
            {
                RocketLauncher.Instance.rocketGuidedRCounter--;
            }
            if (col.gameObject.tag == "HomePlanet")
            {
                GameManager.Instance.LevelIsLosed();
            }
            if (currentMode == RocketLauncher.Mode.enemyAI)
            {
                GameManager.Instance.EnemyRocketBackToPool(this);
                return;
            }
            if (currentMode != RocketLauncher.Mode.rocketGuidance && currentMode != RocketLauncher.Mode.none)
            {
                GameManager.Instance.RocketBackToPool(this);
                return;
            }

            Destroy(gameObject);
        }
    }
    void OnBecameInvisible()
    {
        GameManager.Instance.Explose(gameObject.transform.position);
        if (gameObject.activeSelf == true)
        {
            if (currentMode == RocketLauncher.Mode.manualAiming || currentMode == RocketLauncher.Mode.tapLaunch)
                GameManager.Instance.RocketBackToPool(this);
            else Destroy(gameObject);
        }
    }
}
