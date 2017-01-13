using System.Collections;
using System;
using UnityEngine;

public class MLog : MonoBehaviour {

    // same order in which they appear in LogType
    private static bool[] s_EnabledTypes = {false, true, false, true, false};

    public static void Log(string msg)
    {
        Debug.Log(msg);
    }

    public static void Log(LogType type, string msg)
    {
        if (s_EnabledTypes[(int) type]) Log(msg);
    }

    public static void Log(LogType type, int tabs, string msg)
    {
        string t = "";
        for (int i = 0; i < tabs; i++) t += "\t";

        if (s_EnabledTypes[(int)type]) Log(t + msg);
    }

}

public enum LogType
{
    Verbose, Debug, Warning, Error, BattleLog
}

