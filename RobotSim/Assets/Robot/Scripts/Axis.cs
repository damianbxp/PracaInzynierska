using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axis : MonoBehaviour
{
    public float theta;
    public float minTheta;
    public float maxTheta;

    public float speed;

    float xAngle;
    float yAngle;
    public float offset;

    float newTheta;

    public bool allowMovement = true;

    private void Start() {
        xAngle = transform.localEulerAngles.x;
        yAngle = transform.localEulerAngles.y;
    }

    private void Update() {

        if(allowMovement) {
            if((minTheta==0 && maxTheta==0)||(theta<maxTheta && theta > minTheta)) {
                newTheta = theta + offset;
            }
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(new Vector3(xAngle, yAngle, newTheta)), speed*Time.deltaTime);
        }
    }
}
