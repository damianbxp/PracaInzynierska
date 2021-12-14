using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SelectionBase()]
public class ToolTarget : MonoBehaviour
{
    Transform tcp;
    void Start()
    {
        tcp = GameObject.Find("ToolWorkCenter").GetComponent<Transform>();
    }

    public void FollowTCP(bool follow) {
        if(follow)
            transform.SetParent(tcp);
        else
            transform.SetParent(null);
    }
}
