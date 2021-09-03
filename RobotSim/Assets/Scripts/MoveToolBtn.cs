using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveToolBtn : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    public bool holdingBtn;
    public int axis;
    public float repeatTime;

    UIManager uiManager;
    float time;

    private void Start() {
        holdingBtn = false;
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        time = 0f;
    }

    private void Update() {
        if(holdingBtn) {
            time += Time.deltaTime;
            //Debug.Log(time);
            if(time >= repeatTime) {
                uiManager.MoveToolIncrement(axis);
                time -= repeatTime;
            }
            
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        holdingBtn = true;
        time = 0f;
        uiManager.MoveToolIncrement(axis);

    }

    public void OnPointerUp(PointerEventData eventData) {
        holdingBtn = false;
    }
}
