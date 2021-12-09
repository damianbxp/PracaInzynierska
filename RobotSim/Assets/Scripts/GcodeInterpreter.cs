using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GcodeInterpreter : MonoBehaviour
{
    RobotMaster robotMaster;
    public UIManager uiManager;

    public bool isStarted = false;
    public bool programFinished = false;
    public float posPrecision;
    //int currentLine;
    int currentCommand = 0;

    List<string> gcodeLines;
    List<GCommand> GCommandsList = new List<GCommand>();

    float programStartTime;
    float commandStartTime;

    public Text CommandTimeText; 
    public Text ProgramTimeText; 

    string[] moveCommands = { "G0", "G1", "G2", "G3" };
    void Start()
    {
        robotMaster = GameObject.Find("RobotMaster").GetComponent<RobotMaster>();

    }

    void Update()
    {
        if(!robotMaster.isPaused && !robotMaster.jogMode && isStarted && !programFinished) {
            RunProgram();
        }
    }

    void RunProgram() {
        if(GCommandsList[currentCommand].done) {
            currentCommand++;
            if(currentCommand >= GCommandsList.Count) {
                FinishProgram();
                return;
            }
            commandStartTime = Time.time;
            uiManager.UpdateConsole(GCommandsList[currentCommand].ToString());
        } else {
            Debug.Log($"Running {currentCommand + 1}/{GCommandsList.Count}: {GCommandsList[currentCommand]}");
            UpdateTimer();
            switch(GCommandsList[currentCommand].name) { //wykonaj komende
                case "G0": {
                    robotMaster.SetToolTarget(GCommandsList[currentCommand].position, GCommandsList[currentCommand].rotation);
                    //Debug.Log($"{robotMaster.toolTarget.position} {robotMaster.toolTransform.position} {Vector3.Distance(robotMaster.toolTarget.position, robotMaster.toolTransform.position)}");
                    if(Vector3.Distance(robotMaster.toolTarget.position, robotMaster.toolTransform.position) <= posPrecision) {
                        GCommandsList[currentCommand].done = true;
                    }
                    break;
                }
                case "G1": {//nie dzia³a
                    float distanceTraveled = ( Time.time - commandStartTime ) * GCommandsList[currentCommand].F;
                    float distanceFraction = distanceTraveled / Vector3.Distance(GCommandsList[currentCommand].previousCommand.position, GCommandsList[currentCommand].position);
                    Vector3 pos = Vector3.Lerp(GCommandsList[currentCommand].previousCommand.position, GCommandsList[currentCommand].position, distanceFraction);
                    Vector3 rot = Vector3.Lerp(GCommandsList[currentCommand].previousCommand.rotation, GCommandsList[currentCommand].rotation, distanceFraction);
                    robotMaster.SetToolTarget(pos, rot);
                    if(distanceFraction > 1) {
                        GCommandsList[currentCommand].done = true;
                    }
                    break;
                }
                case "G53": {

                    GCommandsList[currentCommand].done = true;
                    break;
                }
                default: {
                    Debug.LogWarning("Not supported command");
                    break;
                }

            }
        }
    }

    void UpdateGcodeLines() {
        string text = GameObject.Find("GcodeText").GetComponent<TMPro.TextMeshProUGUI>().text;
        //Debug.Log(text);
        gcodeLines = new List<string>(text.Replace("\r", "").Split('\n'));
    }

    List<GCommand> GenerateGCommandsList() {
        List<GCommand> list = new List<GCommand>();
        for(int i = 0; i < gcodeLines.Count; i++) {
            List<GCommand> line = DecryptLine(gcodeLines[i]);
            for(int g = 0; g < line.Count; g++) {
                list.Add(line[g]);
            }
        }

        for(int i = 1; i < list.Count; i++) {
            list[i].previousCommand = list[i - 1];
        }

        for(int i = 0; i < list.Count-1; i++) {
            list[i].nextCommand = list[i + 1];
        }

        for(int i = 0; i < list.Count; i++) {
            list[i].UpdateCommand();
        }

        for(int i = 0; i < list.Count; i++) {
            Debug.LogWarning(list[i]);
        }

        return list;
    }

    List<GCommand> DecryptLine(string code) {
        code = code.ToUpper().Replace(" ", "").Replace(".", ",");
        code += ";";
        List<GCommand> gCommands = new List<GCommand>();
        GCommand gCommand;
        int startIndex = code.IndexOf("G");

        while(startIndex != -1) {
            gCommand = new GCommand();

            gCommand.name = "G" + GetCommandValue("G", code, startIndex);
            if(IsMoveCommand(gCommand.name)) {              // komendy trwaj¹ce wiele klatek
                gCommand.X = GetCommandValue("X", code, startIndex);
                gCommand.Y = GetCommandValue("Y", code, startIndex);
                gCommand.Z = GetCommandValue("Z", code, startIndex);
                gCommand.A = GetCommandValue("A", code, startIndex);
                gCommand.B = GetCommandValue("B", code, startIndex);
                gCommand.C = GetCommandValue("C", code, startIndex);
                

            } else {                                        // komendy na jedn¹ klatkê

            }

            gCommand.F = GetCommandValue("F", code, startIndex);

            gCommands.Add(gCommand);
            startIndex = code.IndexOf("G", startIndex + 1);
        }


        //for(int i = 0; i < gCommands.Count; i++) {
        //    Debug.Log(gCommands[i]);
        //}
        return gCommands;
    }

    float GetCommandValue(string command, string code, int startIndex = 0) {
        if(code.IndexOf(command, startIndex) == -1) return float.NaN;

        int indexStart = code.IndexOf(command, startIndex) + command.Length;
        int indexEnd = 0;
        for(int i = indexStart; i < code.Length; i++) {
            indexEnd = i;
            if(!char.IsDigit(code[i]) && code[i] != ',' && code[i] != '-') break; // je¿eli nie jest cyfr¹ lub ',' lub '-'
        }
        if(indexStart >= indexEnd) return float.NaN;
        return float.Parse(code.Substring(indexStart, indexEnd - indexStart));
    }

    bool IsMoveCommand(string command) {
        for(int i = 0; i < moveCommands.Length; i++) {
            if(moveCommands[i] == command) return true;
        }
        return false;
    }

    public void StartProgram() {
        currentCommand = 0;
        isStarted = true;
        programFinished = false;
        robotMaster.jogMode = false;
        programStartTime = Time.time;
        uiManager.GenerateBlock();
        if(!robotMaster.isPaused) robotMaster.PauseProgram();

        UpdateGcodeLines();
        GCommandsList = GenerateGCommandsList();
        robotMaster.UpdateButtons();
    }

    public void FinishProgram() {
        programFinished = true;
        isStarted = false;
        uiManager.UpdateConsole("Zakoñczono program");
        robotMaster.UpdateButtons();
    }

    void UpdateTimer() {
        CommandTimeText.text = $"{( Time.time - commandStartTime ).Round(1):0.0#}s";
        ProgramTimeText.text = $"{( Time.time - programStartTime ).Round(1):0.0#}s";
    }
}
public struct GcodeData {
    public string dataType;
    public float dataValue;
}