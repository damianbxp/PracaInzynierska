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

    public bool autoTransform;
    void Start()
    {
        
    }

    void Update() {
        if(Input.GetKey(KeyCode.F) || autoTransform) {
            blockMesh.GetComponent<GenTest>().Terraform(toolWorkCenter.position, toolDiameter/2, toolHeight);
            //Debug.Log(transform.position - blockMesh.transform.position);
            onTerrainModified?.Invoke();
        }
    }
}
