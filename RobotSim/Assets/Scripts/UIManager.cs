using System.Collections.Generic;
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
    InputField IncrementJointText;

    Transform toolTarget;
    public float incrementMoveAmount;
    public float incrementRotAmount;
    public float incrementJointAmount;
    int moveCoordSys;
    Transform toolLocalCoord;
    Text RobotCoord;
    Text ToolCoord;
    Text JointCoord;

    public List<InputField> jointMoveUIFields;

    public CanvasGroup KartensianCoordGroup;
    public CanvasGroup JointCoordGroup;

    Text ToolDiameter;
    Text ToolHeight;
    Text ToolTipAngle;

    BlockGen gen;
    RobotMaster robotMaster;
    InverseKinematicsDH IK;
    public Tool tool;

    private void Start() {
        toolTarget = GameObject.Find("ToolTarget").transform;
        toolLocalCoord = GameObject.Find("ToolLocalCoord").transform;
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
        IncrementJointText = GameObject.Find("JointIncrementInput").GetComponent<InputField>();

        RobotCoord = GameObject.Find("RobotCoordText").GetComponent<Text>();
        RobotCoord.color = Color.green;
        ToolCoord = GameObject.Find("ToolCoordText").GetComponent<Text>();
        JointCoord = GameObject.Find("JointCoordText").GetComponent<Text>();
        
        

        ToolDiameter = GameObject.Find("ToolDiameterInputText").GetComponent<Text>();
        ToolHeight = GameObject.Find("ToolHeightInputText").GetComponent<Text>();
        ToolTipAngle = GameObject.Find("ToolTipAngleInputText").GetComponent<Text>();

        gen = GameObject.Find("MeshGenerator").GetComponent<BlockGen>();
        robotMaster = GameObject.Find("RobotMaster").GetComponent<RobotMaster>();
        IK = GameObject.Find("kuka_kr_60_L30_HA").GetComponent<InverseKinematicsDH>();

        SetMoveIncrement(10);
        SetRotIncrement(5);

        UpdateJointIncrement();
        GenerateBlock();
        SetTool();
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

    public void MoveTool(Vector3 addPos) {
        switch(moveCoordSys) {
            case 0: {
                toolTarget.position += addPos;
                break;
            }
            case 1: {
                toolTarget.position += toolLocalCoord.TransformDirection(addPos);
                break;
            }
            default: {
                Debug.LogWarning("Wrong Coord Systen Passed");
                break;
            }
        }
    }
    public void RotateTool(Vector3 addRot) {
        switch(moveCoordSys) {
            case 0: {
                toolTarget.Rotate(addRot,Space.World);
                break;
            }
            case 1: {
                toolTarget.Rotate(addRot);
                break;
            }
            default: {
                Debug.LogWarning("Wrong Coord Systen Passed");
                break;
            }
        }
    }

    public void UpdateConsole(string consoleText) {
        Debug.Log(consoleText);
        ConsoleText.text = consoleText;
    }

    public void SetMoveCoordSys(int coord) {
        switch(coord) {
            case 0: {//robot
                moveCoordSys = coord;
                RobotCoord.color = Color.green;
                ToolCoord.color = Color.black;
                JointCoord.color = Color.black;

                KartensianCoordGroup.interactable = true;
                KartensianCoordGroup.blocksRaycasts = true;
                KartensianCoordGroup.alpha = 1;

                JointCoordGroup.interactable = false;
                JointCoordGroup.blocksRaycasts = false;
                JointCoordGroup.alpha = 0;

                robotMaster.enableIK = true;

                break;
            }
            case 1: {//narzedzie
                moveCoordSys = coord;
                RobotCoord.color = Color.black;
                ToolCoord.color = Color.green;
                JointCoord.color = Color.black;

                KartensianCoordGroup.interactable = true;
                KartensianCoordGroup.blocksRaycasts = true;
                KartensianCoordGroup.alpha = 1;

                JointCoordGroup.interactable = false;
                JointCoordGroup.blocksRaycasts = false;
                JointCoordGroup.alpha = 0;
                robotMaster.enableIK = true;
                break;
            }
            case 2: {// oœ
                RobotCoord.color = Color.black;
                ToolCoord.color = Color.black;
                JointCoord.color = Color.green;

                KartensianCoordGroup.interactable = false;
                KartensianCoordGroup.blocksRaycasts = false;
                KartensianCoordGroup.alpha = 0;

                JointCoordGroup.interactable = true;
                JointCoordGroup.blocksRaycasts = true;
                JointCoordGroup.alpha = 1;
                robotMaster.enableIK = false;

                for(int i = 0; i < 6; i++) {
                    jointMoveUIFields[i].text = (IK.axes[i].theta - IK.axes[i].jointMoveOffset).ToString();
                    SetJointTheta(i);
                }

                break;
            }
            default: {
                Debug.LogWarning("Wrong Coord Systen Passed");
                break;
            }
        }
    }

    #region JogMove
    public void MoveToolIncrement(int axis) {
        if(robotMaster.jogMode) {
            switch(axis) {
                case 1:
                    MoveTool(new Vector3(incrementMoveAmount, 0, 0));
                    break;
                case -1:
                    MoveTool(new Vector3(-incrementMoveAmount, 0, 0));
                    break;
                case 2:
                    MoveTool(new Vector3(0, incrementMoveAmount, 0));
                    break;
                case -2:
                    MoveTool(new Vector3(0, -incrementMoveAmount, 0));
                    break;
                case 3:
                    MoveTool(new Vector3(0, 0, incrementMoveAmount));
                    break;
                case -3:
                    MoveTool(new Vector3(0, 0, -incrementMoveAmount));
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
    #endregion
    #region JogRot
    public void RotToolIncrement(int axis) {
        if(robotMaster.jogMode) {
            switch(axis) {
                case 1:
                    RotateTool(new Vector3(incrementRotAmount, 0, 0));
                    break;
                case -1:
                    RotateTool(new Vector3(-incrementRotAmount, 0, 0));
                    break;
                case 2:
                    RotateTool(new Vector3(0, incrementRotAmount, 0));
                    break;
                case -2:
                    RotateTool(new Vector3(0, -incrementRotAmount, 0));
                    break;
                case 3:
                    RotateTool(new Vector3(0, 0, incrementRotAmount));
                    break;
                case -3:
                    RotateTool(new Vector3(0, 0, -incrementRotAmount));
                    break;
                default:
                    Debug.LogError("Wrong axis passed");
                    break;
            }
        }
    }
    public void ChangeRotIncrement(float amount) {
        SetRotIncrement(incrementRotAmount + amount);
    }
    public void SetRotIncrement(float amount) {
        incrementRotAmount = Mathf.Clamp(amount, 0, 360);
        IncrementRotText.text = incrementRotAmount.ToString();
    }
    #endregion
    #region JogJoint
    public void AddJointIncrement(float amount) {
        SetJointIncrement(amount + incrementJointAmount);
    }
    public void SetJointIncrement(float amount) {
        incrementJointAmount = Mathf.Clamp(amount, 0, 360);
        IncrementJointText.text = incrementJointAmount.ToString();
    }
    public void UpdateJointIncrement() {
        if(float.TryParse(IncrementJointText.text, out float i))
            incrementJointAmount = i;
        else
            Debug.LogWarning("Float Parse Failed " + IncrementJointText.text);
    }

    public void IncreaseJointAngle(int axis) {
        if(!robotMaster.jogMode)
            return;
        if(!float.TryParse(jointMoveUIFields[axis].text, out float theta)) {
            Debug.LogWarning("Float Parse Failed " + jointMoveUIFields[axis].text);
            return;
        }

        jointMoveUIFields[axis].text = (theta + incrementJointAmount).ToString();
        SetJointTheta(axis);
    }

    public void DecreaseJointAngle(int axis) {
        if(!robotMaster.jogMode)
            return;
        if(!float.TryParse(jointMoveUIFields[axis].text, out float theta)) {
            Debug.LogWarning("Float Parse Failed " + jointMoveUIFields[axis].text);
            return;
        }

        jointMoveUIFields[axis].text = ( theta - incrementJointAmount ).ToString();
        SetJointTheta(axis);
    }

    public void SetJointTheta(int axis) {
        if(!robotMaster.jogMode)
            return;
        if(!float.TryParse(jointMoveUIFields[axis].text, out float theta)) {
            Debug.LogWarning("Float Parse Failed " + jointMoveUIFields[axis].text);
            return;
        }
        SetJointTheta(axis, theta);
    }
    public void SetJointTheta(int axis, float theta) {
        IK.axes[axis].SetTheta((theta + IK.axes[axis].jointMoveOffset)*Mathf.Deg2Rad);
    }

    #endregion

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
        float toolDiameter, toolHeight, toolTipAngle;
        
        if(!float.TryParse(ToolDiameter.text, out toolDiameter)) {
            Debug.LogWarning("Float Parse Failed "+ ToolDiameter.text);
            return;
        }
        if(!float.TryParse(ToolHeight.text, out toolHeight)) {
            Debug.LogWarning("Float Parse Failed " + ToolHeight.text);
            return;
        }
        if(!float.TryParse(ToolTipAngle.text, out toolTipAngle)) {
            Debug.LogWarning("Float Parse Failed " + ToolTipAngle.text);
            return;
        }
        toolDiameter /= 1000;
        toolHeight /= 1000;
        

        tool.UpdateTool(toolDiameter, toolHeight, toolTipAngle);
    }
}