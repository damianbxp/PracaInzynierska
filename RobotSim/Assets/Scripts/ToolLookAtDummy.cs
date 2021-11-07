using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolLookAtDummy : MonoBehaviour
{
    public Transform ToolMount;
    void Update()
    {
        transform.position = ToolMount.position - Vector3.up * 5;
    }
}
