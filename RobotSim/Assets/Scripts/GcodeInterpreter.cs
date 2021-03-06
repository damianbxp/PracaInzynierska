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
            //Debug.Log($"Running {currentCommand + 1}/{GCommandsList.Count}: {GCommandsList[currentCommand]}");
            UpdateTimer();
            switch(GCommandsList[currentCommand].name) { //wykonaj komende
                case "G0": {
                    G0Move();
                    SpindleUpdate(GCommandsList[currentCommand]);
                    break;
                }
                case "G1": {
                    G1Move(GCommandsList[currentCommand]);
                    SpindleUpdate(GCommandsList[currentCommand]);
                    break;
                }
                case "G2": {
                    
                    G2G3Move(GCommandsList[currentCommand] as SGCommand);
                    SpindleUpdate(GCommandsList[currentCommand]);
                    break;
                }
                case "G3": {
                    G2G3Move(GCommandsList[currentCommand] as SGCommand);
                    SpindleUpdate(GCommandsList[currentCommand]);
                    break;
                }
                case "S": {
                    SpindleUpdate(GCommandsList[currentCommand]);
                    GCommandsList[currentCommand].done = true;
                    break;
                }
                case "F": {
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

    void G0Move() {
        robotMaster.SetToolTarget(GCommandsList[currentCommand].position, GCommandsList[currentCommand].rotation);
        //Debug.Log($"{robotMaster.toolTarget.position} {robotMaster.toolTransform.position} {Vector3.Distance(robotMaster.toolTarget.position, robotMaster.toolTransform.position)}");
        if(Vector3.Distance(robotMaster.toolTarget.position, robotMaster.toolTransform.position) <= posPrecision) {
            GCommandsList[currentCommand].done = true;
        }
    }
    void G1Move(GCommand g) {
        if(float.IsNaN(g.F)) {
            uiManager.UpdateConsole("Nieznany posuw");
            FinishProgram();
            return;
        }
        float distComplete = ( Time.time - commandStartTime ) * g.F;
        float distFraction = distComplete / Vector3.Distance(g.previousCommand.position, g.position);
        Vector3 pos = Vector3.Lerp(g.previousCommand.position, g.position, distFraction);
        Vector3 rot = Quaternion.Lerp(Quaternion.Euler(g.previousCommand.rotation), Quaternion.Euler(g.rotation), distFraction).eulerAngles;

        robotMaster.SetToolTarget(pos, rot);
        if(distFraction > 1 && Vector3.Distance(robotMaster.toolTarget.position, robotMaster.toolTransform.position) <= posPrecision) {
            g.done = true;
        }
    }
    void G2G3Move(SGCommand g) {
        if(float.IsNaN(g.F)) {
            uiManager.UpdateConsole("Nieznany posuw");
            FinishProgram();
            return;
        }
        bool longWay = false;
        
        Vector3 center = g.previousCommand.position + g.offset; // obliczenie ?rodka ?uku
        Vector3 relStart = g.previousCommand.position - center; // obliczenie punktu pocz?tkowego wzgl?dem ?rodka
        Vector3 relEnd = g.position - center; // obliczenie punktu ko?cowego wzgl?dem ?rodka

        float distComplete = ( Time.time - commandStartTime ) * g.F;
        float travelAngle = Vector3.Angle(g.position, g.previousCommand.position);

        Vector3 normal = Vector3.Cross(relStart - center, relEnd - center).normalized;
        if(Vector3.Angle(normal, new Vector3(1, -1, 1)) >= 90) { // ruch przeciwnie do ruchu wskaz?wek zegara
            if(g.name == "G3")
                longWay = true;
        } else {
            if(g.name == "G2")
                longWay = true;
        }

        Debug.DrawLine(robotMaster.homePoint + new Vector3(center.x, center.z, center.y) / 1000, robotMaster.homePoint + new Vector3(center.x, center.z, center.y) / 1000 + normal.normalized * 0.2f, Color.black);
        Debug.DrawLine(robotMaster.homePoint + new Vector3(center.x, center.z, center.y) / 1000, robotMaster.homePoint + new Vector3(center.x, center.z, center.y) / 1000 + Vector3.forward * 0.1f, Color.green);
        Debug.DrawLine(robotMaster.homePoint + new Vector3(center.x, center.z, center.y) / 1000, robotMaster.homePoint + new Vector3(center.x, center.z, center.y) / 1000 + Vector3.right * 0.1f, Color.red);
        Debug.DrawLine(robotMaster.homePoint + new Vector3(center.x, center.z, center.y) / 1000, robotMaster.homePoint + new Vector3(center.x, center.z, center.y) / 1000 + Vector3.up * 0.1f, Color.blue);

        float opositeTravelAngle = 360 - travelAngle;
        float totalDistance = ( 2 * Mathf.PI * g.offset.magnitude * travelAngle ) / 360;
        float opositeTotalDistance = ( 2 * Mathf.PI * g.offset.magnitude * opositeTravelAngle ) / 360;
        float distFraction = distComplete / totalDistance;
        float opositeDistFraction = distComplete / opositeTotalDistance;

        if(!longWay) {
            distFraction = Mathf.Clamp(distFraction, 0, 1);
        } else {
            opositeDistFraction = Mathf.Clamp(opositeDistFraction, 0, 1);
        }

        Vector3 pos = Vector3.SlerpUnclamped(relStart, relEnd, ( longWay ? -1 : 1 ) * distFraction);
        pos += center;

        Vector3 rot = Quaternion.Lerp(Quaternion.Euler(g.previousCommand.rotation), Quaternion.Euler(g.rotation), distFraction).eulerAngles;

        robotMaster.SetToolTarget(pos, rot);
        if(Mathf.Abs(longWay ? opositeDistFraction : distFraction) == 1 && Vector3.Distance(robotMaster.toolTarget.position, robotMaster.toolTransform.position) <= posPrecision)
            g.done = true;
    }

    void SpindleUpdate(GCommand g) {
        robotMaster.Spindle(g.S > 0);
    }
    public Vector3 GetPointOnCircle(SGCommand g, float fraction) {
        //w uk?adzie XYZ
        Vector3 center = g.previousCommand.position + g.offset;
        Vector3 relStart = g.previousCommand.position - center;
        Vector3 relEnd = g.position - center;

        bool longWay = false;

        Vector3 normal = Vector3.Cross(relStart - center, relEnd - center).normalized;

        if(Vector3.Angle(normal, new Vector3(1, -1, 1)) >= 90) {
            if(g.name == "G3")
                longWay = true;
        } else {
            if(g.name == "G2")
                longWay = true;
        }

        Vector3 pos = Vector3.SlerpUnclamped(relStart, relEnd, ( longWay ? -1 : 1 ) * fraction);
        return pos += center;
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

        if(startIndex == -1) {
            float otherValue;
            otherValue = GetCommandValue("S", code);
            if(!float.IsNaN(otherValue)) {
                GCommand g = new GCommand {
                    name = "S",
                    S = otherValue
                };
                gCommands.Add(g);
            } else {
                otherValue = GetCommandValue("F", code);
                if(!float.IsNaN(otherValue)) {
                    GCommand g = new GCommand {
                        name = "F",
                        F = otherValue
                    };
                    gCommands.Add(g);
                }
            }
        }

        while(startIndex != -1) {

            string name = "G" + GetCommandValue("G", code, startIndex);

            switch(CommandType(name)) {
                case 0: {   //komandy na jedn? klatk?
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
                case 2: {   //ruch ko?owy
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
            if(!char.IsDigit(code[i]) && code[i] != ',' && code[i] != '-') break; // je?eli nie jest cyfr? lub ',' lub '-'
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
        robotMaster.Spindle(false);
        uiManager.UpdateConsole("Zako?czono program");
        robotMaster.UpdateButtons();
    }

    void UpdateTimer() {
        CommandTimeText.text = $"{( Time.time - commandStartTime ).Round(1):0.0#}s";
        ProgramTimeText.text = $"{( Time.time - programStartTime ).Round(1):0.0#}s";
    }

    public void ExportKRL() {
        UpdateGcodeLines();
        GCommandsList = GenerateGCommandsList();
        string krlCode = "";
        foreach(GCommand g in GCommandsList) {
            krlCode += g.ToKRL() + "\n";
        }
        GetComponent<GcodeSaveOpen>().SaveFile(krlCode, "src");
    }
}
public struct GcodeData {
    public string dataType;
    public float dataValue;
}