using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading;
//[ExecuteAlways]
public class ThreeBezierScript : MonoBehaviour
{
    [SerializeField] private Vector3 p0;
    [SerializeField] private Vector3 p1;
    [SerializeField] private Vector3 p2;

    public Vector3 P0 { get { return p0; } }
    public Vector3 P1 { get { return p1; } }
    public Vector3 P2 { get { return p2; } }

    [SerializeField] private AnimationCurve health;

    [SerializeField] private int subdivs = 20;
    public float speed = 0.5f;
    [Range(0, 1)]
    [SerializeField] private float t = 0f;
    public float T { set { t = T; } get { return t; } }

    public RocketLauncher.Mode currentMode;
    private LineRenderer lineRenderer;
    public bool isDrawn = false;

    public float[] speedByChordsLengths;
    private float totalLength;

    void Start() 
    {
        ResetCoords();
        lineRenderer = GetComponent<LineRenderer>();
    }

    void FixedUpdate()
    {//
     //if (currentMode == RocketLauncher.Mode.rocketGuidance)
     //{
     //    if (P2.gameObject.activeSelf == false)//
     //    {
     //        x = 0;
     //        GameObject newP0 = new GameObject();
     //        newP0.transform.position = gameObject.transform.position;
     //        p0 = newP0.transform;
     //        p1 = p0;
     //        GameObject P2 = GameManager.Instance.currEnemyRockets.Count > 0 ? GameManager.Instance.currEnemyRockets[0]
     //        : GameManager.Instance.enemyPlanet;
     //        this.P2 = P2.transform;
     //    } //магическое перенаправление

        //}
        try
        {
            if (isDrawn == false && lineRenderer != null)
                DrawPath();
            MoveForward();
        } 
        catch
        {
            Debug.Log("error");
            Destroy(gameObject);
        }
        if (t >= 1)
        {    

            if (currentMode == RocketLauncher.Mode.loop)
            {
                t = 0;
                RandomP1();
            }
            else Destroy(gameObject);
        }
    }
    public void MoveForward()
    {
        t += Time.deltaTime * speed / totalLength / Bezier.GetSpeedByCoordLength(t, subdivs, ref speedByChordsLengths);
        t = Mathf.Clamp01(t);

        transform.position = Bezier.GetThreePoint(P0, P1, P2, t);
        transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, 1), 
            Bezier.GetFirstDerivativeForThreePoints(P0, P1, P2, t));
    }

    public void RandomP1(float minOffset = 3f, float maxOffset = 7f)
    {
        Vector3 testP1position = new Vector3(P0.x + Random.Range(minOffset, maxOffset) * randomPlusMinus(), 
            P0.y + Random.Range(minOffset, maxOffset) * randomPlusMinus(), 0 );
        p1 = testP1position;
        ResetCoords();
    }

    private int randomPlusMinus()
    {
        return Random.value >= 0.5 ? 1 : -1;
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
        Bezier.PrepareCoords(subdivs, P0, P1, P2, ref speedByChordsLengths, ref totalLength);

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (currentMode != RocketLauncher.Mode.loop)
        {
            if (col.gameObject.layer == gameObject.layer)
                return;
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
        if (gameObject.activeSelf == true)
        {
            if (currentMode == RocketLauncher.Mode.manualAiming || currentMode == RocketLauncher.Mode.tapLaunch)
                GameManager.Instance.RocketBackToPool(this);
            else Destroy(gameObject);
        }
    }
}
