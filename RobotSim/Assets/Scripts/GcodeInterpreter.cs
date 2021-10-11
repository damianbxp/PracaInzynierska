using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GcodeInterpreter : MonoBehaviour {

    GcodeCommands GcodeCommands;
    public string commandLine;

    private void Start() {
        GcodeCommands = GetComponent<GcodeCommands>();
        ExecuteCommands(DecryptLine(commandLine));
    }

    void ExecuteCommands(List<GcodeData> list) {
        Debug.Log($"Detected {list.Count} elements");
        for(int i = 0; i < list.Count; i++) {
            switch(list[i].dataType) {
                case 'G': {
                    switch(list[i].dataValue) {
                        case 1: {
                            for(int j = 1; j < Mathf.Min(6, list.Count-i); j++) {
                                if(list[i + j].dataType == 'G') {
                                    i += j-1;
                                    break;
                                } else {
                                    G1Data g1Data = new G1Data();
                                    g1Data.x = float.NaN;
                                    g1Data.y = float.NaN;
                                    g1Data.z = float.NaN;
                                    g1Data.f = float.NaN;

                                    switch(list[i + j].dataType) {
                                        case 'X': {
                                            g1Data.x = list[i + j].dataValue;
                                            break;
                                        }
                                        case 'Y': {
                                            g1Data.y = list[i + j].dataValue;
                                            break;
                                        }
                                        case 'Z': {
                                            g1Data.z = list[i + j].dataValue;
                                            break;
                                        }
                                        case 'F': {
                                            g1Data.f = list[i + j].dataValue;
                                            break;
                                        }

                                    }
                                    Debug.Log($"{i}|{j} {list[i + j].dataType} {list[i + j].dataValue}");
                                    
                                }
                            }
                            
                            break;
                        }

                        default:
                            break;
                    }


                    break;
                }
                default:
                    break;
            }
        }
    }

    List<GcodeData> DecryptLine(string code) {
        Debug.Log(code);
        code = code.Trim(' ');

        GcodeData temp = new GcodeData();
        List<GcodeData> codeList = new List<GcodeData>();

        int dataStart = 0;
        int dataEnd = 0;

        for(int i = 0; i < code.Length; i++) {

            if(!char.IsNumber(code[i])) {
                if(dataStart > 0 && dataEnd > 0) {
                    temp.dataValue = float.Parse(code.Substring(dataStart, dataEnd - dataStart));
                }


                if(i > 0) {
                    //Debug.Log(temp.dataType + temp.dataValue.ToString());
                    codeList.Add(temp);
                }
                temp.dataType = code[i];
                dataStart = i + 1;
                dataEnd = dataStart;
            } else {
                dataEnd++;
            }
        }
        temp.dataValue = float.Parse(code.Substring(dataStart, dataEnd - dataStart));
        //Debug.Log(temp.dataType + temp.dataValue.ToString());
        codeList.Add(temp);

        return codeList;
    }

}

public struct GcodeData {
    public char dataType;
    public float dataValue;
}

public struct G1Data {
    public float x;
    public float y;
    public float z;
    public float f;
}
