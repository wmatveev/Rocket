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

    [Range(0, 1)]
    [SerializeField] private float t;

    private IEnumerator coroutine;

    private float x = 0f;
    // private int i = 0;

    private void Start() {
    }

    // Update is called once per frame
    void Update()
    {
        // x = (float)i / 400;
        x += Time.deltaTime * 0.5f;

        t = x;

        transform.position = Bezier.GetThreePoint(P0.position, P1.position, P2.position, t);
        transform.rotation = Quaternion.LookRotation( new Vector3(0, 0, 1), Bezier.GetFirstDerivativeForThreePoints(P0.position, P1.position, P2.position, t) );

        if( t >= 1 ) {
            x = 0;
            RandomP1();
        }
    }

    private int fff()
    {
        return Random.value >= 0.5 ? 1 : -1;
    }

    private void RandomP1()
    {
        Vector3 testP1position = new Vector3( P0.position.x + Random.Range(3f, 7f) * fff(), 
            P0.position.y + Random.Range(3f, 7f) * fff(), 0 );
        P1.position = testP1position;
    }

    private void aaa()
    {
        // x += Time.deltaTime * 0.5f;
        // t = x;

        // transform.position = Bezier.GetThreePoint(P0.position, P1.position, P2.position, t);
        // transform.rotation = Quaternion.LookRotation( new Vector3(0, 0, 1), Bezier.GetFirstDerivativeForThreePoints(P0.position, P1.position, P2.position, t) );

        StartCoroutine( waiter() );
    }

    IEnumerator waiter()
    {
        // //Rotate 90 deg
        // transform.Rotate(new Vector3(90, 0, 0), Space.World);

        // Debug.Log("Started Coroutine at timestamp : " + Time.time);

        //Wait for 4 seconds
        yield return new WaitForSeconds(4);

        // Debug.Log("Finished Coroutine at timestamp : " + Time.time);


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

            Gizmos.DrawSphere(point, 0.5f);
            // Gizmos.DrawLine(preveousePoint, point);

            Gizmos.color = Color.magenta;
            // if( i % 2 == 0) Gizmos.color = Color.clear;
            // else Gizmos.color = Color.red;

            preveousePoint = point;
        }
    }

}
