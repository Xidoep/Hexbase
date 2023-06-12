using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

public class EstatsInspector : OdinMenuEditorWindow
{
    [MenuItem("TwinTown/Inspector")]
    protected static void OpenWindow()
    {
        GetWindow<EstatsInspector>().Show();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();
        tree.Config.AutoFocusSearchBar = true;

        tree.AddAllAssetsAtPath("Estats", "Assets/XidoStudio/Hexbase/Peces/Estats", typeof(Estat));

        return tree;
    }
}
