using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;
using UnityEditor;

[CreateAssetMenu(menuName = "Xido Studio/Hex/UnpackTiles")]
public class TilesUnpack : ScriptableObject
{
    const string TERRA = "Terra";
    const string CASA = "Casa";
    const string RIU = "Riu";

    const string AUTOGENERATS_PATH = "Assets/XidoStudio/Hexbase/Peces/Connexio/Autogenerats";
    const char SEPARADO = '_';
    const char PES = '$';
    const char INVERTIT = '%';
    const char CONDICIO = 'C';
    const char VARIACIO = '#';


    [SerializeField] Referencies referencies;
    [SerializeField] Object tiles;
    [SerializeField] string outputPath;

    [Space(20)]
    [SerializeField] bool terra;
    [SerializeField] bool casa;
    [SerializeField] bool riu;

    [Apartat("Auto-configurable")]
   // [SerializeField] List<Object> connexions;



    //INTERN
    Object[] subobjects;
    string nom;
    bool invertit;
    string[] nomsConnexions;
    Connexio[] connexionsTile;
    GameObject intance;
    GameObject prefab;
    bool trobat;
    Connexio connexio;
    Tile tile;
    int variacio = 0;
    int pes;
    int condicio;
    TileSetBase tileset;




    string Path_Asset(string root) => $"{outputPath}/{root}/{nom}.asset";
    string Path_AssetPrefab(string root) => $"{outputPath}/{root}/Prefab/{nom}{(variacio != 0 ? $"_{variacio}" : "")}.prefab";
    //string Path_AssetPrefab(string root) => $"{outputPath}/{root}/Prefab/{nom}.prefab";





    [ContextMenu("Unpack")]
    void Unpack()
    {
        subobjects = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(tiles));

