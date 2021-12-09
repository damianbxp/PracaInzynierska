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

    string[] linearCommands = { "G0", "G1"};
    string[] circularCommands = {"G2", "G3" };
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
                    G0Move();
                    break;
                }
                case "G1": {
                    G1Move();
                    break;
                }
                case "G2": {
                    G2G3Move(GCommandsList[currentCommand] as SGCommand);
                    break;
                }
                case "G3": {
                    G2G3Move(GCommandsList[currentCommand] as SGCommand);
                    break;
                }
                default: {
                    Debug.LogWarning("Not supported command");
                    break;
                }

            }
        }
    }

    void G0Move() {
        robotMaster.SetToolTarget(GCommandsList[currentCommand].position, GCommandsList[currentCommand].rotation);
        //Debug.Log($"{robotMaster.toolTarget.position} {robotMaster.toolTransform.position} {Vector3.Distance(robotMaster.toolTarget.position, robotMaster.toolTransform.position)}");
        if(Vector3.Distance(robotMaster.toolTarget.position, robotMaster.toolTransform.position) <= posPrecision) {
            GCommandsList[currentCommand].done = true;
        }
    }
    void G1Move() {
        float distComplete = ( Time.time - commandStartTime ) * GCommandsList[currentCommand].F;
        float distFraction = distComplete / Vector3.Distance(GCommandsList[currentCommand].previousCommand.position, GCommandsList[currentCommand].position);
        Vector3 pos = Vector3.Lerp(GCommandsList[currentCommand].previousCommand.position, GCommandsList[currentCommand].position, distFraction);
        Vector3 rot = Vector3.Lerp(GCommandsList[currentCommand].previousCommand.rotation, GCommandsList[currentCommand].rotation, distFraction);
        robotMaster.SetToolTarget(pos, rot);
        if(distFraction > 1) {
            GCommandsList[currentCommand].done = true;
        }
    }
    void G2G3Move(SGCommand g) {
        Vector3 center = g.previousCommand.position + g.offset;
        Vector3 relStart = g.previousCommand.position - center;
        Vector3 relEnd = g.position - center;

        float distComplete = ( Time.time - commandStartTime ) * g.F;

        bool longWay = false;

        float travelAngle = Vector3.Angle(g.position, g.previousCommand.position);
        float opositeTravelAngle = 360 - travelAngle;
        float totalDistance = ( 2 * Mathf.PI * g.offset.magnitude * travelAngle ) / 360;
        float opositeTotalDistance = ( 2 * Mathf.PI * g.offset.magnitude * opositeTravelAngle ) / 360;
        float distFraction = distComplete / totalDistance;
        float opositeDistFraction = distComplete / opositeTotalDistance;


        Vector3 pos = Vector3.SlerpUnclamped(relStart, relEnd, ( longWay ? -1 : 1 ) * distFraction); //d³u¿sza droga + | krótsza droga -
        //Debug.LogWarning($"{distFraction} {opositeDistFraction}");
        robotMaster.SetToolTarget(pos, Vector3.zero);
        if(Mathf.Abs(longWay ? opositeDistFraction : distFraction) > 1)
            g.done = true;
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
        int startIndex = code.IndexOf("G");

        while(startIndex != -1) {

            string name = "G" + GetCommandValue("G", code, startIndex);

            switch(CommandType(name)) {
                case 0: {   //komandy na jedn¹ klatkê
                    break;
                }
                case 1: {   //ruch liniowy
                    GCommand gCommand = new GCommand {
                        name = name,
                        X = GetCommandValue("X", code, startIndex),
                        Y = GetCommandValue("Y", code, startIndex),
                        Z = GetCommandValue("Z", code, startIndex),
                        A = GetCommandValue("A", code, startIndex),
                        B = GetCommandValue("B", code, startIndex),
                        C = GetCommandValue("C", code, startIndex),

                        F = GetCommandValue("F", code, startIndex),
                        S = GetCommandValue("S", code, startIndex)
                    };

                    gCommands.Add(gCommand);

                    break;
                }
                case 2: {   //ruch ko³owy
                    SGCommand sGCommand = new SGCommand {
                        name = name,
                        X = GetCommandValue("X", code, startIndex),
                        Y = GetCommandValue("Y", code, startIndex),
                        Z = GetCommandValue("Z", code, startIndex),
                        A = GetCommandValue("A", code, startIndex),
                        B = GetCommandValue("B", code, startIndex),
                        C = GetCommandValue("C", code, startIndex),

                        I = GetCommandValue("I", code, startIndex),
                        J = GetCommandValue("J", code, startIndex),
                        K = GetCommandValue("K", code, startIndex),

                        F = GetCommandValue("F", code, startIndex),
                        S = GetCommandValue("S", code, startIndex)
                    };

                    gCommands.Add(sGCommand);

                    break;
                }
                default:
                    break;
            }


            startIndex = code.IndexOf("G", startIndex + 1);
        }
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

    int CommandType(string command) {
        for(int i = 0; i < linearCommands.Length; i++) {
            if(linearCommands[i] == command) return 1;
            if(circularCommands[i] == command) return 2;
        }
        return 0;
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