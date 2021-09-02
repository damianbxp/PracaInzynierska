using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SelectionBase]
public class Tool : MonoBehaviour
{
    public event System.Action onTerrainModified;
    public GameObject blockMesh;
    public Transform toolWorkCenter;

    public float toolRadius;
    public float toolHeight;

    public bool autoTransform;
    void Start()
    {
        
    }

    void Update() {
        if(Input.GetKey(KeyCode.F) || autoTransform) {
            blockMesh.GetComponent<GenTest>().Terraform(toolWorkCenter.position, toolRadius, toolHeight);
            //Debug.Log(transform.position - blockMesh.transform.position);
            onTerrainModified?.Invoke();
        }
    }
}
