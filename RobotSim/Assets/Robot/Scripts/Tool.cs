
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SelectionBase]
public class Tool : MonoBehaviour
{
    public event System.Action onTerrainModified;
    public GameObject blockMesh;
    public Transform toolWorkCenter;

    public float toolDiameter;
    public float toolHeight;

    Transform TCP;

    public bool autoTransform;
    void Start()
    {
        TCP = GameObject.Find("TCP").transform;

    }

    void Update() {
        transform.position = TCP.position;
        transform.rotation = TCP.rotation;

        if(Input.GetKey(KeyCode.F) || autoTransform) {
            blockMesh.GetComponent<GenTest>().Terraform(toolWorkCenter.position, toolDiameter/2, toolHeight);
            //Debug.Log(transform.position - blockMesh.transform.position);
            onTerrainModified?.Invoke();
        }
    }
}
