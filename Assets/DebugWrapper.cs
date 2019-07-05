
using UnityEngine;



public class DebugWrapper{
    public static void Log(object message) {
        Debug.Log(message+"\r\n"+StackTraceUtility.ExtractStackTrace()+"\r\n\r\n");
    }
    public static void LogWarning(object message) {
        Debug.LogWarning(message);
    }
    public static void LogError(object message) {
        Debug.LogError(message);
    }
}

