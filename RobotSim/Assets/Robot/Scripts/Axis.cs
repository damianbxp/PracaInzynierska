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
    public float jointMoveOffset;

    [Header("Limits")]
    public float minTheta;
    public float maxTheta;

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
        if(allowMovement) {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(new Vector3(xAngle, yAngle, theta)), speed * Time.deltaTime);
        }
    }

    public void SetTheta(float angle) {
        if(!float.IsNaN(angle)) { // sprawdzenie czy "angle" jest liczb¹
            if(minTheta != 0 && maxTheta != 0) // sprawdzenie czy na oœ jest na³o¿one ograniczenie
                angle = Mathf.Clamp(angle, minTheta, maxTheta); // korekcja k¹ta
            dummyAxis.SetTheta(angle); // nadanie k¹ta dla robota ducha
            theta = Mathf.Rad2Deg * angle; // nadanie k¹ta
        }
    }
}
