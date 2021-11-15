using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Theta4IKSolver : MonoBehaviour
{
    public Transform toolTarget;
    Vector3 localToolPos;
    public Axis axis;


    void Update()
    {
        localToolPos = transform.InverseTransformVector(transform.position - toolTarget.position);
        //Debug.Log(localToolPos * 1000);

        float theta4 = Mathf.Atan2(localToolPos.y, localToolPos.x);
        theta4 = 2 * Mathf.PI - theta4;
        theta4 += axis.offset * Mathf.Rad2Deg;
        Debug.Log(theta4 * Mathf.Rad2Deg);
        axis.SetTheta(theta4);
    }
}
