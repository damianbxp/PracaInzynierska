using UnityEngine;
[SelectionBase]
public class Tool : MonoBehaviour
{
    public event System.Action onTerrainModified;
    public GameObject blockMesh;
    public Transform toolWorkCenter;

    public Transform armTarget;

    public Transform dummyTool;

    public float toolDiameter;
    public float toolHeight;
    public float toolTipAngle;


    public bool powerOn;

    public Transform toolMount;

    void Start()
    {
        UpdateTool();
    }

    void Update() {
        if(powerOn) {
            blockMesh.GetComponent<BlockGen>().ModifyBlock(toolWorkCenter, toolDiameter/2, toolHeight, toolTipAngle/2);
            onTerrainModified?.Invoke();
        }
    }

    public void UpdateTool(float diameter, float height, float tipAngle) {
        toolDiameter = diameter;
        toolHeight = height;
        toolTipAngle = tipAngle;
        Debug.Log(toolTipAngle);
        UpdateTool();
    }

    public void UpdateTool() {
        transform.localPosition = new Vector3(0,0,toolHeight / 2);
        transform.GetChild(0).localScale = new Vector3(toolDiameter, toolHeight/2, toolDiameter);
        toolWorkCenter.localPosition = new Vector3(0, -toolHeight / 2, 0);
        armTarget.localPosition = new Vector3(0, toolHeight, 0);
        
        dummyTool.localPosition = new Vector3(0, 0, toolHeight / 2);
        dummyTool.GetChild(0).localScale = new Vector3(toolDiameter, toolHeight / 2, toolDiameter);

        GameObject.Find("RobotMaster").GetComponent<GcodeInterpreter>().posPrecision = toolHeight/1.8f;
    }
}
