using SFB;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GcodeSaveOpen : MonoBehaviour
{
    GcodeInterpreter gcodeInterpreter;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenFile() {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "gcode", false);

        if(paths.Length > 0) {
            string code = File.ReadAllText(paths[0]);
            GameObject.Find("GcodeText").GetComponent<TMPro.TMP_InputField>().text = code;
            Debug.Log("File loaded");
        } else
            Debug.LogError("Open fail - no path found");
    }

    public void SaveFile() {
        string path = StandaloneFileBrowser.SaveFilePanel("Save File", "", "", "gcode");
        if(path.Length > 0) {
            File.WriteAllText(path, GameObject.Find("GcodeText").GetComponent<TMPro.TMP_InputField>().text.ToUpper().Normalize(System.Text.NormalizationForm.FormC));
            Debug.Log("File saved");
        } else
            Debug.LogError("Save fail - no path found");
    }
}
