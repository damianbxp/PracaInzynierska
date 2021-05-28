using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FcnBlockScript : MonoBehaviour {
    List<Transform> childList = new List<Transform>();
    bool isExpanded = true;
    private void Start() {
        for(int i = 1; i < transform.childCount; i++) {
            childList.Add(transform.GetChild(i));
        }
    }

    public void Expand() {
        
        for(int i = 0; i < childList.Count; i++) {
            childList[i].gameObject.SetActive(!isExpanded);
        }
        isExpanded = !isExpanded;
    }

}
