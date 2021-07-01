using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Robot))]
public class RobotInspector : Editor
{
    bool showAxis;

    public override void OnInspectorGUI() {
        EditorGUILayout.LabelField("asdasd");
        Robot robot = target as Robot;

        if(GUILayout.Button("Load XML")) robot.loadXMLData();
        if(GUILayout.Button("Save XML")) robot.saveXML();

        showAxis = EditorGUILayout.Foldout(showAxis, "Axis");
        if(showAxis) {
            using(new GUILayout.VerticalScope(EditorStyles.helpBox)) {
                for(int i = 0; i < robot.axes.Count; i++){
                    using( new GUILayout.VerticalScope(EditorStyles.helpBox)) {
                        Robot.Axis axis = robot.axes[i];
                        EditorGUILayout.LabelField(i.ToString());

                        axis.offset = EditorGUILayout.FloatField("Offset" , axis.offset);
                        EditorGUILayout.Space(6);

                        axis.minAngle = EditorGUILayout.FloatField("Min Angle", axis.minAngle);
                        axis.maxAngle = EditorGUILayout.FloatField("Max Angle", axis.maxAngle);
                    }
                }
            }
        }

    }
}
