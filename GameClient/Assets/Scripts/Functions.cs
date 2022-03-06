using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Functions
{

    public static Vector3 StringToVector3(string sVector)
    {
        // stolen from: https://answers.unity.com/questions/1134997/string-to-vector3.html
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }
        // split the items
        string[] sArray = sVector.Split(',');
        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2])
        );
        return result;
    }

    public static string GenUUID()
    {
        return System.Guid.NewGuid().ToString();
    }

}
