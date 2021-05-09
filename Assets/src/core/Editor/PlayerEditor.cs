using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor
{
    //Player player;
    //Editor shapeeditor;
    //Editor coloreditor;
    //MaterialEditor colormaterialeditor;
    //SerializedObject colorsettings;

    //public override void OnInspectorGUI()
    //{
    //    bool checkchanged = false;

    //    using (var check = new EditorGUI.ChangeCheckScope())
    //    {
    //        base.OnInspectorGUI();
    //        if (check.changed)
    //            player.GeneratePlanet();

    //        checkchanged = check.changed;
    //    }

    //    if (GUILayout.Button("Generate Planet"))
    //    {
    //        player.GeneratePlanet();
    //    }

    //    DrawSettingsEditor(player.shapesettings, ref player.shapesettingsfoldout, player.OnShapeSettingsUpdate, ref shapeeditor);
    //    DrawSettingsEditor(player.colorsettings, ref player.colorsettingsfoldout, player.OnColorSettingsUpdated, ref coloreditor);

    //    // -- draw material editor
    //    EditorGUILayout.PropertyField(colorsettings.FindProperty("planetmaterial"));
    //    if(checkchanged)
    //    {
    //        serializedObject.ApplyModifiedProperties();
    //        if (colormaterialeditor != null)
    //            DestroyImmediate(colormaterialeditor);

    //        if (player.colorsettings.planetmaterial != null)
    //            colormaterialeditor = (MaterialEditor)CreateEditor(player.colorsettings.planetmaterial);
    //    }

    //    if (player.colorsettings.planetmaterial != null)
    //    {
    //        colormaterialeditor.DrawHeader();
    //        bool isdefaultmat = !AssetDatabase.GetAssetPath(player.colorsettings.planetmaterial).StartsWith("Assets");
    //        using (new EditorGUI.DisabledGroupScope(isdefaultmat))
    //        {
    //            colormaterialeditor.OnInspectorGUI();
    //        }
    //    }
    //}

    //void DrawSettingsEditor(Object settings, ref bool foldout, System.Action onsettingsupdated, ref Editor editor)
    //{
    //    if (settings == null)
    //        return;

    //    foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
    //    using (var check = new EditorGUI.ChangeCheckScope())
    //    {
    //        if (foldout)
    //        {
    //            if (editor == colormaterialeditor)
    //            {
    //                CreateCachedEditor(settings, typeof(MaterialEditor), ref editor);
    //                colormaterialeditor.DrawDefaultInspector();
    //            }
    //            else
    //                CreateCachedEditor(settings, null, ref editor);

    //            editor.OnInspectorGUI();

    //            if (check.changed)
    //            {
    //                if (onsettingsupdated != null)
    //                {
    //                    onsettingsupdated();
    //                }
    //            }
    //        }
    //    }
    //}

    //private void OnEnable()
    //{
    //    player = target as Player;
    //    colorsettings = new SerializedObject(player.colorsettings);

    //    if (player.colorsettings.planetmaterial != null)
    //        colormaterialeditor = (MaterialEditor)CreateEditor(player.colorsettings.planetmaterial);
    //}
}
