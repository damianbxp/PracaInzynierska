using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraControler : MonoBehaviour
{
    Transform yPivot;
    Transform cameraTR;

    public float sensitivityX;
    public float sensitivityY;
    public float sensitivityZoom;

    void Start()
    {
        yPivot = transform.GetChild(0).transform;
        cameraTR = yPivot.GetChild(0).transform;
    }

    
    void Update()
    {
        if(Input.GetKey(KeyCode.LeftAlt)) {
            MoveX();
            MoveY();
            Zoom();
        } else if(Input.GetKey(KeyCode.LeftControl)) {
            MoveX();
        }
    }

    void MoveX() {
        transform.Rotate(0, Input.GetAxis("Mouse X")*sensitivityX*Time.deltaTime, 0);
    }
    void MoveY() {
        float input = Input.GetAxis("Mouse Y");
        yPivot.Rotate(input * sensitivityY * Time.deltaTime, 0, 0);
    }
    void Zoom() {
        float input = Input.GetAxis("Mouse ScrollWheel");
        if((cameraTR.localPosition.z < -1f || input < 0)&& (cameraTR.localPosition.z > -5 || input > 0))
            cameraTR.Translate(0, 0, input * sensitivityZoom * Time.deltaTime, Space.Self);
    }

    public void SetPivotPos(Transform tr) {
        transform.position = tr.position;
    }
}
