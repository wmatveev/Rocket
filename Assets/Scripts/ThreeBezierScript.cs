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

    private IEnumerator coroutine;

    private float x = 0f;
    // private int i = 0;

    private void Start() {
    }

    // Update is called once per frame
    void Update()
    {
        x += Time.deltaTime * speed;
        t = x;

        transform.position = Bezier.GetThreePoint(P0.position, P1.position, P2.position, t);
        transform.rotation = Quaternion.LookRotation( new Vector3(0, 0, 1), Bezier.GetFirstDerivativeForThreePoints(P0.position, P1.position, P2.position, t) );

        if( t >= 1 ) {
            x = 0;
            RandomP1();
        }
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

    private void OnMouseDown() {
        
    }

    private void OnDrawGizmos() {
    //Отрисовка кривой Безье
        int sigmentNumbers     = 40;
        Vector3 preveousePoint = P0.position;

        for( int i=0; i<sigmentNumbers+1; i++ ) {
            float parameter = (float)i / sigmentNumbers;

            Vector3 point = Bezier.GetThreePoint(P0.position, P1.position, P2.position, parameter);

            Gizmos.DrawSphere(point, 0.08f);
            Gizmos.color = Color.magenta;

            preveousePoint = point;
        }

        //Отрисовка прямой
        for( int i=0; i<sigmentNumbers+1; i++ )
        {   
            float parameter = (float)i / sigmentNumbers;

            Vector3 point = Bezier.GetTwoPoint(P2.position, P1.position, parameter);

            Gizmos.DrawSphere(point, 0.08f);
            Gizmos.color = Color.green;

            preveousePoint = point;            
        }

    }

}
