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
    public Vector3 rotation = new Vector3(float.NaN, float.NaN, float.NaN);


    public float A = float.NaN;
    public float B = float.NaN;
    public float C = float.NaN;
    
    public float F = float.NaN;
    public float S = float.NaN;

    public GCommand previousCommand;
    public GCommand nextCommand;

    public void UpdateCommand() {// zastêpuje NaN poprzednimi warotœciami
        if(float.IsNaN(X)) X = previousCommand.X;
        if(float.IsNaN(Y)) Y = previousCommand.Y;
        if(float.IsNaN(Z)) Z = previousCommand.Z;

        //if(float.IsNaN(A)) A = previousCommand.A;
        //if(float.IsNaN(B)) B = previousCommand.B;
        //if(float.IsNaN(C)) C = previousCommand.C;

        position = new Vector3(X, Z, Y);
        rotation = new Vector3(A, B, C);
    }

    public override string ToString() {
        return $"{name} X{X} Y{Y} Z{Z} A{A} B{B} C{C} F{F} S{S}";
    }
}