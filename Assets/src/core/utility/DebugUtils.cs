using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugUtils
{
    public static bool enabled = false;

    public static void CheckUtilsEnable(XboxController controller)
    {
        bool controllerheld = controller.GetStartButton(ButtonQuery.Pressed) && controller.GetSelectButton(ButtonQuery.Down);
        bool keyboardheld = Input.GetKeyDown(KeyCode.Tab);

        if (controllerheld || keyboardheld)
            enabled = !enabled;
    }

    public static void GUIText(Rect baserect, ref float yoffset, float yseperation, string text)
    {
        GUIText(baserect, ref yoffset, yseperation, text, "", "", "");
    }

    public static void GUIText(Rect baserect, ref float yoffset, float yseperation, string text, object arg1)
    {   
        GUIText(baserect, ref yoffset, yseperation, text, arg1, "", "");
    }

    public static void GUIText(Rect baserect, ref float yoffset, float yseperation, string text, object arg1, object arg2)
    {
        GUIText(baserect, ref yoffset, yseperation, text, arg1, arg2, "");
    }

    public static void GUIText(Rect baserect, ref float yoffset, float yseperation, string text, object arg1, object arg2, object arg3)
    {
        GUI.Label(new Rect(baserect.x, baserect.y + yoffset, baserect.width, baserect.height),
                  string.Format(text, arg1.ToString(), arg2.ToString(), arg3.ToString()));
        yoffset += yseperation;
    }
}
