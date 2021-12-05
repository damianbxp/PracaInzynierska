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

        if(allowMovement) {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(new Vector3(xAngle, yAngle, newTheta)), speed * Time.deltaTime);
        }
    }

    public void SetTheta(float angle) {
        if(!float.IsNaN(angle)) { // sprawdzenie czy "angle" jest liczb�
            if(minTheta != 0 && maxTheta != 0) // sprawdzenie czy na o� jest na�o�one ograniczenie
                angle = Mathf.Clamp(angle, minTheta, maxTheta); // korekcja k�ta
            dummyAxis.SetTheta(angle); // nadanie k�ta dla robota ducha
            newTheta = Mathf.Rad2Deg * angle; // nadanie k�ta
        }
    }
}
