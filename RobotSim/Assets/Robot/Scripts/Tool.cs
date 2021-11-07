using UnityEngine;
[SelectionBase]
public class Tool : MonoBehaviour
{
    public event System.Action onTerrainModified;
    public GameObject blockMesh;
    public Transform toolWorkCenter;

    public float toolDiameter;
    public float toolHeight;


    public bool powerOn;

    void Start()
    {
        UpdateTool();
    }

    void Update() {

        transform.rotation = Quaternion.Euler(0, 0, 0); /// utrzymuje wrzeciono pionowo <- usun¹æ

        if(powerOn) {
            blockMesh.GetComponent<BlockGen>().ModifyBlock(toolWorkCenter.position, toolDiameter/2, toolHeight);
            //Debug.Log(transform.position - blockMesh.transform.position);
            onTerrainModified?.Invoke();
        }
    }

    public void UpdateTool() {
        transform.localPosition = new Vector3(toolHeight / 2, 0, 0.05f);
        transform.Find("Cylinder").localScale = new Vector3(toolDiameter, toolHeight/2, toolDiameter);
        toolWorkCenter.localPosition = new Vector3(0, -toolHeight / 2, 0);
    }
}
