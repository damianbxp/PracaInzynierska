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

        inv4();
        //inv3();
        //inv2();
        //inv1();
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

        Debug.Log("q1="+q1+"|q2="+q2+"|alpha="+alpha+"|beta="+beta);


        axes[1].theta = q1;
        axes[2].theta = q2;
    }

    void inv3() {
        float q1, q2;
        //targetLocalPos = new Vector2(0.5f, 0);

        q2 = Mathf.Pow(targetLocalPos.x, 2) + Mathf.Pow(targetLocalPos.y, 2) - Mathf.Pow(a1, 2) - Mathf.Pow(a2, 2);
        q2 /= 2 * a1 * a2;
        q2 = Mathf.Acos(q2);
        q1 = a2 * Mathf.Sin(q2);
        q1 /= a1 + a2 * Mathf.Cos(q2);
        q1 = Mathf.Atan(targetLocalPos.y / targetLocalPos.x) - Mathf.Atan(q1);

        q1 = Mathf.Rad2Deg * q1 * -1;
        q2 = Mathf.Rad2Deg * q2;

        q1 = 90 - q1;

        Debug.Log(q1 + "|" + q2);

        axes[1].theta = q1;
        axes[2].theta = q2;

    }
    void inv2() {
        float q1, q2;

        q1 = Mathf.Pow(a1, 2) + Mathf.Pow(targetLocalPos.x, 2) + Mathf.Pow(targetLocalPos.y, 2) - Mathf.Pow(a2, 2);
        q1 /= 2 * a1 * Mathf.Sqrt(Mathf.Pow(targetLocalPos.x, 2) + Mathf.Pow(targetLocalPos.y, 2));
        q1 = Mathf.Acos(q1);

        q2 = Mathf.Pow(a1, 2) + Mathf.Pow(a2, 2) - Mathf.Pow(targetLocalPos.x, 2) - Mathf.Pow(targetLocalPos.y, 2);
        q2 /= 2 * a1 * a2;
        q2 = Mathf.Acos(q2);

        Debug.Log(Mathf.Rad2Deg * q1 + "|" + Mathf.Rad2Deg * q2);

        axes[1].theta = 90 - Mathf.Rad2Deg * q1;
        axes[2].theta = 90 - Mathf.Rad2Deg * q2 + q2Offset;
    }

    void inv1() {
        float q1, q2;

        q2 = Mathf.Pow(targetLocalPos.x, 2) + Mathf.Pow(targetLocalPos.y, 2) - Mathf.Pow(a1, 2) - Mathf.Pow(a2, 2);
        q2 /= 2 * a1 * a2;
        q2 = Mathf.Acos(q2);

        q1 = a2 * Mathf.Sin(q2);
        q1 /= a1 + a2 * Mathf.Cos(q2);
        q1 = Mathf.Atan(targetLocalPos.y / targetLocalPos.x) - Mathf.Atan(q1);


        //Debug.Log(targetLocalPos);
        Debug.Log(Mathf.Rad2Deg * q1 +"|" + Mathf.Rad2Deg* q2);

        axes[1].theta = Mathf.Rad2Deg * q1;
        axes[2].theta = Mathf.Rad2Deg * q2 + 180;
    }
}
