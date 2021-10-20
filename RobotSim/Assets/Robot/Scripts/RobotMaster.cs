using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMaster : MonoBehaviour
{
    public bool isPaused;
    int currentCommand;
    public List<GCommand> commands;

    public Vector3 targetPos;

    public Vector3 homePointOffset;
    public Transform toolTarget;
    public GcodeInterpreter interpreter;

    public float feedRate;
    public float maxSpeed;
    public bool fastMove;

    public float spindleSpeed;

    public float posPrecision;

    private void Start() {
        isPaused = true;
        targetPos = new Vector3();
        commands = interpreter.LoadCommands();

    }

    void Update()
    {
        if(!isPaused && currentCommand<commands.Count) {
            if(!commands[currentCommand].done && !commands[currentCommand].active) {
                commands[currentCommand].Execute(this);
            }
            if(Vector3.Distance(toolTarget.position, targetPos) < posPrecision) {
                commands[currentCommand].done = true;
                commands[currentCommand].active = false;
                fastMove = false;
                currentCommand++;
            } else {
                if(fastMove) {
                    toolTarget.position = homePointOffset + targetPos/1000;
                } else {

                }
            }
            
        }
    }
}
