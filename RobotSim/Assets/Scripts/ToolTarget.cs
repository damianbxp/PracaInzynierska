using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SelectionBase()]
public class ToolTarget : MonoBehaviour
{
    RobotMaster robotMaster;
    Transform tcp;
    void Start()
    {
        robotMaster = GameObject.Find("RobotMaster").GetComponent<RobotMaster>();
        tcp = GameObject.Find("ToolWorkCenter").GetComponent<Transform>();
    }

    void Update()
    {
        if(!robotMaster.enableIK) {
            transform.position = tcp.position;
            transform.rotation = Quaternion.Euler(tcp.eulerAngles + new Vector3(-90,0,0));
        }
    }
}
