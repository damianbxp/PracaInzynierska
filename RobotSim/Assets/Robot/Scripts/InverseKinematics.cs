using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseKinematics : MonoBehaviour
{
    public Transform target;
    List<Axis> axes = new List<Axis>();

    public Transform rotationJoint, upperArmJoint; 

    Vector2 targetLocalPos;

    private void Start() {
        UpdateAxes(transform.GetChild(0));
    }

    private void Update() {
        UpdateTargetLocalPos();
        IKSolveBaseRotation();
        IKSolvePosition();
    }

    void IKSolvePosition() {
        float q1, q2, beta, alpha, d, a;
        d = targetLocalPos.x;
        alpha = Mathf.Atan(( 1.22f + 0.17f ) / 0.145f); // przy obrocie kiœci¹ to bêdzie zmienna
        a = Mathf.Sqrt(Mathf.Pow(0.145f, 2) + Mathf.Pow(1.22f + 0.17f, 2)); // przy obrocie kiœci¹ to bêdzie zmienna

        beta = Mathf.Pow(d, 2) + Mathf.Pow(targetLocalPos.y, 2) - Mathf.Pow(0.85f, 2) - Mathf.Pow(a, 2);
        beta /= 2 * 0.85f * a;
        beta = Mathf.Acos(beta);

        q1 = Mathf.Atan(( targetLocalPos.y ) / ( d ));
        q1 += Mathf.Atan(( a * Mathf.Sin(beta) ) / ( 0.85f + a * Mathf.Cos(beta) ));
        q1 *= Mathf.Rad2Deg;

        beta *= Mathf.Rad2Deg;
        alpha *= Mathf.Rad2Deg;
        q2 = beta - alpha;

        q1 -= 90;
        q1 *= -1;

        //Debug.Log("q1="+q1+"|q2="+q2 + "|d=" + d + "|beta=" + beta);
        //Debug.Log("q1="+q1+"|q2="+q2);


        if(!float.IsNaN(q1) && !float.IsNaN(q2)) {
            axes[1].theta = q1;
            axes[2].theta = q2;
        }
    }

    void IKSolveBaseRotation() {
        float q0 = Mathf.Atan(target.position.z / target.position.x) * Mathf.Rad2Deg;
        if(target.position.x > 0) {
            if(target.position.z > 0) {
                q0 = 360 - q0;
            } else {
                q0 *= -1;
            }
        } else {
            q0 = 180 - q0;
        }
        axes[0].theta = q0;
        //Debug.Log(q0);
    }

    private void UpdateTargetLocalPos() {
        Vector3 temp = rotationJoint.InverseTransformVector(target.position - upperArmJoint.position);
        targetLocalPos = new Vector2(-temp.x, temp.z);
    }

    private void UpdateAxes(Transform joint) {
        axes.Add(joint.GetComponent<Axis>());

        if(joint.childCount > 0) {
            UpdateAxes(joint.GetChild(0));
        }
    }
}
