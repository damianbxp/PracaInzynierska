using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKRotSolver : MonoBehaviour
{
    public Transform lookAtTarget;
    Vector3 localToolPos;
    public Axis axis;
    public DummyAxis dummyAxis;

    private void Update() {
        localToolPos = transform.InverseTransformVector(transform.position - lookAtTarget.position);
        localToolPos = new Vector3(-localToolPos.y, -localToolPos.x);

        //Debug.Log(localToolPos * 1000);

        float theta5 = Mathf.Atan2(localToolPos.x, localToolPos.y);
        theta5 = theta5 + axis.offset * Mathf.Deg2Rad;
        axis.SetTheta(theta5);
        dummyAxis.SetTheta(theta5);
    }
}
