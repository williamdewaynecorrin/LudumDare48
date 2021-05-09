using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Assert
{
    public static void Assert_(bool condition, string failmessage)
    {
        if (!condition)
            DebugXT.LogError(failmessage);
    }

    public static void Assert_(bool condition, string failmessage, object arg1)
    {
        if (!condition)
            DebugXT.LogError(failmessage, arg1);
    }

    public static void Assert_(bool condition, string failmessage, object arg1, object arg2)
    {
        if (!condition)
            DebugXT.LogError(failmessage, arg1, arg2);
    }

    public static void Assert_(bool condition, string failmessage, object arg1, object arg2, object arg3)
    {
        if (!condition)
            DebugXT.LogError(failmessage, arg1, arg2, arg3);
    }

    public static void Assert_(bool condition, string failmessage, object arg1, object arg2, object arg3, object arg4)
    {
        if (!condition)
            DebugXT.LogError(failmessage, arg1, arg2, arg3, arg4);
    }

    public static void Assert_(bool condition, string failmessage, object arg1, object arg2, object arg3, object arg4, object arg5)
    {
        if (!condition)
            DebugXT.LogError(failmessage, arg1, arg2, arg3, arg4, arg5);
    }
}
