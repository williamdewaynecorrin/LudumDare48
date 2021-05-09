using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using int8 = System.Byte;

public class FlagsPropertyDrawer<EnumType> : PropertyDrawer
{
    protected string dropdowndisplayname = "Flag Values";

    bool[] flagsvalues = new bool[8];
    int extralines = 1;
    bool cachedfoldout = false;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        Flags8<EnumType> flagsobject = fieldInfo.GetValue(property.serializedObject.targetObject) as Flags8<EnumType>;

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        SerializedProperty flags = property.FindPropertyRelative("flags");
        SerializedProperty foldout = property.FindPropertyRelative("foldout");
        bool foldoutval = foldout.boolValue;

        Rect baserect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        SetDropdownDisplayName();
        foldoutval = EditorGUI.Foldout(baserect, foldout.boolValue, dropdowndisplayname);
        baserect.y += 20f;

        // -- only show flag values if foldout variable is true
        if (foldoutval)
        {
            EditorGUI.indentLevel = 1;
            int8 base10 = (int8)flags.intValue;
            for (int8 i = 0; i < flagsvalues.Length; ++i)
            {
                bool flagenabled = Flags8<EnumType>.BitOn((int8)base10, i);
                string flaglabel = flagsobject.EnumValueAt(i).ToString();
                if (EditorGUI.Toggle(baserect, flaglabel, flagenabled))
                    flagsobject.SetFlag(i);
                else
                    flagsobject.ClearFlag(i);

                baserect.y += 20f;
            }

            flags.intValue = flagsobject.flags;
        }

        // -- set serialized editor properties
        foldout.boolValue = foldoutval;
        cachedfoldout = foldoutval;

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float foldoutsize = EditorGUIUtility.singleLineHeight * flagsvalues.Length +
                            flagsvalues.Length * 2 + EditorGUIUtility.singleLineHeight / 2f;

        float regularsize = (20 - EditorGUIUtility.singleLineHeight) + 
                            EditorGUIUtility.singleLineHeight * extralines + 
                            extralines * 2;

        float fullsize = regularsize;
        if (cachedfoldout)
            fullsize += foldoutsize;

        return fullsize;
    }

    protected virtual void SetDropdownDisplayName()
    {
        dropdowndisplayname = "Flag Values";
    }
}
