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

    string[] moveCommands = { "G0", "G1","G2","G3" };

    private void Start() {
        isPaused = true;
        targetPos = new Vector3();
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        UpdateGcodeLines();

    }

    void Update()
    {
        if(!isPaused && currentCommand < gcodeLines.Count) {
            DecryptLine(gcodeLines[currentCommand]);

            currentCommand++;
        }
    }
    void UpdateGcodeLines() {
        string text = GameObject.Find("GcodeText").GetComponent<Text>().text;
        gcodeLines = new List<string>(text.Replace("\r", "").Split('\n'));
    }

    List<GCommand> DecryptLine(string code) {
        code = code.ToUpper().Replace(" ", "");
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

            } else {                                        // komendy na pedn¹ klatkê

            }

            gCommands.Add(gCommand);
            startIndex = code.IndexOf("G", startIndex + 1);
        }


        for(int i = 0; i < gCommands.Count; i++) {
            Debug.Log(gCommands[i]);
        }
        return gCommands;
    }

    float GetCommandValue(string command, string code, int startIndex = 0) {
        if(code.IndexOf(command, startIndex) == -1) return float.NaN;

        int indexStart = code.IndexOf(command, startIndex) + command.Length;
        int indexEnd = 0;
        for(int i = indexStart; i < code.Length; i++) {
            indexEnd = i;
            if(!char.IsDigit(code[i]) && code[i]!='.') break; // je¿eli nie jest cyfr¹ lub '.'
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