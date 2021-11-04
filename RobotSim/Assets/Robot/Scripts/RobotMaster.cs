using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotMaster : MonoBehaviour
{
    public bool isPaused;
    public float posPrecision = 0.01f;
    public UIManager uiManager;
    int currentLine;
    int currentCommand = 0;

    public Transform toolTarget;
    public Transform tool;

    public Vector3 homePoint;

    List<string> gcodeLines;
    List<GCommand> GCommandsLine = new List<GCommand>();

    GCommand lastCommand = new GCommand();

    float interpolation;

    string[] moveCommands = { "G0", "G1","G2","G3" };

    private void Start() {
        isPaused = true;
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        UpdateGcodeLines();

        lastCommand.position = new Vector3();
        lastCommand.X = 0;
        lastCommand.Y = 0;
        lastCommand.Z = 0;
    }

    void Update()
    {
        //uiManager.UpdateToolPosition((tool.position-homePoint)*1000);

        if(!isPaused && currentLine < gcodeLines.Count) {
            if(currentCommand >= GCommandsLine.Count) { // je¿eli zrobi³ wszystkie komendy w lini
                GCommandsLine = DecryptLine(gcodeLines[currentLine]);
                //Debug.Log($"Commands in line {currentLine}: {GCommandsLine.Count}");
                currentCommand = 0;
                uiManager.UpdateConsole(gcodeLines[currentLine]);
            } else {
                Debug.Log($"Running {currentCommand+1}/{GCommandsLine.Count} in line {currentLine+1}: {GCommandsLine[currentCommand].name} X{GCommandsLine[currentCommand].X} Y{GCommandsLine[currentCommand].Y} Z{GCommandsLine[currentCommand].Z}");

                switch(GCommandsLine[currentCommand].name) { //wykonaj komende
                    case "G0": {
                        GCommandsLine[currentCommand].UpdateCommand(lastCommand);
                        SetToolTarget(GCommandsLine[currentCommand].position);

                        if(Vector3.Distance(toolTarget.position, tool.position)<= posPrecision) GCommandsLine[currentCommand].done = true;
                        break;
                    }
                    case "G1": {//nie dzia³a
                        GCommandsLine[currentCommand].UpdateCommand(lastCommand);
                        LerpToolTarget(GCommandsLine[currentCommand].position);

                        if(Vector3.Distance(toolTarget.position, tool.position) <= posPrecision) {
                            GCommandsLine[currentCommand].done = true;
                            interpolation = 0;
                        }
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

    void SetToolTarget(Vector3 pos) {
        toolTarget.position = pos / 1000 + homePoint;
    }

    void LerpToolTarget(Vector3 pos) {
        toolTarget.position = Vector3.Lerp(toolTarget.position, pos / 1000 + homePoint, interpolation);
        interpolation += 0.1f * Time.deltaTime;
        Debug.Log(interpolation);
    }

    void UpdateGcodeLines() {
        string text = GameObject.Find("GcodeText").GetComponent<TMPro.TextMeshProUGUI>().text;
        Debug.Log(text);
        gcodeLines = new List<string>(text.Replace("\r", "").Split('\n'));
    }

    List<GCommand> DecryptLine(string code) {
        code = code.ToUpper().Replace(" ", "").Replace(".",",");
        code += ";";
        List<GCommand> gCommands = new List<GCommand>();
        GCommand gCommand;
        int startIndex = code.IndexOf("G");

        while(startIndex !=-1) {
            gCommand = new GCommand();

            gCommand.name = "G" + GetCommandValue("G", code, startIndex);
            if(IsMoveCommand(gCommand.name)) {              // komendy trwaj¹ce wiele klatek
                gCommand.X = GetCommandValue("X", code, startIndex);
                gCommand.Y = GetCommandValue("Y", code, startIndex);
                gCommand.Z = GetCommandValue("Z", code, startIndex);

            } else {                                        // komendy na jedn¹ klatkê

            }

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
            if(!char.IsDigit(code[i]) && code[i]!=',' && code[i] != '-') break; // je¿eli nie jest cyfr¹ lub ',' lub '-'
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

}
public struct GcodeData {
    public string dataType;
    public float dataValue;
}