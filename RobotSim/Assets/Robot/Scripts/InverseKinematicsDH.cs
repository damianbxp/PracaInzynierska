using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseKinematicsDH : MonoBehaviour
{
    public Transform armTarget;
    public Transform wristTarget;
    public Transform rotBaseTransform;
    public Transform joint3;
    public List<Axis> axes = new List<Axis>();
    //Matrix4x4 transformMatrix = new Matrix4x4();

    Vector2 wristLocalPos;


    private void Start() {
        UpdateAxes(transform.GetChild(0));
    }

    private void Update() {
        //UpdateMatix();
        Vector3 temp = rotBaseTransform.InverseTransformVector(wristTarget.position);
        wristLocalPos = new Vector2(temp.x, temp.z);
        Calculate();
    }

    void Calculate() {
        SetTheta1();
        SetTheta3_2();
        
    }

    void SetTheta1() {
        //float theta1 = Mathf.Atan2(transformMatrix.m23 - axes[5].d * transformMatrix.m22, transformMatrix.m03 - axes[5].d * transformMatrix.m02);
        float theta1 = Mathf.Atan2(wristTarget.position.x, wristTarget.position.z);
        theta1 -= Mathf.PI/2;
        axes[0].SetTheta(theta1);
    }
    void SetTheta3_2() {
        float alpha = Mathf.Atan(axes[3].d / axes[2].a);
        Vector2 adjustedWristLocalPos = new Vector2(-1*wristLocalPos.x - axes[0].a, wristLocalPos.y - axes[0].d);
        //Debug.Log(adjustedWristLocalPos);

        float l1 = Vector2.Distance(Vector2.zero, new Vector2(1.220f, 0.145f));
        float P24 = Vector2.Distance(axes[1].localPos, wristLocalPos);

        float theta3 = Pow2(adjustedWristLocalPos.x) + Pow2(adjustedWristLocalPos.y) - Pow2(axes[1].a) - Pow2(l1);
        theta3 /= 2 * axes[1].a * l1;
        theta3 = Mathf.Acos(theta3);

        //Debug.Log(theta3 * Mathf.Rad2Deg);


        float temp1 = Mathf.Atan(adjustedWristLocalPos.y / adjustedWristLocalPos.x);
        float temp2 = l1 * Mathf.Sin(theta3);
        temp2 /= axes[1].a + l1 * Mathf.Cos(theta3);
        temp2 = Mathf.Atan(temp2);

        float theta2 = temp1 + temp2;

        theta2 += axes[1].offset * Mathf.Deg2Rad;
        theta2 *= -1;

        theta3 -= alpha;
        theta3 += axes[2].offset * Mathf.Deg2Rad;

        axes[1].SetTheta(theta2);
        axes[2].SetTheta(theta3);

        //Debug.Log(adjustedWristLocalPos);
    }

    void UpdateMatix() {
        //transformMatrix.SetTRS(armTarget.position, armTarget.rotation, Vector3.one);
    }

    private void UpdateAxes(Transform joint) {
        if(joint.GetComponent<Axis>() != null) axes.Add(joint.GetComponent<Axis>());

        if(joint.childCount > 0) {
            UpdateAxes(joint.GetChild(0));
        }
    }

    float Pow2(float x) {
        return Mathf.Pow(x, 2);
    }
}
