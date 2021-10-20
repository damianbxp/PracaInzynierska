using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class GCommand
{
    public bool active;
    public bool done;
    public string name;
    public List<string> implementedCommands;

    abstract public void Execute(Transform toolTarget);
    abstract public bool ImplementCheck(string command);
}

public class G0 : GCommand {
    public float X;
    public float Y;
    public float Z;
    public Vector3 position;
    public Vector3 rotation;

    public Vector3 oldPos;
    public Vector3 oldRot;

    public G0() {
        name = "G0";
        active = false;
        done = false;
        X = float.NaN;
        Y = float.NaN;
        Z = float.NaN;

        implementedCommands = new List<string>();
        implementedCommands.Add("X");
        implementedCommands.Add("Y");
        implementedCommands.Add("Z");
    }

    public override string ToString() {
        return $"{name} X{X} Y{Y} Z{Z} {(active?"Active":"Not Active")} {(done?"Finnished":"Not Finished")}";
    }

    public override void Execute(Transform toolTarget) {
        position.x = ( position.x != float.NaN ) ? position.x : oldPos.x;
        position.y = ( position.y != float.NaN ) ? position.y : oldPos.y;
        position.z = ( position.z != float.NaN ) ? position.z : oldPos.z;

        toolTarget.position = position;
    }

    public override bool ImplementCheck(string command) {
        for(int i = 0; i < implementedCommands.Count; i++) {
            if(command == implementedCommands[i]) {
                return true;
            }
        }
        return false;
    }
}

public class G1 : G0{
    public float F;

    public G1() : base() {
        name = "G1";
        F = float.NaN;
        implementedCommands.Add("F");
    }

    public override void Execute(Transform toolTarget) {
        throw new System.NotImplementedException();
    }
}
