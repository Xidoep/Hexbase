using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Configurador Materials")]
public class ConfiguracioMaterials : ScriptableObject
{
    string pathMaterials = "Assets/XidoStudio/Hexbase/Peces/Tiles/Materials/";
    string pathTextures = "Assets/XidoStudio/Hexbase/Peces/Tiles/Materials/Textures";

    [SerializeField] Material[] materials;
    [SerializeField] Texture[] textures;

    Object[] _tmp;

    [ContextMenu("Configurar")]
    void Configurar()
    {
        Debug.Log("hola");
        _tmp = AssetDatabase.LoadAllAssetRepresentationsAtPath(pathMaterials);

        for (int i = 0; i < _tmp.Length; i++)
        {
            Debug.Log(_tmp[i].name);
        }


        textures = (Texture[])AssetDatabase.LoadAllAssetsAtPath(pathTextures);
    }
}
