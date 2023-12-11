using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Configurador Materials")]
public class ConfiguracioMaterials : ScriptableObject
{
    [System.Serializable]
    public class Settings
    {
        public Texture2D textura;
        [SerializeField] [Range(-1, 1)] float rang;
        [SerializeField] [Range(0, 5)] float multplicador;
        [SerializeField] bool invertit;
        [SerializeField] [Range(0,5)] float uv;
        [SerializeField] [Range(0, 0.1f)] float variacio;

        public bool Iguals(Material material, int index) => material.GetTexture($"_{(index > 0 ? index : "")}RGB") == textura;

        public void Setup(Material material, int index)
        {
            material.SetFloat($"_{(index > 0 ? index : "")}UV", uv);
            material.SetFloat($"_{(index > 0 ? index : "")}Variacio", variacio);

            if (index == 0)
                return;

            material.SetFloat($"_{index}H_Multiply", multplicador);

            if (index == 1)
                return;

            material.SetFloat($"_{index - 1}_{index}Rang", rang);
            material.SetFloat($"_{index - 1}_{index}Inverted", invertit ? 1 : 0);
        }
    }



    const string pathMaterials = "Assets/XidoStudio/Hexbase/Peces/Tiles/Materials/";
    const string pathTextures = "Assets/XidoStudio/Hexbase/Peces/Tiles/Materials/Textures";

    [SerializeField] List<Settings> settings;
    
    [SerializeField] Shader simple;
    [SerializeField] Shader R;
    [SerializeField] Shader RG;
    [SerializeField] Shader RGB;
    
    
    [SerializeField] Material[] materials;
    [SerializeField] Texture2D[] textures;



    [ContextMenu("Configurar")]
    void Configurar()
    {
        Debug.Log("hola");

        materials = XS_Editor.LoadAllAssetsAtPath<Material>(pathMaterials).ToArray();
        textures = XS_Editor.LoadAllAssetsAtPath<Texture2D>(pathTextures).ToArray();

        for (int m = 0; m < materials.Length; m++)
        {
            Debug.Log($"{materials[m].name} - {materials[m].shader.name}");


            if(materials[m].shader == simple)
            {
                for (int s = 0; s < settings.Count; s++)
                {
                    if(settings[s].Iguals(materials[m], 0))
                    {
                        settings[s].Setup(materials[m], 0);
                        return;
                    }
                }
            }
            else if(materials[m].shader == R)
            {
                for (int s = 0; s < settings.Count; s++)
                {
                    if (settings[s].Iguals(materials[m], 1))
                    {
                        settings[s].Setup(materials[m], 1);
                    }
                    else if (settings[s].Iguals(materials[m], 2))
                    {
                        settings[s].Setup(materials[m], 2);
                    }
                }
            }
            else if (materials[m].shader == RG)
            {
                for (int s = 0; s < settings.Count; s++)
                {
                    if (settings[s].Iguals(materials[m], 1))
                    {
                        settings[s].Setup(materials[m], 1);
                    }
                    else if (settings[s].Iguals(materials[m], 2))
                    {
                        settings[s].Setup(materials[m], 2);
                    }
                    else if (settings[s].Iguals(materials[m], 3))
                    {
                        settings[s].Setup(materials[m], 3);
                    }
                }
            }
            else if (materials[m].shader == RGB)
            {
                for (int s = 0; s < settings.Count; s++)
                {
                    if (settings[s].Iguals(materials[m], 1))
                    {
                        settings[s].Setup(materials[m], 1);
                    }
                    else if (settings[s].Iguals(materials[m], 2))
                    {
                        settings[s].Setup(materials[m], 2);
                    }
                    else if (settings[s].Iguals(materials[m], 3))
                    {
                        settings[s].Setup(materials[m], 3);
                    }
                    else if (settings[s].Iguals(materials[m], 4))
                    {
                        settings[s].Setup(materials[m], 4);
                    }
                }
            }
        }
        /*_tmp = AssetDatabase.LoadAllAssetRepresentationsAtPath(pathMaterials);

        for (int i = 0; i < _tmp.Length; i++)
        {
            Debug.Log(_tmp[i].name);
        }


        textures = (Texture2D[])AssetDatabase.LoadAllAssetsAtPath(pathTextures);*/
    }
}
