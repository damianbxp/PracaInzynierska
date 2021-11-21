using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Theta5IKSolver : MonoBehaviour
{
    public Transform lookAtTarget;
    Vector3 localToolPos;
    public Axis axis;

    private void Update() {
        localToolPos = transform.InverseTransformVector(transform.position - lookAtTarget.position);
        localToolPos = new Vector3(-localToolPos.y, -localToolPos.x);

        //Debug.Log(localToolPos * 1000);

        float theta5 = Mathf.Atan2(localToolPos.x, localToolPos.y);
        axis.SetTheta(theta5 + axis.offset *Mathf.Deg2Rad);

    }
}
