using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    Text FPSText;
    Text ToolPosText;
    Text ConsoleText;

    Text BlockWidthText;
    Text BlockLenghtText;
    Text BlockHeightText;

    Text BlockPosX;
    Text BlockPosY;
    Text BlockPosZ;

    Text IncrementText;

    Transform toolTarget;
    public float incrementMoveAmount;

    BlockGen gen;
    RobotMaster robotMaster;

    private void Start() {
        toolTarget = GameObject.Find("ToolTarget").transform;
        incrementMoveAmount = 0.001f;

        FPSText = GameObject.Find("FPSText").GetComponent<Text>();
        ToolPosText = GameObject.Find("ToolPosText").GetComponent<Text>();
        ConsoleText = GameObject.Find("ConsoleText").GetComponent<Text>();

        BlockWidthText = GameObject.Find("WidthText").GetComponent<Text>();
        BlockLenghtText = GameObject.Find("LenghtText").GetComponent<Text>();
        BlockHeightText = GameObject.Find("HeightText").GetComponent<Text>();

        BlockPosX = GameObject.Find("PosXText").GetComponent<Text>();
        BlockPosY = GameObject.Find("PosYText").GetComponent<Text>();
        BlockPosZ = GameObject.Find("PosZText").GetComponent<Text>();

        IncrementText = GameObject.Find("IncrementText").GetComponent<Text>();

        gen = GameObject.Find("MeshGenerator").GetComponent<BlockGen>();
        robotMaster = GameObject.Find("RobotMaster").GetComponent<RobotMaster>();

        
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
    

    public void ChangeIncrement(float amount) {
        incrementMoveAmount = Mathf.Clamp(incrementMoveAmount*1000 + amount, 0, 1000)/1000;
        IncrementText.text = ( incrementMoveAmount * 1000 ).ToString();
    }

    public void SetIncrement(float amount) {
        incrementMoveAmount = Mathf.Clamp(amount, 0, 1000) / 1000;
        IncrementText.text = (incrementMoveAmount*1000).ToString();
    }
    
}