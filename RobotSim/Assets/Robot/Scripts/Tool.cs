using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SelectionBase]
public class Tool : MonoBehaviour
{
    public event System.Action onTerrainModified;
    public GameObject blockMesh;
    public Transform toolWorkCenter;

    public bool autoTransform;
    void Start()
    {
        
    }

    void Update() {
        if(Input.GetKey(KeyCode.F) || autoTransform) {
            blockMesh.GetComponent<GenTest>().Terraform(toolWorkCenter.position, 0.2f,0.02f);
            //Debug.Log(transform.position - blockMesh.transform.position);
            onTerrainModified?.Invoke();
        }
    }
}
