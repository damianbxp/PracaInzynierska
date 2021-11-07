using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Cinemachine.CinemachineFreeLook cameraCtrl;
    public float sensitivityX;
    public float sensitivityY;

    private void Update() {
        if(Input.GetKey(KeyCode.LeftAlt)) {
            MoveCamX();
            MoveCamY();
        }else if(Input.GetKey(KeyCode.LeftControl)) {
            MoveCamX();
        }
    }

    void MoveCamX() {
        cameraCtrl.m_XAxis.Value += Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
    }
    void MoveCamY() {
        cameraCtrl.m_YAxis.Value += Input.GetAxis("Mouse Y") * -sensitivityY * Time.deltaTime;
    }
}
