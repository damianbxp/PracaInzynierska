using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyAxis : MonoBehaviour
{
    public bool debug = false;
    float xAngle;
    float yAngle;

    private void Start() {
        xAngle = transform.localEulerAngles.x;
        yAngle = transform.localEulerAngles.y;
    }

    private void Update() {
        if(debug) {
            Gizmos.DrawCube(transform.position, Vector3.one);
        }
    }

    public void SetTheta(float angle) {
        if(!float.IsNaN(angle)) {
            transform.localRotation = Quaternion.Euler(new Vector3(xAngle, yAngle, Mathf.Rad2Deg * angle));
        }
    }
}
