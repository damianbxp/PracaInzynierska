using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GCommand {
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
    //public GCommand nextCommand;

    public virtual void UpdateCommand() {// zastêpuje NaN poprzednimi warotœciami
        if(previousCommand == null)
            previousCommand = new GCommand();
        //if(nextCommand == null)
        //    nextCommand = new GCommand();

        if(float.IsNaN(X)) X = previousCommand.X;
        if(float.IsNaN(Y)) Y = previousCommand.Y;
        if(float.IsNaN(Z)) Z = previousCommand.Z;

        if(float.IsNaN(A)) A = previousCommand.A;
        if(float.IsNaN(B)) B = previousCommand.B;
        if(float.IsNaN(C)) C = previousCommand.C;

        position = new Vector3(X, Z, Y);
        rotation = new Vector3(A, B, C);

        if(float.IsNaN(F)) F = previousCommand.F;
        else F /= 60;
        if(float.IsNaN(S)) S = previousCommand.S;
    }

    public virtual string ToKRL() {
        string returnStr;
        if(name == "G0")
            returnStr = "PTP";
        else
            returnStr = "LIN";
        
        string point = " {" + $"X {X.ToStrDot()}, Y {Y.ToStrDot()}, Z {Z.ToStrDot()}, A {A.ToStrDot()}, B {B.ToStrDot()}, C {C.ToStrDot()}" + "} ";
        returnStr += point;
        if(name == "G1")
            returnStr += $"Vel={F / 1000}m/s";

        return returnStr;
    }

    public override string ToString() {
        return $"{name} X{X} Y{Y} Z{Z} A{A} B{B} C{C} F{F} S{S}";
    }
}

public class SGCommand : GCommand {
    public float I = float.NaN;
    public float J = float.NaN;
    public float K = float.NaN;
    public Vector3 offset = new Vector3(float.NaN, float.NaN, float.NaN);

    public override void UpdateCommand(){
        base.UpdateCommand();

        offset = new Vector3(I, K, J);
    }

    public override string ToKRL() {
        string returnStr = "CIRC";
        string point = " {" + $"X {X.ToStrDot()}, Y {Y.ToStrDot()}, Z {Z.ToStrDot()}, A {A.ToStrDot()}, B {B.ToStrDot()}, C {C.ToStrDot()}" + "} ";
        Vector3 auxPoint = GameObject.Find("RobotMaster").GetComponent<GcodeInterpreter>().GetPointOnCircle(this, 0.5f);
        string auxPointStr = " {" + $"X {auxPoint.x.ToStrDot()}, Y {auxPoint.z.ToStrDot()}, Z {auxPoint.y.ToStrDot()}" + "}";

        returnStr += auxPointStr + point;
        returnStr += $"Vel={F / 1000}m/s";
        return returnStr;
    }

    public override string ToString() {
        return $"{name} X{X} Y{Y} Z{Z} A{A} B{B} C{C} I{I} J{J} K{K} F{F} S{S}";
    }
}