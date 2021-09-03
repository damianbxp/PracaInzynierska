using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    Text FPSText;
    Text VerticesText;
    Text ToolPosText;

    private void Start() {
        FPSText = GameObject.Find("FPSText").GetComponent<Text>();
        VerticesText = GameObject.Find("VerticesText").GetComponent<Text>();
        ToolPosText = GameObject.Find("ToolPosText").GetComponent<Text>();

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
}


