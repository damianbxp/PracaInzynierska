using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{

    

    [Header("Axis")]
    public float a;
    [SerializeField]
    public List<RobotAxis> axes = new List<RobotAxis>();

 
}

public class RobotAxis {
    public enum Axes { X, Y, Z };

    public Axes rotAxis = Axes.Y;
    public float offset = 0;
}