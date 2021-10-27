using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GCommand
{
    public bool done = false;
    public string name = "";

    public float X = float.NaN;
    public float Y = float.NaN;
    public float Z = float.NaN;
    public Vector3 position = new Vector3(float.NaN, float.NaN, float.NaN);


    public float A = float.NaN;
    public float B = float.NaN;
    public float C = float.NaN;
    
    public float F = float.NaN;
    public float S = float.NaN;


    public void UpdateCommand(GCommand lastCommand) {
        if(float.IsNaN(X)) X = lastCommand.X;
        if(float.IsNaN(Y)) Y = lastCommand.Y;
        if(float.IsNaN(Z)) Z = lastCommand.Z;


        position = new Vector3(X, Y, Z);
    }

    public override string ToString() {
        return $"{name} X{X} Y{Y} Z{Z} A{A} B{B} C{C} F{F} S{S}";
    }
}