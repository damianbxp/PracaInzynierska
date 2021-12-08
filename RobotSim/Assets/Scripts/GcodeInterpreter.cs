using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GcodeInterpreter : MonoBehaviour
{
    RobotMaster robotMaster;
    public UIManager uiManager;

    public bool isStarted = false;
    public float posPrecision = 0.01f;
    int currentLine;
    int currentCommand = 0;

    List<string> gcodeLines;
    List<GCommand> GCommandsLine = new List<GCommand>();

    string[] moveCommands = { "G0", "G1", "G2", "G3" };
    void Start()
    {
        robotMaster = GameObject.Find("RobotMaster").GetComponent<RobotMaster>();

    }

    void Update()
    {
        if(!robotMaster.isPaused && !robotMaster.jogMode && isStarted && currentLine < gcodeLines.Count) {
            if(currentCommand >= GCommandsLine.Count) { // je¿eli zrobi³ wszystkie komendy w lini
                GCommandsLine = DecryptLine(gcodeLines[currentLine]);
                //Debug.Log($"Commands in line {currentLine}: {GCommandsLine.Count}");
                currentCommand = 0;
                uiManager.UpdateConsole(gcodeLines[currentLine]);
            } else {
                Debug.Log($"Running {currentCommand+1}/{GCommandsLine.Count} in line {currentLine+1}: {GCommandsLine[currentCommand]}");

                switch(GCommandsLine[currentCommand].name) { //wykonaj komende
                    case "G0": {
                        robotMaster.SetToolTarget(GCommandsLine[currentCommand].position);
                        //Debug.Log($"{toolTarget.position} {toolTransform.position} {Vector3.Distance(toolTarget.position, toolTransform.position)}");
                        if(Vector3.Distance(robotMaster.toolTarget.position, robotMaster.toolTransform.position) <= posPrecision) {
                            GCommandsLine[currentCommand].done = true;
                        }
                        break;
                    }
                    case "G1": {//nie dzia³a

                        break;
                    }
                    case "G53": {
                        Debug.Log(GCommandsLine[currentCommand]);

                        GCommandsLine[currentCommand].done = true;
                        break;
                    }
                    default: {
                        Debug.LogWarning("Not supported command");
                        break;
                    }

                }

                if(GCommandsLine[currentCommand].done) {
                    currentCommand++;
                }

            }

            if(currentCommand >= GCommandsLine.Count) {
                currentLine++;
            }
        }
    }

    void UpdateGcodeLines() {
        string text = GameObject.Find("GcodeText").GetComponent<TMPro.TextMeshProUGUI>().text;
        Debug.Log(text);
        gcodeLines = new List<string>(text.Replace("\r", "").Split('\n'));
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

            if(gCommands.Count > 0) {
                gCommand.previousCommand = gCommands[gCommands.Count - 1];
                gCommands[gCommands.Count - 1].nextCommand = gCommand;
            }
            gCommand.UpdateCommand();
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
        currentLine = 0;
        currentCommand = 0;
        isStarted = true;
        robotMaster.jogMode = false;
        uiManager.GenerateBlock();
        if(!robotMaster.isPaused) robotMaster.PauseProgram();

        UpdateGcodeLines();

        robotMaster.UpdateButtons();
    }
}
public struct GcodeData {
    public string dataType;
    public float dataValue;
}