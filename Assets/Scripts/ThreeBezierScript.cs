using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteAlways]
public class ThreeBezierScript : MonoBehaviour
{
    [SerializeField] private Transform P0;
    [SerializeField] private Transform P1;
    [SerializeField] private Transform P2;

    [SerializeField] private AnimationCurve health;

    [SerializeField] private float speed = 0.5f;

    [Range(0, 1)]
    [SerializeField] private float t;

    private float x = 0f;
    [HideInInspector] public bool move = false;
    public Mode currentMode;
    public enum Mode
    {
        loop,
        rocketGuidance
    }

    private void Start() {
        if (currentMode == Mode.loop)
        {
            move = true;
        }


    }

    void Update()
    {
        if (move == true)
        {
            MoveForward();

            if (t >= 1)
            {
                if (currentMode == Mode.loop)
                {
                    x = 0;
                    RandomP1();
                }
                if (currentMode == Mode.rocketGuidance)
                {
                    x = 0;
                    move = false;
                    gameObject.SetActive(false);
                }
            }
        }
    }

    public void MoveForward()
    {
        x += Time.deltaTime * speed;
        t = x;
        
        transform.position = Bezier.GetThreePoint(P0.position, P1.position, P2.position, t);
        transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, 1), 
            Bezier.GetFirstDerivativeForThreePoints(P0.position, P1.position, P2.position, t));
    }

    public ThreeBezierScript(Transform P0, Transform P1, Transform P2)
    {
        SetPoints(P0, P1, P2);
    }

    private void RandomP1()
    {
        Vector3 testP1position = new Vector3( P0.position.x + Random.Range(3f, 7f) * randomPlusMinus(), 
            P0.position.y + Random.Range(3f, 7f) * randomPlusMinus(), 0 );
        P1.position = testP1position;
    }

    private int randomPlusMinus()
    {
        return Random.value >= 0.5 ? 1 : -1;
    }
    IEnumerator waiter()
    {
        // //Rotate 90 deg
        // transform.Rotate(new Vector3(90, 0, 0), Space.World);

        //Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //Wait for 4 seconds
        yield return new WaitForSeconds(4);

        // //Rotate 40 deg
        // transform.Rotate(new Vector3(40, 0, 0), Space.World);

        // //Wait for 2 seconds
        // yield return new WaitForSeconds(2);

        // //Rotate 20 deg
        // transform.Rotate(new Vector3(20, 0, 0), Space.World);
    }

    private void OnDrawGizmos() {

        int sigmentNumbers     = 40;
        Vector3 preveousePoint = P0.position;

        for( int i=0; i<sigmentNumbers+1; i++ ) {
            float parameter = (float)i / sigmentNumbers;

            Vector3 point = Bezier.GetThreePoint(P0.position, P1.position, P2.position, parameter);

            Gizmos.DrawSphere(point, 0.08f);
            Gizmos.color = Color.magenta;

            preveousePoint = point;
        }

        for( int i=0; i<sigmentNumbers+1; i++ )
        {   
            float parameter = (float)i / sigmentNumbers;

            Vector3 point = Bezier.GetTwoPoint(P2.position, P1.position, parameter);

            Gizmos.DrawSphere(point, 0.08f);
            Gizmos.color = Color.green;

            preveousePoint = point;            
        }
        Gizmos.DrawSphere(P1.transform.position, 0.5f);
    }

    public void SetPoints(Transform P0, Transform P1, Transform P2)
    {
        this.P0 = P0;
        this.P1 = P1;
        this.P2 = P2;
    }
    public void SetPointP1(Transform P1)
    {
        this.P1 = P1;
    }

    public void SetPointP2(Transform P2)
    {
        this.P2 = P2;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (currentMode != Mode.loop)
        {
            if (col.gameObject.tag == gameObject.tag)
            {
                return;
            }
            Destroy(P1.gameObject);
            Destroy(gameObject);
        }
    }
}
