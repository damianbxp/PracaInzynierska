using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class GCommand
{
    public bool active;
    public bool done;
    public string name;
    public List<string> implementedCommands;

    abstract public void Execute(RobotMaster robot);
    abstract public bool ImplementCheck(string command);
}

public class G0 : GCommand {
    public float X;
    public float Y;
    public float Z;

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

    public override void Execute(RobotMaster robot) {
        active = true;

        Vector3 newPos = new Vector3(( X == float.NaN ? robot.targetPos.x : X ), ( Y == float.NaN ? robot.targetPos.y : Y ), ( Z != float.NaN ? robot.targetPos.z : Z ));
        robot.targetPos = newPos;
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

    public override void Execute(RobotMaster robot) {
        throw new System.NotImplementedException();
    }
}