        if (terra) Unpack(TERRA);
        if (casa) Unpack(CASA);
        if (riu) Unpack(RIU);
    }




    private void Unpack(string root)
    {
        NetejarTileset(root);

        for (int i = 0; i < subobjects.Length; i++)
        {
            if (!EsGameObject(i))
                continue;

            if (!EsPeça(i, root))
                continue;

            if (subobjects[i].name.Contains('.'))
                continue;

            Debug.Log(subobjects[i].name);

            GetVariacio(i);
            GetPes(i);
            GetInvertit(i);


            EliminarAssetAntic(root);

            CrearPrefab(root, i);
            CrearConnexions(root);
            CrearTile(root);

            AddTileToTileset(root);
        }

    }

    void NetejarTileset(string root)
    {
        tileset = AssetDatabase.LoadAssetAtPath<TileSetBase>($"{outputPath}/{root}/{root}.asset");

        if (tileset == null)
        {
            Debug.LogError("No hi ha tilesset creat!");
            return;
        }

        if (tileset is TileSet_Simple)
        {
            ((TileSet_Simple)tileset).TileSet.NetejarTiles();
        }
        else if (tileset is TileSet_Ocupable)
        {
            ((TileSet_Ocupable)tileset).TileSetLliure.NetejarTiles();
            ((TileSet_Ocupable)tileset).TileSetOcupat.NetejarTiles();
        }
        else if (tileset is TileSet_Condicional)
        {
            TileSet_Condicional.Condicio[] condicions = ((TileSet_Condicional)tileset).Condicions;
            for (int i = 0; i < condicions.Length; i++)
            {
                condicions[i].TileSet.NetejarTiles();
            }
        }
    }
    bool EsGameObject(int i) => subobjects[i].GetType().Equals(typeof(GameObject));
    bool EsPeça(int i, string root) => subobjects[i].name.StartsWith($"{root}-");

    void GetVariacio(int i)
    {
        variacio = 0;
        if (!subobjects[i].name.Contains(VARIACIO))
            return;

        int.TryParse(subobjects[i].name.Split(VARIACIO)[1], out variacio);
    }
    void GetPes(int i)
    {
        pes = -1;
        condicio = -1;

        string[] tmp = subobjects[i].name.Split(PES);
        nom = tmp[0];

        //if (tmp.Length < 2)
        //    return;

        if (!tmp[1].Contains(CONDICIO))
        {
            int.TryParse(subobjects[i].name.Split(PES)[1], out pes);
            return;
        }

        string[] tmpPes = subobjects[i].name.Split(PES)[1].Split(CONDICIO);
        int.TryParse(tmpPes[0], out pes);
        int.TryParse(tmpPes[1], out condicio);
    }
    void GetInvertit(int i)
    {
        string[] tmp = nom.Split(INVERTIT);

        invertit = tmp.Length > 1;
        nom = tmp[0];

        /*
        if (!nom.Contains('('))
            return;

        string[] first = nom.Split('(');
        string[] second = first[1].Split(')');
        nom = $"{first[0]}{second[1]}";
        */
        Debug.Log(nom);
    }



    void EliminarAssetAntic(string root)
    {
        AssetDatabase.DeleteAsset($"{outputPath}/{root}/{nom}.asset");
        AssetDatabase.DeleteAsset($"{outputPath}/{root}/Prefab/{nom}.prefab");
    }



    void CrearPrefab(string root, int i)
    {
        intance = (GameObject)Instantiate(subobjects[i]);

        if(invertit)
            intance.transform.localScale = new Vector3(-1, 1, 1);

        Debug.Log("Potser aqui es podrien afegir els efectes i elements extres i logiques");
        //AssetDatabase.CreateAsset(intance, Path_AssetPrefab(root));
        prefab = PrefabUtility.SaveAsPrefabAsset(intance, Path_AssetPrefab(root));
        //prefab = PrefabUtility.SaveAsPrefabAsset(intance, $"{outputPath}/{root}/Prefab/{nom}.asset".Replace("/",@"\"));
        //prefab = PrefabUtility.SaveAsPrefabAsset(intance, $"Assets/XidoStudio/Hexbase/Peces/Tiles/Proves/{nom}.asset");
        DestroyImmediate(intance);
    }
    void CrearConnexions(string root)
    {
        nomsConnexions = nom.Substring(root.Length + 1).Split(SEPARADO);
        connexionsTile = new Connexio[3];
        for (int nc = 0; nc < 3; nc++)
        {
            trobat = false;
            Debug.Log($"{nomsConnexions[nc]}");
            if (nomsConnexions[nc].Contains(PES))
            {
                nomsConnexions[nc] = nomsConnexions[nc].Split(PES)[0];
            }
            for (int c = 0; c < referencies.Connexions.Length; c++)
            {
                if (nomsConnexions[nc] == referencies.Connexions[c].name)
                {
                    connexionsTile[nc] = referencies.Connexions[c] as Connexio;
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

                referencies.Refrex();
            }
        }
    }
    private void CrearTile(string root)
    {
        tile = CreateInstance<Tile>();
        tile.Setup(prefab, connexionsTile);

        AssetDatabase.CreateAsset(tile, Path_Asset(root));
    }



    void AddTileToTileset(string root)
    {
        if (pes == -1)
        {
            Debug.LogError("No hi ha pes!");
            return;
        }


        tileset = AssetDatabase.LoadAssetAtPath<TileSetBase>($"{outputPath}/{root}/{root}.asset");

        if (tileset == null)
        {
            Debug.LogError("No hi ha tilesset creat!");
            return;
        }

        if (tileset is TileSet_Simple)
        {
            ((TileSet_Simple)tileset).TileSet.AddTile(tile, pes);
        }
        else if (tileset is TileSet_Ocupable)
        {
            ((TileSet_Ocupable)tileset).TileSetLliure.AddTile(tile, pes);
            ((TileSet_Ocupable)tileset).TileSetOcupat.AddTile(tile, pes);
        }
        else if (tileset is TileSet_Condicional)
        {
            ((TileSet_Condicional)tileset).Condicions[condicio].TileSet.AddTile(tile, pes);
        }
    }
    
}
