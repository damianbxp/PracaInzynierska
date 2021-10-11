using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTarget : MonoBehaviour
{
    Transform TCP;

    private void Start() {
        TCP = GameObject.Find("TCP").transform;
    }
    void Update()
    {
        transform.position = TCP.position;
        transform.rotation = TCP.rotation;
    }
}
