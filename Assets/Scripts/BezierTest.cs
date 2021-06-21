using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteAlways]
public class BezierTest : MonoBehaviour
{
    [SerializeField] private Transform P0;
    [SerializeField] private Transform P1;
    [SerializeField] private Transform P2;
    [SerializeField] private Transform P3;

    [Range(0, 1)]
    [SerializeField] private float t;


    void Update()
    {
        transform.position = Bezier.GetFourPoint(P0.position, P1.position, P2.position, P3.position, t);
        transform.rotation = Quaternion.LookRotation( Bezier.GetFirstDerivativeForFourPoints(P0.position, P1.position, P2.position, P3.position, t) );
    }

    private void OnDrawGizmos() {
        int sigmentNumbers     = 20;
        Vector3 preveousePoint = P0.position;

        for( int i=0; i<sigmentNumbers+1; i++ ) {
            float parameter = (float)i / sigmentNumbers;

            Vector3 point = Bezier.GetFourPoint(P0.position, P1.position, P2.position, P3.position, parameter);
            Gizmos.DrawLine(preveousePoint, point);

            preveousePoint = point;
        }
    }
}
