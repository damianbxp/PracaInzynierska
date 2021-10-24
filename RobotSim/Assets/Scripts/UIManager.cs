using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    Text FPSText;
    Text VerticesText;
    Text ToolPosText;
    Text ConsoleText;

    Transform toolTarget;
    float incrementMoveAmount;

    private void Start() {
        toolTarget = GameObject.Find("ToolTarget").transform;
        incrementMoveAmount = 0.001f;

        FPSText = GameObject.Find("FPSText").GetComponent<Text>();
        VerticesText = GameObject.Find("VerticesText").GetComponent<Text>();
        ToolPosText = GameObject.Find("ToolPosText").GetComponent<Text>();
        ConsoleText = GameObject.Find("ConsoleText").GetComponent<Text>();
    }

    private void Update() {
        UpdateFPSText();
    }

    void UpdateFPSText() {
        FPSText.text = "FPS " + 1/Time.deltaTime;
    }

    public void UpdateVertices(int vertices) {
        VerticesText.text = "Vertices " + vertices;
    }
    public void UpdateToolPosition(Vector3 toolPos) {
        ToolPosText.text = $"Tool Position:\n\tX:{toolPos.x}\n\tY:{toolPos.y}\n\tZ:{toolPos.z}";
    }

    public void MoveTool(Vector3 newPos) {
        toolTarget.position = newPos;
    }

    public void UpdateConsole(string consoleText) {
        Debug.Log(consoleText);
        ConsoleText.text = consoleText;
    }

    public void MoveToolIncrement(int axis) {
        

        switch(axis) {
            case 1:
                MoveTool(new Vector3(incrementMoveAmount, 0, 0) + toolTarget.position);
                break;
            case -1:
                MoveTool(new Vector3(-incrementMoveAmount, 0, 0) + toolTarget.position);
                break;
            case 2:
                MoveTool(new Vector3(0, incrementMoveAmount, 0) + toolTarget.position);
                break;
            case -2:
                MoveTool(new Vector3(0, -incrementMoveAmount, 0) + toolTarget.position);
                break;
            case 3:
                MoveTool(new Vector3(0, 0, incrementMoveAmount) + toolTarget.position);
                break;
            case -3:
                MoveTool(new Vector3(0, 0, -incrementMoveAmount) + toolTarget.position);
                break;
            default:
                Debug.LogError("Wrong axis passed");
                break;
        }
    }
    
    public void SetIncrementAmount(float amount) {
        incrementMoveAmount = amount;
    }
    
}