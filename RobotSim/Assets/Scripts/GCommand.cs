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

    public float A = float.NaN;
    public float B = float.NaN;
    public float C = float.NaN;
    
    public float F = float.NaN;
    public float S = float.NaN;


    public override string ToString() {
        return $"{name} X{X} Y{Y} Z{Z} A{A} B{B} C{C} F{F} S{S}";
    }
}