using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class ExtensionMethods {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="vector3">Wektor do zaokr¹glenia</param>
    /// <param name="decimalPlaces">Liczba cyfr po przecinku</param>
    /// <returns>Zaokr¹glony wektor</returns>
    public static Vector3 Round(this Vector3 vector3, int decimalPlaces = 2) {
        float multiplier = 1;
        for(int i = 0; i < decimalPlaces; i++) {
            multiplier *= 10f;
        }
        return new Vector3(
            Mathf.Round(vector3.x * multiplier) / multiplier,
            Mathf.Round(vector3.y * multiplier) / multiplier,
            Mathf.Round(vector3.z * multiplier) / multiplier);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="x">Float do zaokr¹glenia</param>
    /// <param name="decimalPlaces">Liczba cyfr po przecinku</param>
    /// <returns>Zaokr¹glony float</returns>
    public static float Round(this float x, int decimalPlaces = 2) {
        float multiplier = 1;
        for(int i = 0; i < decimalPlaces; i++) {
            multiplier *= 10f;
        }

        return Mathf.Round(x * multiplier) / multiplier;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="x">Float do przekonwerowania</param>
    /// <returns>String floata z "." jako separatorem dziesiêtnym</returns>
    public static string ToStrDot(this float x) {
        return x.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }
}
