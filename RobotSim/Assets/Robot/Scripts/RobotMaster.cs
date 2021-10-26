using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotMaster : MonoBehaviour
{
    public UIManager uiManager;
    public bool isPaused;
    int currentCommand;
    //public List<GCommand> commands;

    public Vector3 targetPos;

    public Vector3 homePointOffset;
    public Transform toolTarget;
    public GcodeInterpreter interpreter;

    public float feedRate;
    public float maxSpeed;
    public bool fastMove;

    public float spindleSpeed;

    public float posPrecision;

    List<string> gcodeLines;

    private void Start() {
        isPaused = true;
        targetPos = new Vector3();
        //commands = interpreter.LoadCommands();
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        UpdateGcodeLines();

        DecryptLine(gcodeLines[0]);
    }

    void Update()
    {
        if(!isPaused && currentCommand < gcodeLines.Count) {

        }


        //if(!isPaused && currentCommand<commands.Count) {
        //    if(!commands[currentCommand].done && !commands[currentCommand].active) {
        //        uiManager.UpdateConsole(commands[currentCommand].ToString());
        //        commands[currentCommand].Execute(this);
        //    }
        //    if(Vector3.Distance(toolTarget.position, targetPos) < posPrecision) {
        //        commands[currentCommand].done = true;
        //        commands[currentCommand].active = false;
        //        fastMove = false;
        //        currentCommand++;
        //    } else {
        //        if(fastMove) {
        //            toolTarget.position = homePointOffset + targetPos/1000;
        //        } else {

        //        }
        //    }
        //}
    }
    void UpdateGcodeLines() {
        string text = GameObject.Find("GcodeText").GetComponent<Text>().text;
        gcodeLines = new List<string>(text.Replace("\r", "").Split('\n'));
    }
    List<GCommand> DecryptLine(string code) {
        code = code.ToUpper().Replace(" ", "");
        GcodeData temp = new GcodeData();
        List<GcodeData> codeList = new List<GcodeData>();

        int dataStart = 0, dataEnd = 0;

        //Debug.Log(code);
        for(int i = 0; i < code.Length; i++) {
            if(char.IsLetter(code[i])) {
                if(dataStart > 0 && dataEnd > 0) {
                    temp.dataValue = float.Parse(code.Substring(dataStart, dataEnd - dataStart));
                }
                if(i > 0) {
                    //Debug.Log($"{temp.dataType}{temp.dataValue}");
                    codeList.Add(temp);
                }
                temp.dataType = code[i].ToString();
                dataStart = i + 1;
                dataEnd = dataStart;
            } else { // is number
                dataEnd++;
            }
        }
        codeList.Add(temp);

        GCommand gCommand = new GCommand();
        List<GCommand> gCommandsList = new List<GCommand>();

        for(int i = 0; i < codeList.Count; i++) {
            if(codeList[i].dataType == "G") {
                if(gCommand.name != "") {
                    Debug.Log("Added " + gCommand.name);
                    gCommandsList.Add(gCommand);
                }

                gCommand.name = codeList[i].dataType + codeList[i].dataValue.ToString();
                
            }
            for(;i < codeList.Count && codeList[i].dataType != "G"; i++) {
                Debug.Log($"[{i+1}/{codeList.Count}] {codeList[i].dataType}");
            }
        }
        return gCommandsList;
    }

}
public struct GcodeData {
    public string dataType;
    public float dataValue;
}