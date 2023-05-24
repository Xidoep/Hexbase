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
    const string TIPUS_SIMPLE = "TS";
    const string TIPUS_OCUPABLE = "TO";
    const string TIPUS_CONDICIONAL = "TC";
    const string NULES = "Nules";
    const string ESPECIFIQUES = "Espes";
    const char SEPARADO = '_';
    const char SEPARADO_INFORMACIO = ':';
    const char IGUAL = '=';
 
    [SerializeField] Object tiles;
    [SerializeField] string outputPath;

    [SerializeField] List<Object> connexions;
    [SerializeField] bool terra;
    [SerializeField] bool casa;
    [SerializeField] Referencies referencies;

    Object[] subobjects;
    string nom;
    string[] nomsConnexions;
    Connexio[] connexionsTile;
    GameObject intance;
    GameObject prefab;
    bool trobat;
    Connexio connexio;
    Tile tile;
    TileSet_Simple TilesetSimple;
    TileSet_Ocupable TilesetOcupable;
    TileSet_Condicional TilesetCondicional;

    bool tilesetCreat = false;

   [ContextMenu("Unpack")]
    void Unpack()
    {
        //connexions = AssetDatabase.LoadAllAssetsAtPath("Assets/XidoStudio/Hexbase/Peces/Connexio");
        //Debug.Log(((GameObject)tiles).GetComponent<MeshFilter>().sharedMesh.name);
        subobjects = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(tiles));

        //
        if (terra) Unpack(TERRA);
        if (casa) Unpack(CASA);
    }

    bool EsGameObject(int i) => subobjects[i].GetType().Equals(typeof(GameObject));
    bool EsPeça(int i, string root) => subobjects[i].name.StartsWith($"_{root}");
    bool EsInfo(int i, string root) => subobjects[i].name.StartsWith($"_{root}-");
    bool EsTipusSimple(int i, string root) => subobjects[i].name.Substring(root.Length + 2).StartsWith(TIPUS_SIMPLE);
    bool EsTipusOcupable(int i, string root) => subobjects[i].name.Substring(root.Length + 2).StartsWith(TIPUS_OCUPABLE);
    bool EsTipusCondicional(int i, string root) => subobjects[i].name.Substring(root.Length + 2).StartsWith(TIPUS_CONDICIONAL);

    bool EsInfoNules(int i, string root) => subobjects[i].name.Substring(root.Length + 4).StartsWith(NULES);
    string[] InfoNules(int i, string root) => subobjects[i].name.Substring(root.Length + 4).Split(SEPARADO_INFORMACIO)[1].Split(SEPARADO);

    bool EsInfoEspecifiques(int i, string root) => subobjects[i].name.Substring(root.Length + 4).StartsWith(ESPECIFIQUES);
    string[] InfoEspecifiquesEstats(int i, string root) => subobjects[i].name.Substring(root.Length + 4).Split(SEPARADO_INFORMACIO)[1].Split(IGUAL)[0].Split(SEPARADO);
    string[] InfoEspecifiquesConnexions(int i, string root) => subobjects[i].name.Substring(root.Length + 4).Split(SEPARADO_INFORMACIO)[1].Split(IGUAL)[1].Split(SEPARADO);

    private void Unpack(string root)
    {
        string debug = "";

        if (!AssetDatabase.IsValidFolder($"{outputPath}/{root}"))
            AssetDatabase.CreateFolder(outputPath, root);
        if (!AssetDatabase.IsValidFolder($"{outputPath}/{root}/Prefab"))
            AssetDatabase.CreateFolder($"{outputPath}/{root}", "Prefab");

        tilesetCreat = false;
        TilesetSimple = null;
        TilesetOcupable = null;
        TilesetCondicional = null;

        for (int i = 0; i < subobjects.Length; i++)
        {
            //Debug.Log($"{subobjects[i].name} | {subobjects[i].GetType().Name}");

            if (!EsGameObject(i))
                continue;

           

            //Crear el Tileset

            if (EsInfo(i, root))
            {
                //Tipus
                if (!tilesetCreat)
                {
                    if (EsTipusSimple(i, root))
                    {
                        TilesetSimple = CreateInstance<TileSet_Simple>();
                        Debug.Log("Crear TileSet Tipus simple");
                    }
                    else if (EsTipusOcupable(i, root))
                    {
                        TilesetOcupable = CreateInstance<TileSet_Ocupable>();
                        Debug.Log("Crear TileSet Tipus ocupable");
                    }
                    else if (EsTipusCondicional(i, root))
                    {
                        TilesetCondicional = CreateInstance<TileSet_Condicional>();
                        Debug.Log("Crear TileSet Tipus condicional");
                    }
                    tilesetCreat = true;
                }

                if(TilesetCondicional == null)
                {
                    if (EsInfoNules(i, root))
                    {
                        debug = "Nules:";
                        string[] nules = InfoNules(i, root);
                        for (int n = 0; n < nules.Length; n++)
                        {
                            debug += $" {nules[n]},";
                        }
                        Debug.Log(debug);
                    }

                    if (EsInfoEspecifiques(i, root))
                    {
                        debug = "Especifiques:";
                        string[] estats = InfoEspecifiquesEstats(i, root);
                        string[] connexions = InfoEspecifiquesConnexions(i, root);
                        for (int e = 0; e < estats.Length; e++)
                        {
                            debug += $" {estats[e]},";
                        }
                        debug += "=";
                        for (int c = 0; c < connexions.Length; c++)
                        {
                            debug += $" {connexions[c]},";
                        }
                        Debug.Log(debug);
                    }
                }
                else
                {
                    //crear tantes condicions com numero trobi despres del tipus.
                    //Si trobo 2, creo 2 condicions, si despres en trobo 3, l'afageixo 1. Si despre en trobo 1, com que hi haurà més condicions, no en crearé cap.

                    //Fer igual que abans, pero seguint l'index que em surt despres del tipus.
                }
                
            }

            continue;

            if (EsPeça(i,root))
            {
                if (subobjects[i].name.Contains('.'))
                    continue;

                //Agafar nomes nets
                string[] pes = subobjects[i].name.Split('$');

                Debug.Log(pes[1]);





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
