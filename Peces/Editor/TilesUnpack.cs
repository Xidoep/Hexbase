using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;
using UnityEditor;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Xido Studio/Hex/UnpackTiles")]
public class TilesUnpack : ScriptableObject
{
    const string PREFAB = "Prefab";

    const string AUTOGENERATS_PATH = "Assets/XidoStudio/Hexbase/Peces/Connexio/Autogenerats";
    const char SEPARADO = '_';
    const char PES = '$';
    const char INVERTIT = '%';
    const char CONDICIO = 'C';
    const char VARIACIO = '#';



    //[SerializeField] Object tiles;
    [SerializeField] AnimacioPerCodi_GameObject_Referencia outline;

    //[Apartat("Auto-configurable")]
   // [SerializeField] List<Object> connexions;



    //INTERN
    Object[] subobjects;
    string outputPath;
    Referencies referencies;

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



    string Main(string root) => $"{outputPath}/{root}";
    string Path_Estat(string root) => $"{Main(root)}/{root.ToUpper()}.asset";
    string Path_Tile(string root) => $"{Main(root)}/Tiles/{nom}.asset";
    string Path_Tileset(string root) => $"{Main(root)}/Tiles/{root}.asset";
    string Path_Connexio(string nom) => $"{AUTOGENERATS_PATH}/{nom}.asset";
    string Path_PrefabTile(string root) => $"{Main(root)}/Tiles/Prefabs/{nom}{(variacio != 0 ? $"_{variacio}" : "")}.prefab";
    //string Path_AssetPrefab(string root) => $"{outputPath}/{root}/Prefab/{nom}.prefab";
    string Path_Colocable(string root) => $"{Main(root)}/Colocable/{root}.asset";
    string Path_PrefabEstat(string root) => $"{Main(root)}/Prefab/{root}.prefab";



    public void Unpack(string root, Object[] subobjects, string outputPath, Referencies referencies)
    {
        this.subobjects = subobjects;
        this.outputPath = outputPath;
        this.referencies = referencies;

        NetejarTileset(root);

        EliminarAntics(root, outputPath);

        for (int i = 0; i < subobjects.Length; i++)
        {
            if (!EsGameObject(i))
                continue;

            //FOR DEBUGING
            /*if (EsPrefab(i, root))
                CrearPrefabEstat(i, root);

            continue;*/

            if (!EsPeça(i, root))
            {
                if (!EsPrefab(i, root))
                    continue;

                CrearPrefabEstat(i, root);
                continue;
            }

            if (subobjects[i].name.Contains('.'))
                continue;

            Debug.Log($"Tot el nom = {subobjects[i].name}");

            GetVariacio(i);
            GetPes(i);
            GetInvertit(i);

            //EliminarAssetAntic(root);

            if (TileJaExisteix(root))
                continue;

            CrearPrefabTile(i, root);
            CrearConnexions(root);
            CrearTile(root);

            AddTileToTileset(root);

            Debug.Log("---------------------");
        }

    }


    void NetejarTileset(string root)
    {
        tileset = AssetDatabase.LoadAssetAtPath<TileSetBase>($"{outputPath}/{root}/Tiles/{root}.asset");

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
    void EliminarAntics(string root, string outputPath)
    {
        List<Tile> oldTiles = XS_Editor.LoadAllAssetsAtPath<Tile>($"{outputPath}/{root}/Tiles");
        for (int i = 0; i < oldTiles.Count; i++)
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(oldTiles[i]));
        }
        List<GameObject> oldPrefabs = XS_Editor.LoadAllAssetsAtPath<GameObject>($"{outputPath}/{root}/Tiles/Prefabs");
        for (int i = 0; i < oldPrefabs.Count; i++)
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(oldPrefabs[i]));
        }
    }



    bool EsGameObject(int i) => subobjects[i].GetType().Equals(typeof(GameObject));
    bool EsPeça(int i, string root) => subobjects[i].name.StartsWith($"{root}-");
    bool EsPrefab(int i, string root) => subobjects[i].name.StartsWith($"{PREFAB}_{root}");
    void CrearPrefabEstat(int i, string root)
    {

        //LOAD PEÇA
        Estat estat = AssetDatabase.LoadAssetAtPath<Estat>(Path_Estat(root));

        //CREAR PREFAB & NEEDS
        intance = (GameObject)Instantiate(subobjects[i]);
        UI_Peca peça = intance.AddComponent<UI_Peca>();
        AnimacioPerCodi_GameObject_Referencia tmpOutline = (AnimacioPerCodi_GameObject_Referencia)PrefabUtility.InstantiatePrefab(outline, intance.transform);
        //AnimacioPerCodi_GameObject_Referencia tmpOutline = Instantiate(outline, Vector3.down * 0.3f, Quaternion.identity, intance.transform);

        //CREAR COLOCABLE
        EstatColocable colocable = CreateInstance<EstatColocable>();
        AssetDatabase.CreateAsset(colocable, Path_Colocable(root));

        //SETUP PEÇA
        peça.Setup(AssetDatabase.LoadAssetAtPath<EstatColocable>(Path_Colocable(root)), tmpOutline);

        //GUARDAR PREFAB
        prefab = PrefabUtility.SaveAsPrefabAsset(intance, Path_PrefabEstat(root));

        //SETUP COLOCALBE
        colocable.Setup(estat, prefab.GetComponent<UI_Peca>());

        DestroyImmediate(intance);
    }



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
        Debug.Log($"Nom final = {nom}");
    }



    bool TileJaExisteix(string root)
    {
        if (AssetDatabase.LoadAssetAtPath<Tile>(Path_Tile(root)) != null)
        {
            Debug.LogError($"El Tile amb el nom {nom} ja existeix. Es possible que hagis duplicat el tipus de peça. Mira si es pot eliminar o s'ha de crear com una versio.");
            return true;
        }
        return false;
    }



    void CrearPrefabTile(int i, string root)
    {
        intance = (GameObject)Instantiate(subobjects[i]);

        if(invertit)
            intance.transform.localScale = new Vector3(-1, 1, 1);

        //Debug.Log("Potser aqui es podrien afegir els efectes i elements extres i logiques");

        //AssetDatabase.CreateAsset(intance, Path_AssetPrefab(root));
        prefab = PrefabUtility.SaveAsPrefabAsset(intance, Path_PrefabTile(root));
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
            //Debug.Log($"{nomsConnexions[nc]}");
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
                AssetDatabase.CreateAsset(connexio, Path_Connexio(connexio.name));
                connexionsTile[nc] = AssetDatabase.LoadAssetAtPath<Connexio>(Path_Connexio(connexio.name));

                referencies.Refrex();
            }
        }
    }
    private void CrearTile(string root)
    {
        tile = CreateInstance<Tile>();
        tile.Setup(prefab, connexionsTile);

        AssetDatabase.CreateAsset(tile, Path_Tile(root));
    }



    void AddTileToTileset(string root)
    {
        if (pes == -1)
        {
            Debug.LogError("No hi ha pes!");
            return;
        }


        tileset = AssetDatabase.LoadAssetAtPath<TileSetBase>(Path_Tileset(root));

        if (tileset == null)
        {
            Debug.LogError("No hi ha tilesset creat!");
            return;
        }

        Debug.Log($"Add {tile.name}");
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
