using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotMaster : MonoBehaviour
{
    public bool isPaused = true;

    public bool jogMode = false;
    public bool spindle = false;
    public bool enableIK = true;
    public UIManager uiManager;
    public InverseKinematicsDH inverseKinematics;
    public BlockGen blockGen;
    public Tool tool;
    public GcodeInterpreter gcodeInterpreter;


    ToolTarget toolTargetScript;
    public Transform toolTarget;
    public Transform toolTransform;

    public Vector3 homePoint;

    Text JogBtnText;
    Text RunBtnText;
    Text PauseBtnText;
    Text SpindleBtnText;
    private void Start() {
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        JogBtnText = GameObject.Find("JogBtnText").GetComponent<Text>();
        RunBtnText = GameObject.Find("RunBtnText").GetComponent<Text>();
        PauseBtnText = GameObject.Find("PauseBtnText").GetComponent<Text>();
        SpindleBtnText = GameObject.Find("SpindleBtnText").GetComponent<Text>();

        UpdateButtons();
    }

    void Update()
    {
        uiManager.UpdateToolPosition(((toolTransform.position-homePoint)*1000).Round(2), toolTransform.rotation.eulerAngles.Round(2));
    }

    

    public void PauseProgram() {
        isPaused = !isPaused;
        foreach(Axis axis in inverseKinematics.axes) {
            axis.allowMovement = !isPaused;
        }
        UpdateButtons();
    }

    public void JogMode() {
        jogMode = !jogMode;
        gcodeInterpreter.isStarted = false;
        UpdateButtons();
    }

    public void Spindle() {
        Debug.LogError(spindle);
        Spindle(!spindle);
    }

    public void Spindle(bool value) {
        if(spindle != value) {
            spindle = value;
            tool.powerOn = spindle;
            UpdateButtons();
        }
    }

    public void EnableIK(bool enable) {
        enableIK = enable;
    }

    public void UpdateButtons() {
        if(isPaused) PauseBtnText.color = Color.green;
        else PauseBtnText.color = Color.black;

        if(jogMode) JogBtnText.color = Color.green;
        else JogBtnText.color = Color.black;

        if(gcodeInterpreter.isStarted) RunBtnText.color = Color.green;
        else RunBtnText.color = Color.black;

        if(spindle) SpindleBtnText.color = Color.green;
        else SpindleBtnText.color = Color.black;
    }

    public void SetToolTarget(Vector3 pos, Vector3 rot) {
        toolTarget.position = pos / 1000 + homePoint;
        toolTarget.eulerAngles = new Vector3(rot.x, rot.z, rot.y);
    }
}
