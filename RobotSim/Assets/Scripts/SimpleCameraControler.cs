using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraControler : MonoBehaviour
{
    Transform yPivot;
    Transform cameraTR;

    public Transform robot;
    public Transform block;
    public Transform tool;
    int lookAt;

    public float sensitivityX;
    public float sensitivityY;
    public float sensitivityZoom;

    public float zoomMin;
    public float zoomMax;

    void Start()
    {
        yPivot = transform.GetChild(0).transform;
        cameraTR = yPivot.GetChild(0).transform;
        ChangeCameraPivot();
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

        if(lookAt == 2) {
            transform.position = tool.position;
        }
    }

    void MoveX() {
        transform.Rotate(0, Input.GetAxis("Mouse X"), 0);
    }
    void MoveY() {
        float input = Input.GetAxis("Mouse Y");
        yPivot.Rotate(input * sensitivityY, 0, 0);
    }
    void Zoom() {
        float input = Input.GetAxis("Mouse ScrollWheel");
        //if((cameraTR.localPosition.z < -zoomMin || input < 0)&& (cameraTR.localPosition.z > -zoomMax || input > 0))
        //    cameraTR.Translate(0, 0, input * sensitivityZoom, Space.Self);
        input *= sensitivityZoom;
        cameraTR.localPosition = new Vector3(0,0,Mathf.Clamp(cameraTR.localPosition.z + input, -zoomMax, -zoomMin));
    }

    public void ChangeCameraPivot() {
        lookAt++;
        if(lookAt > 2)
            lookAt = 0;
        switch(lookAt) {
            case 0: {//patrz na robota
                transform.position = robot.position;
                break;
            }
            case 1: {//patrz na blok
                transform.position = block.position;
                break;
            }
            case 2: {//patrz na narzedzie
                transform.position = tool.position;
                break;
            }
            default: {
                Debug.LogError("Camera change - lookingAt wrong value");
                break;
            }
        }
    }
}
