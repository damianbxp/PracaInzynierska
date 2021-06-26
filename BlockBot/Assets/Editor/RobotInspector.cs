using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Robot))]
public class RobotInspector : Editor
{


    public override void OnInspectorGUI() {
        EditorGUILayout.LabelField("asdasd");
        Robot robot = target as Robot;

        if(GUILayout.Button("RESET AXIS")) GenerateAxes(robot);
        

        foreach(RobotAxis robotAxis in robot.axes) {
            robotAxis.rotAxis = (RobotAxis.Axes)EditorGUILayout.EnumPopup("AXIS", robotAxis.rotAxis);
        }

    }

    void GenerateAxes(Robot robot) {
        robot.axes.Clear();

        for(int i = 0; i < 6; i++) {
            robot.axes.Add(new RobotAxis());
        }
    }
}
