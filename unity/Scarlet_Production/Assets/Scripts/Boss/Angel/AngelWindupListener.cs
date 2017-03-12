using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelWindupListener : MonoBehaviour {

    public static string WINDUP_START = "angel_windup_start";
    public static string WINDUP_END = "angel_windup_end";

    public static string LONG_STANCE_START = "angel_long_stance_start";
    public static string LONG_STANCE_END = "angel_long_stance_end";


	public void WindupStart(int which)
    {
        MLog.Log(LogType.AngelLog, "Windup Listener: Windup Start ");
        EventManager.TriggerEvent(WINDUP_START);       
    }

    public void WindupEnd(int which)
    {
        MLog.Log(LogType.AngelLog, "Windup Listener: Windup End ");

        EventManager.TriggerEvent(WINDUP_END);
    }

    public void LongStanceStart(int which)
    {
        MLog.Log(LogType.AngelLog, "Windup Listener: Long Stance Start ");
        EventManager.TriggerEvent(LONG_STANCE_START);
    }

    public void LongStanceEnd(int which)
    {
        MLog.Log(LogType.AngelLog, "Windup Listener: Long Stance End ");
        EventManager.TriggerEvent(LONG_STANCE_END);
    }

}
