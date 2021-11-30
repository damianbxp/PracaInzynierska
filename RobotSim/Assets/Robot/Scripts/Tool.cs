using UnityEngine;
[SelectionBase]
public class Tool : MonoBehaviour
{
    public event System.Action onTerrainModified;
    public GameObject blockMesh;
    public Transform toolWorkCenter;

    public Transform armTarget;

    public float toolDiameter;
    public float toolHeight;


    public bool powerOn;

    public Transform toolMount;

    void Start()
    {
        UpdateTool();
    }

    void Update() {

        //transform.rotation = Quaternion.Euler(0, 0, 0); /// utrzymuje wrzeciono pionowo <- usun¹æ
        

        if(powerOn) {
            blockMesh.GetComponent<BlockGen>().ModifyBlock(toolWorkCenter, toolDiameter/2, toolHeight);
            //Debug.Log(transform.position - blockMesh.transform.position);
            onTerrainModified?.Invoke();
        }
    }

    public void UpdateTool(float diameter, float height) {
        toolDiameter = diameter;
        toolHeight = height;
        UpdateTool();
    }

    public void UpdateTool() {
        transform.localPosition = new Vector3(0,0,toolHeight / 2);
        transform.Find("Cylinder").localScale = new Vector3(toolDiameter, toolHeight/2, toolDiameter);
        toolWorkCenter.localPosition = new Vector3(0, -toolHeight / 2, 0);
        //armTarget.localPosition = new Vector3(0, toolHeight, 0);
    }
}
