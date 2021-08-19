using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKSolver : MonoBehaviour
{
    public Transform target;

    List<Axis> axes = new List<Axis>();

    public Transform rotBase, joint1, joint2;

    Vector2 targetLocalPos;


    float a1, a2;
    float q2Offset;

    private void Start() {
        UpdateAxes(transform.GetChild(0));
        a1 = 0.85f;
        a2 = 1.22f;
        //a1 = 1f;
        //a2 = 1f;
        q2Offset = 90 - Mathf.Rad2Deg* Mathf.Atan(a2 / 0.145f);
    }


    private void Update() {
        UpdateTargetLocalPos();

        //inv4();
        invLocalPos();
        IkRotation();
    }

    private void UpdateTargetLocalPos() {
        Vector3 temp = rotBase.InverseTransformVector(target.position - joint1.position);
        targetLocalPos = new Vector2(-temp.x, temp.z);
    }   

    private void UpdateAxes(Transform joint) {
        axes.Add(joint.GetComponent<Axis>());

        if(joint.childCount > 0) {
            UpdateAxes(joint.GetChild(0));
        }
    }

    void IkRotation() {

        float q0 = Mathf.Atan(target.position.z / target.position.x)*Mathf.Rad2Deg;
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
        Debug.Log(q0);
    }

    void invLocalPos() {
        float q1, q2, beta, alpha, d, a;
        d = targetLocalPos.x;
        alpha = Mathf.Atan(( 1.22f + 0.17f ) / 0.145f); // przy obrocie kiœci¹ to bêdzie zmienna
        a = Mathf.Sqrt(Mathf.Pow(0.145f, 2) + Mathf.Pow(1.22f + 0.17f, 2)); // przy obrocie kiœci¹ to bêdzie zmienna

        beta = Mathf.Pow(d, 2) + Mathf.Pow(targetLocalPos.y, 2) - Mathf.Pow(0.85f, 2) - Mathf.Pow(a, 2);
        beta /= 2 * 0.85f * a;
        beta = Mathf.Acos(beta);

        q1 = Mathf.Atan(( targetLocalPos.y ) / (d));
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

    void inv4() {
        float q1,q2, beta, alpha, d, a;

        d = Mathf.Sqrt(Mathf.Pow(target.position.x, 2) + Mathf.Pow(target.position.z, 2));//pos robota = (0,0,0)
        alpha = Mathf.Atan((1.22f + 0.17f) / 0.145f);
        a = Mathf.Sqrt(Mathf.Pow(0.145f, 2) + Mathf.Pow(1.22f + 0.17f, 2));

        beta = Mathf.Pow(d - 0.35f, 2) + Mathf.Pow(target.position.y - 0.815f, 2) - Mathf.Pow(0.85f, 2) - Mathf.Pow(a, 2);
        beta /= 2 * 0.85f * a;
        beta = Mathf.Acos(beta);

        q1 = Mathf.Atan(( target.position.y - 0.815f ) / ( d - 0.35f ));
        q1 += Mathf.Atan(( a * Mathf.Sin(beta) ) / ( 0.85f + a * Mathf.Cos(beta) ));
        q1 *= Mathf.Rad2Deg;

        beta *= Mathf.Rad2Deg;
        alpha *= Mathf.Rad2Deg;
        q2 = beta - alpha;

        q1 -= 90;
        q1 *= -1;

        //Debug.Log("q1="+q1+"|q2="+q2);

        if (!float.IsNaN(q1) && !float.IsNaN(q2)) {
            if(q1>axes[1].minTheta && q2<axes[1].maxTheta && q2 >axes[2].minTheta && q2 < axes[2].maxTheta) {
                axes[1].theta = q1;
                axes[2].theta = q2;
            }
        }

    }

    
}
