using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GcodeCommands : MonoBehaviour
{
    Transform toolTarget;

    public void Start() {
        toolTarget = GameObject.Find("ToolTarget").transform;
    }

    public void G1(G1Data data) {
        float newX = ( data.x != float.NaN ) ? data.x : toolTarget.position.x;
        float newY = ( data.y != float.NaN ) ? data.y : toolTarget.position.y;
        float newZ = ( data.z != float.NaN ) ? data.z : toolTarget.position.z;

        Debug.Log($"New tool pos: {newX} {newY} {newZ}");

        toolTarget.position = new Vector3(newX, newY, newZ);
    }
}
