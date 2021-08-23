using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationMatrix
{
    public float[,] matix;

    public RotationMatrix() {
        matix = new float[3, 3];
    }

    public void Update(float yaw, float pitch, float roll) {
        matix[0, 0] = Mathf.Cos(yaw) * Mathf.Cos(pitch);
        matix[0, 1] = Mathf.Cos(yaw) * Mathf.Sin(pitch) * Mathf.Sin(roll) - Mathf.Sin(yaw) * Mathf.Cos(roll);
        matix[0, 2] = Mathf.Cos(yaw) * Mathf.Sin(pitch) * Mathf.Cos(roll) + Mathf.Sin(yaw) * Mathf.Sin(roll);

        matix[1, 0] = Mathf.Sin(yaw) * Mathf.Cos(pitch);
        matix[1, 1] = Mathf.Sin(yaw) * Mathf.Sin(pitch) * Mathf.Sin(roll) + Mathf.Cos(yaw) * Mathf.Cos(roll);
        matix[1, 2] = Mathf.Sin(yaw) * Mathf.Sin(pitch) * Mathf.Cos(roll) - Mathf.Cos(yaw) * Mathf.Sin(roll);

        matix[2, 0] = -Mathf.Sin(pitch);
        matix[2, 1] = Mathf.Cos(pitch) * Mathf.Sin(roll);
        matix[2, 2] = Mathf.Cos(pitch) * Mathf.Cos(roll);

        Round();
    }

    public void Round() {
        for(int i = 0; i < 3; i++) {
            for(int j = 0; j < 3; j++) {
                matix[i, j] *= 1000;
                matix[i, j] = Mathf.Round(matix[i, j]);
                matix[i, j] /= 1000;
            }
        }
    }

    public void Update(Vector3 eulerAngles) {
        Update(eulerAngles.x, eulerAngles.y, eulerAngles.z);
    }

    public void UpdateDeg(float yaw, float pitch, float roll) {
        Update(yaw * Mathf.Deg2Rad, pitch * Mathf.Deg2Rad, roll * Mathf.Deg2Rad);
    }
    public void UpdateDeg(Vector3 eulerAngles) {
        UpdateDeg(eulerAngles.x, eulerAngles.y, eulerAngles.z);
    }

    public override string ToString() {
        string returnStr;
        returnStr = "\t[" + matix[0, 0] + "\t" + matix[0, 1] + "\t" + matix[0, 2] + "]\t";
        returnStr += "[" + matix[1, 0] + "\t" + matix[1, 1] + "\t" + matix[1, 2] + "]\t";
        returnStr += "[" + matix[2, 0] + "\t" + matix[2, 1] + "\t" + matix[2, 2] + "]";
        return returnStr;
    }
}
