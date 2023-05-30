using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Unpack : Editor
{
    [MenuItem("Unpack/Editor")]
    static void OpenMenu()
    {
        EditorUtility.OpenPropertyEditor(AssetDatabase.LoadAssetAtPath<EstatsUnpack>("Assets/XidoStudio/Hexbase/Peces/Editor/Estats Unpack.asset"));
        //Editor editor = Editor.CreateEditor(AssetDatabase.LoadAssetAtPath<EstatsUnpack>("Assets/XidoStudio/Hexbase/Peces/Editor/Estats Unpack.asset"));
        //editor.Repaint();
    }
}
