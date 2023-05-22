using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;
using UnityEditor;

[CreateAssetMenu(menuName = "Xido Studio/Hex/UnpackTiles")]
public class TilesUnpack : ScriptableObject
{
    const string AUTOGENERATS_PATH = "Assets/XidoStudio/Hexbase/Peces/Connexio/Autogenerats";
    const string TERRA = "Terra";
    const string CASA = "Casa";

    [SerializeField] Object tiles;
    [SerializeField] string outputPath;

    [SerializeField] List<Object> connexions;
    [SerializeField] bool terra;
    [SerializeField] bool casa;

    Object[] subobjects;
    string nom;
    string[] nomsConnexions;
    Connexio[] connexionsTile;
    GameObject intance;
    GameObject prefab;
    bool trobat;
    Connexio connexio;
    Tile tile;

   [ContextMenu("Unpack")]
    void Unpack()
    {
        //connexions = AssetDatabase.LoadAllAssetsAtPath("Assets/XidoStudio/Hexbase/Peces/Connexio");
        //Debug.Log(((GameObject)tiles).GetComponent<MeshFilter>().sharedMesh.name);
        subobjects = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(tiles));

        //
        if(terra) Unpack(TERRA);
        if (casa) Unpack(CASA);
    }

    private void Unpack(string root)
    {
        if (!AssetDatabase.IsValidFolder($"{outputPath}/{root}"))
            AssetDatabase.CreateFolder(outputPath, root);
        if (!AssetDatabase.IsValidFolder($"{outputPath}/{root}/Prefab"))
            AssetDatabase.CreateFolder($"{outputPath}/{root}", "Prefab");


        for (int i = 0; i < subobjects.Length; i++)
        {
            //Debug.Log($"{subobjects[i].name} | {subobjects[i].GetType().Name}");

            if (!subobjects[i].GetType().Equals(typeof(GameObject)))
                continue;

            if (subobjects[i].name.StartsWith($"{root}-"))
            {
                if (subobjects[i].name.Contains('.'))
                    continue;

                //Agafar nomes nets
                string[] invertit = subobjects[i].name.Split("%");
                nom = invertit[0];
                if (nom.Contains('('))
                {
                    string[] first = nom.Split('(');
                    string[] second = first[1].Split(')');
                    nom = $"{first[0]}{second[1]}";
                    Debug.Log(nom);
                }

                //Borrar antic
                AssetDatabase.DeleteAsset($"{outputPath}/{root}/{nom}.asset");
                AssetDatabase.DeleteAsset($"{outputPath}/{root}/Prefab/{nom}.prefab");

                //Crear prefab
                intance = (GameObject)Instantiate(subobjects[i]);
                prefab = PrefabUtility.SaveAsPrefabAsset(intance, $"{outputPath}/{root}/Prefab/{nom}.prefab");
                //if (subobjects[i].name.Contains('('))
                //    prefab.transform.localScale = new Vector3(-1, 1, 1);

                DestroyImmediate(intance);



                //Agafar connexions
                nomsConnexions = nom.Substring(root.Length + 1).Split('_');
                connexionsTile = new Connexio[3];
                for (int nc = 0; nc < 3; nc++)
                {
                    trobat = false;
                    Debug.Log($"{nomsConnexions[nc]}");
                    for (int c = 0; c < connexions.Count; c++)
                    {
                        if (nomsConnexions[nc] == connexions[c].name)
                        {
                            connexionsTile[nc] = connexions[c] as Connexio;
                            trobat = true;
                            break;
                        }
                    }
                    if (!trobat)
                    {
                        connexio = CreateInstance<Connexio>();
                        connexio.name = nomsConnexions[nc];
                        AssetDatabase.CreateAsset(connexio, $"{AUTOGENERATS_PATH}/{connexio.name}.asset");
                        connexionsTile[nc] = AssetDatabase.LoadAssetAtPath<Connexio>($"{AUTOGENERATS_PATH}/{connexio.name}.asset");
                    }
                }

                if(invertit.Length > 1)
                {
                    prefab.transform.localScale = new Vector3(-1, 1, 1);
                }

                //Crear tile
                tile = CreateInstance<Tile>();
                tile.Setup(prefab, connexionsTile);

                //Guardar tile
                AssetDatabase.CreateAsset(tile, $"{outputPath}/{root}/{nom}.asset");

            }
        }
    }

    private void OnValidate()
    {
        connexions = XS_Editor.LoadAllAssetsAtPath<Object>(AUTOGENERATS_PATH);
    }
}
