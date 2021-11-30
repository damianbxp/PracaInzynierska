using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    Text FPSText;
    Text ToolPosText;
    Text ToolRotText;
    Text ConsoleText;

    Text BlockWidthText;
    Text BlockLenghtText;
    Text BlockHeightText;

    Text BlockPosX;
    Text BlockPosY;
    Text BlockPosZ;

    Text IncrementMoveText;
    Text IncrementRotText;

    Transform toolTarget;
    public float incrementMoveAmount;
    public float incrementRotAmount;

    Text ToolDiameter;
    Text ToolHeight;

    BlockGen gen;
    RobotMaster robotMaster;
    public Tool tool;

    private void Start() {
        toolTarget = GameObject.Find("ToolTarget").transform;
        incrementMoveAmount = 0.001f;

        FPSText = GameObject.Find("FPSText").GetComponent<Text>();
        ToolPosText = GameObject.Find("ToolPosText").GetComponent<Text>();
        ToolRotText = GameObject.Find("ToolRotText").GetComponent<Text>();
        ConsoleText = GameObject.Find("ConsoleText").GetComponent<Text>();

        BlockWidthText = GameObject.Find("WidthText").GetComponent<Text>();
        BlockLenghtText = GameObject.Find("LenghtText").GetComponent<Text>();
        BlockHeightText = GameObject.Find("HeightText").GetComponent<Text>();

        BlockPosX = GameObject.Find("PosXText").GetComponent<Text>();
        BlockPosY = GameObject.Find("PosYText").GetComponent<Text>();
        BlockPosZ = GameObject.Find("PosZText").GetComponent<Text>();

        IncrementMoveText = GameObject.Find("IncrementMoveText").GetComponent<Text>();
        IncrementRotText = GameObject.Find("IncrementRotText").GetComponent<Text>();

        ToolDiameter = GameObject.Find("ToolDiameterInputText").GetComponent<Text>();
        ToolHeight = GameObject.Find("ToolHeightInputText").GetComponent<Text>();

        gen = GameObject.Find("MeshGenerator").GetComponent<BlockGen>();
        robotMaster = GameObject.Find("RobotMaster").GetComponent<RobotMaster>();


        SetMoveIncrement(20);
        SetRotIncrement(5);

        GenerateBlock();
    }

    private void Update() {
        UpdateFPSText();
    }

    public void GenerateBlock() {
        

        float width, lenght, height;
        float x, y, z;

        if(!float.TryParse(BlockWidthText.text,out width)) {
            Debug.LogWarning("Incorrect dimmesnion");
            return;
        }
        if(!float.TryParse(BlockLenghtText.text, out lenght)) {
            Debug.LogWarning("Incorrect dimmesnion");
            return;
        }
        if(!float.TryParse(BlockHeightText.text, out height)) {
            Debug.LogWarning("Incorrect dimmesnion");
            return;
        }
        if(!float.TryParse(BlockPosX.text, out x)) {
            Debug.LogWarning("Incorrect posintion");
            return;
        }
        if(!float.TryParse(BlockPosY.text, out y)) {
            Debug.LogWarning("Incorrect posintion");
            return;
        }
        if(!float.TryParse(BlockPosZ.text, out z)) {
            Debug.LogWarning("Incorrect posintion");
            return;
        }

        Debug.Log($"Generating Block {width} x {lenght} x {height}");

        gen.GenerateBlock(new Vector3(x,y,z)/1000, width, lenght, height);

    }

    void UpdateFPSText() {
        FPSText.text = "FPS " + Mathf.Round(1/Time.deltaTime);
    }
    public void UpdateToolPosition(Vector3 toolPos, Vector3 toolRot) {
        ToolPosText.text = $"Tool Position:\n\tX:{toolPos.x}\n\tY:{toolPos.y}\n\tZ:{toolPos.z}";
        ToolRotText.text = $"\nA:{toolRot.x}\nB:{toolRot.y}\nC:{toolRot.z}";
    }

    public void MoveTool(Vector3 newPos) {
        toolTarget.position = newPos;
    }

    public void RotateTool(Vector3 newRotation) {
        toolTarget.rotation = Quaternion.Euler(newRotation);
    }

    public void UpdateConsole(string consoleText) {
        Debug.Log(consoleText);
        ConsoleText.text = consoleText;
    }

    public void MoveToolIncrement(int axis) {

        if(robotMaster.jogMode) {
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
    }

    public void RotToolIncrement(int axis) {

        if(robotMaster.jogMode) {
            switch(axis) {
                case 1:
                    //RotateTool(new Vector3(incrementRotAmount, 0, 0) + toolTarget.rotation.eulerAngles);
                    toolTarget.Rotate(new Vector3(incrementRotAmount, 0, 0));
                    break;
                case -1:
                    //RotateTool(new Vector3(-incrementRotAmount, 0, 0) + toolTarget.rotation.eulerAngles);
                    toolTarget.Rotate(new Vector3(-incrementRotAmount, 0, 0));
                    break;
                case 2:
                    //RotateTool(new Vector3(0, incrementRotAmount, 0) + toolTarget.rotation.eulerAngles);
                    toolTarget.Rotate(new Vector3(0, incrementRotAmount, 0));
                    break;
                case -2:
                    //RotateTool(new Vector3(0, -incrementRotAmount, 0) + toolTarget.rotation.eulerAngles);
                    toolTarget.Rotate(new Vector3(0, -incrementRotAmount, 0));
                    break;
                case 3:
                    //RotateTool(new Vector3(0, 0, incrementRotAmount) + toolTarget.rotation.eulerAngles);
                    toolTarget.Rotate(new Vector3(0, 0, incrementRotAmount));
                    break;
                case -3:
                    //RotateTool(new Vector3(0, 0, -incrementRotAmount) + toolTarget.rotation.eulerAngles);
                    toolTarget.Rotate(new Vector3(0, 0, -incrementRotAmount));
                    break;
                default:
                    Debug.LogError("Wrong axis passed");
                    break;
            }
        }
    }

    public void ChangeMoveIncrement(float amount) {
        SetMoveIncrement(incrementMoveAmount * 1000 + amount);

        //incrementMoveAmount = Mathf.Clamp(incrementMoveAmount*1000 + amount, 0, 1000)/1000;
        //IncrementMoveText.text = ( incrementMoveAmount * 1000 ).ToString();
    }

    public void SetMoveIncrement(float amount) {
        incrementMoveAmount = Mathf.Clamp(amount, 0, 1000) / 1000;
        IncrementMoveText.text = (incrementMoveAmount*1000).ToString();
    }

    public void ChangeRotIncrement(float amount) {
        SetRotIncrement(incrementRotAmount + amount);
    }

    public void SetRotIncrement(float amount) {
        incrementRotAmount = Mathf.Clamp(amount, 0, 360);
        IncrementRotText.text = incrementRotAmount.ToString();
    }

    public void WindowVisibility(Transform uiWindow) {
        if(uiWindow == null) {
            Debug.LogError("Nothing assigned to button");
            return;
        }
        CanvasGroup canvasRenderer = uiWindow.GetComponent<CanvasGroup>();
        if(canvasRenderer != null) {
            canvasRenderer.interactable = !canvasRenderer.interactable;
            canvasRenderer.blocksRaycasts = canvasRenderer.interactable;
            if(canvasRenderer.interactable) canvasRenderer.alpha = 1f;
            else canvasRenderer.alpha = 0f;
        } else {
            Debug.LogError("Canvas Renderer not found");
        }
    }

    public void SetTool() {
        float toolDiameter;
        float toolHeight;
        if(!float.TryParse(ToolDiameter.text, out toolDiameter)) {
            Debug.LogWarning("Float Parse Failed "+ ToolDiameter.text);
            return;
        }
        if(!float.TryParse(ToolHeight.text, out toolHeight)) {
            Debug.LogWarning("Float Parse Failed " + ToolHeight.text);
            return;
        }

        tool.UpdateTool(toolDiameter, toolHeight);
    }
}