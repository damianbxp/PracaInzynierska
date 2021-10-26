using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GcodeInterpreter : MonoBehaviour {

    public string commandLine;
    public List<GCommand> commands;

    private void Start() {
        //for(int i = 0; i < commands.Count; i++) {
        //    Debug.Log(commands[i]);
        //}
    }

    //public List<GCommand> LoadCommands() {
    //    commands = GenerateCommands(DecryptLine(commandLine));
    //    return commands;
    //}

    //List<GCommand> GenerateCommands(List<GcodeData> list) {
    //    Debug.Log($"Detected {list.Count} elements");
    //    List<GCommand> commands = new List<GCommand>();

    //    for(int i = 0; i < list.Count; i++) {
    //        switch(list[i].dataType) {
    //            case "G": {
    //                switch(list[i].dataValue) {
    //                    case 0: {
    //                        G0 g0 = new G0(GetLastMoveCommad(commands));
    //                        for(int j = 1; j < list.Count - i; j++) {
    //                            if(list[i + j].dataType == "G") break;
    //                            if(!g0.ImplementCheck(list[i + j].dataType)) { Debug.LogWarning($"Not implemented Command - {list[i+j].dataType}{list[i + j].dataValue}"); break; }
    //                            g0.GetType().GetField(list[i+j].dataType.ToString()).SetValue(g0, list[i + j].dataValue);
    //                        }
    //                        commands.Add(g0);
    //                        break;
    //                    }
    //                    case 1: {
    //                        G1 g1 = new G1();
    //                        for(int j = 1; j < list.Count - i; j++) {
    //                            if(list[i + j].dataType == "G") break;
    //                            if(!g1.ImplementCheck(list[i + j].dataType)) { Debug.LogWarning($"Not implemented Command - {list[i + j].dataType}{list[i + j].dataValue}"); break; }
    //                            g1.GetType().GetField(list[i+j].dataType.ToString()).SetValue(g1, list[i + j].dataValue);
    //                        }
    //                        commands.Add(g1);
    //                        break;
    //                    }
    //                }

    //                break;
    //            }
    //            default:
    //                break;
    //        }
    //    }
    //    return commands;
    //}

    List<GcodeData> DecryptLine(string code) {
        Debug.Log(code);
        code = code.Trim(' ');
        code = code.ToUpper();

        GcodeData temp = new GcodeData();
        List<GcodeData> codeList = new List<GcodeData>();

        int dataStart = 0;
        int dataEnd = 0;

        for(int i = 0; i < code.Length; i++) {

            if(!char.IsNumber(code[i])) {// is letter
                if(dataStart > 0 && dataEnd > 0) {
                    temp.dataValue = float.Parse(code.Substring(dataStart, dataEnd - dataStart));
                }


                if(i > 0) {
                    //Debug.Log(temp.dataType + temp.dataValue.ToString());
                    codeList.Add(temp);
                }
                temp.dataType = code[i].ToString();
                dataStart = i + 1;
                dataEnd = dataStart;
            } else { // is number
                dataEnd++;
            }
        }
        temp.dataValue = float.Parse(code.Substring(dataStart, dataEnd - dataStart));
        //Debug.Log(temp.dataType + temp.dataValue.ToString());
        codeList.Add(temp);

        return codeList;
    }

    //GCommand GetLastMoveCommad(List<GCommand> commands) {
    //    for(int i = commands.Count; i >= 0; i--) {
    //        if(commands[i].isMoveCommand == true) return commands[i];
    //    }
    //    return null;
    //}
}

//public struct GcodeData {
//    public string dataType;
//    public float dataValue;
//}
