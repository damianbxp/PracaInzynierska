using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axis : MonoBehaviour
{
    [Header("DH")]
    public float theta;
    public float d;
    public float a;
    public float alpha;
    public float offset;

    [Header("Limits")]
    public float minTheta;
    public float maxTheta;

    public Vector2 localPos;

    public float speed;

    float xAngle;
    float yAngle;

    public DummyAxis dummyAxis;
    public float newTheta;
    public Transform baseTransform;

    public bool allowMovement = true;

    private void Start() {
        xAngle = transform.localEulerAngles.x;
        yAngle = transform.localEulerAngles.y;
    }

    private void Update() {
        Vector3 temp = baseTransform.InverseTransformVector(transform.position);
        localPos = new Vector2(temp.x, temp.y);


        //transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(new Vector3(xAngle, yAngle, newTheta)), speed * Time.deltaTime);

        if(allowMovement) {
            //if(( minTheta == 0 && maxTheta == 0 ) || ( theta < maxTheta && theta > minTheta )) {
            //    newTheta = theta;
            //}
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(new Vector3(xAngle, yAngle, newTheta)), speed * Time.deltaTime);

        }
    }

    public void SetTheta(float angle) {
        if(!float.IsNaN(angle)) {
            dummyAxis.SetTheta(angle);
            newTheta = Mathf.Rad2Deg * angle;
        }

    }
}
