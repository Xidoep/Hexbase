using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;
using UnityEditor;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Unpack/Tiles")]
public class TilesUnpack : ScriptableObject
{
    const string PREFAB = "_Prefab";

    const string AUTOGENERATS_PATH = "Assets/XidoStudio/Hexbase/Peces/Connexio";
    const char SEPARADO = '_';
    const string PES = "___$";
    const char INVERTIT = '%';
    const string CONDICIO = "_c";
    const char VARIACIO = '#';
    const char PUNT = '.';
    const string PROP = "prop_";
    const string PROP_OCUPAT = "prop/0_";
    const char SCRIPT = '@';

    //[SerializeField] Object tiles;
    [SerializeField] AnimacioPerCodi_GameObject_Referencia outline;

    //[Apartat("Auto-configurable")]
   // [SerializeField] List<Object> connexions;



    //INTERN
    Object[] subobjects;
    string outputTiles;
    string outputDetalls;
    Referencies referencies;

    string nom;
    bool invertit;
    string[] nomsConnexions;
    Connexio[] connexionsTile;
    GameObject intance;
    GameObject detallPrefab;
    GameObject detall;
    GameObject prefab;
    bool trobat;
    Connexio connexio;
    Tile tile;
    int variacio = 0;
    bool condicional;
    int[] pes;
    //int[] condicio;
    TileSetBase tileset;
    List<Tile> oldTiles;
    List<GameObject> oldPrefabs;
    Estat estat;
    UI_Peca peça;
    AnimacioPerCodi_GameObject_Referencia tmpOutline;
    EstatColocable colocable;
    string[] tmpPes;
    string[] pesos;
    Detall_Ocupable detallOcupable;

    bool ocupable;

    bool detallsCreats;
    bool connexionsCreades;

    string Main(string root) => $"{outputTiles}/{root}";
    string Main_Prefabs(string root) => $"{Main(root)}/Tiles/Prefabs";
    string Main_Tiles(string root) => $"{Main(root)}/Tiles";
    string Path_Estat(string root) => $"{outputTiles}/{root.ToUpper()}.asset";
    string Path_Tile(string root) => $"{Main_Tiles(root)}/{nom}{(variacio != 0 ? $"_{variacio}" : "")}.asset";
    string Path_TileLliure(string root) => $"{Main_Tiles(root)}/Lliure/{nom}{(variacio != 0 ? $"_{variacio}" : "")}.asset";
    string Path_Tileset(string root) => $"{Main_Tiles(root)}/{root}.asset";
    string Path_Connexio(string nom) => $"{AUTOGENERATS_PATH}/{nom}.asset";
    string Path_PrefabTile(string root) => $"{Main_Prefabs(root)}/{nom}{(variacio != 0 ? $"_{variacio}" : "")}.prefab";
    string Path_PrefabTileLliure(string root) => $"{Main_Prefabs(root)}/Lliure/{nom}{(variacio != 0 ? $"_{variacio}" : "")}.prefab";
    //string Path_AssetPrefab(string root) => $"{outputPath}/{root}/Prefab/{nom}.prefab";
    string Path_Colocable(string root) => $"{Main(root)}/Colocable/{root}.asset";
    string Path_PrefabEstat(string root) => $"{Main(root)}/Prefab/{root}.prefab";
    string Path_Detall(string nom) => $"{outputDetalls}/{nom}.prefab";


    public void Unpack(string root, Object[] subobjects, string outputTiles, string outputDetalls, Referencies referencies, bool detalls, bool actualitzarConnexions)
    {
        this.subobjects = subobjects;
        this.outputTiles = outputTiles;
        this.referencies = referencies;
        this.outputDetalls = outputDetalls;

        NetejarTileset(root);
        EliminarAntics(root, outputTiles);

        if (!connexionsCreades)
        {
            if (actualitzarConnexions)
            {
                for (int i = 0; i < subobjects.Length; i++)
                {
                    if (!EsConnexio(i))
                        continue;

                    CrearConnexions(i);
                }
                referencies.Refresh();


                for (int i = 0; i < subobjects.Length; i++)
                {
                    if (!EsConnexio(i))
                        continue;

                    AddViables(i);
                }
                if (EsConnexio(subobjects.Length - 1))
                    AddViables(subobjects.Length - 1);
                referencies.Refresh();
            }
        }
        connexionsCreades = true;

        if (!detallsCreats)
        {
            for (int i = 0; i < subobjects.Length; i++)
            {
                if (!EsGameObject(i))
                    continue;

                if (!EsProp(i))
                    continue;

                if (subobjects[i].name.Contains(PUNT))
                    continue;

                if (detalls)
                    CrearDetall(i);
            }
            referencies.Refresh();
        }

        detallsCreats = true;

        for (int i = 0; i < subobjects.Length; i++)
        {
            if (!EsGameObject(i))
                continue;

            if (!EsPeça(i, root))
            {
                if (!EsPrefab(i, root))
                    continue;

                CrearPrefabEstat(i, root);
                continue;
            }

            if (subobjects[i].name.Contains(PUNT))
                continue;

            //Debug.Log($"Tot el nom = {subobjects[i].name}");

            GetVariacio(i);
            GetPes(i);
            GetInvertit(i);

            //EliminarAssetAntic(root);

            if (TileJaExisteix(root))
            {
                SetupTileset(root);
                continue;
            }

            CrearPrefabTile(i, root, true);
            AddConnexions(root);
            CrearTile(root, true);
            AddTileToTileset(root, true);

            if (tileset is TileSet_Ocupable)
            {
                CrearPrefabTile(i, root, false);
                AddConnexions(root);
                CrearTile(root, false);
                AddTileToTileset(root, false);
            }

            /*
            CrearPrefabTile(i, root);
            //if (tileset is TileSet_Ocupable)
            //    CrearPrefabTile(i, root, true);

            AddConnexions(root);
            CrearTile(root, tileset is TileSet_Ocupable);

            AddTileToTileset(root);
            */
            //Debug.Log("---------------------");
        }

        SaveTilset(root);

    }

    public void Reset()
    {
        connexionsCreades = false;
        detallsCreats = false;
    }

    void NetejarTileset(string root)
    {
        tileset = AssetDatabase.LoadAssetAtPath<TileSetBase>($"{outputTiles}/{root}/Tiles/{root}.asset");

        if (tileset == null)
        {
            Debug.LogError("No hi ha tileset creat!");
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
        oldTiles = XS_Editor.LoadAllAssetsAtPath<Tile>($"{outputPath}/{root}/Tiles");
        for (int i = 0; i < oldTiles.Count; i++)
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(oldTiles[i]));
        }

        oldPrefabs = XS_Editor.LoadAllAssetsAtPath<GameObject>($"{outputPath}/{root}/Tiles/Prefabs");
        for (int i = 0; i < oldPrefabs.Count; i++)
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(oldPrefabs[i]));
        }
    }


    bool EsProp(int i) => subobjects[i].name.StartsWith(PROP) || subobjects[i].name.StartsWith(PROP_OCUPAT);
    bool EsGameObject(int i) => subobjects[i].GetType().Equals(typeof(GameObject));
    bool EsPeça(int i, string root) => subobjects[i].name.StartsWith($"{root}-");
    bool EsConnexio(int i) => subobjects[i].name.StartsWith("C_");
    bool EsPrefab(int i, string root) => subobjects[i].name.StartsWith($"{PREFAB}-{root}");
    bool TeScripts(int i) => subobjects[i].name.Contains(SCRIPT);
    bool TeScripts(string nom) => nom.Contains(SCRIPT);
    string[] Scripts(int i) => subobjects[i].name.Split(SCRIPT)[1].Split(',');
    /*bool EsPrefab(int i, string root) 
    {
        Debug.Log($"EsPrefab({PREFAB}-{root})? {subobjects[i].name.StartsWith($"{PREFAB}-{root}")}");
        return subobjects[i].name.StartsWith($"{PREFAB}-{root}");
    }*/

    void CrearDetall(int i)
    {
        Debug.Log($"Crear detall = {subobjects[i].name.Substring(5)}");
        AssetDatabase.DeleteAsset(subobjects[i].name.Substring(5));

        intance = (GameObject)Instantiate(subobjects[i]);


        /*if (TeScripts(i))
        {
            intance.name = intance.name.Split(SCRIPT)[0];
            for (int c = 0; c < Scripts(i).Length; c++)
            {
                if (Scripts(i)[c] == "Pis") intance.AddComponent<Detall_Pis>();
            }
        }*/

        nom = subobjects[i].name.Substring(subobjects[i].name.Contains("/0") ? 7 : 5);

        if (TeScripts(i))
        {
            AddScriptsByNameWithArguments(intance.name.Contains('(') ? intance.name.Split('(')[0] : intance.name, intance);
        }

        prefab = PrefabUtility.SaveAsPrefabAsset(intance, Path_Detall(!TeScripts(i) ? nom : subobjects[i].name.Substring(5).Split('@')[0]));

        DestroyImmediate(intance);
    }

    void CrearPrefabEstat(int i, string root)
    {
        //ELIMINAR ANTERIOR
        AssetDatabase.DeleteAsset(Path_Colocable(root));
        AssetDatabase.DeleteAsset(Path_PrefabEstat(root));


        //LOAD PEÇA
        estat = AssetDatabase.LoadAssetAtPath<Estat>(Path_Estat(root));
        Debug.Log($"Crear prefab estat = {Path_Estat(root)}");
        Debug.Log($"{estat.name}");

        //CREAR PREFAB & NEEDS
        intance = (GameObject)Instantiate(subobjects[i]);
        peça = intance.AddComponent<UI_Peca>();
        tmpOutline = (AnimacioPerCodi_GameObject_Referencia)PrefabUtility.InstantiatePrefab(outline, intance.transform);

        //CREAR COLOCABLE
        colocable = CreateInstance<EstatColocable>();
        AssetDatabase.CreateAsset(colocable, Path_Colocable(root));

        //SETUP PEÇA
        peça.Setup(colocable, tmpOutline);

        //GUARDAR PREFAB
        prefab = PrefabUtility.SaveAsPrefabAsset(intance, Path_PrefabEstat(root));

        //SETUP COLOCALBE
        colocable.Setup(estat, prefab.GetComponent<UI_Peca>());
        EditorUtility.SetDirty(colocable);

        DestroyImmediate(intance);
    }



    void GetVariacio(int i)
    {
        variacio = 0;

        //Debug.Log($"Trobar variacio a {subobjects[i].name}");

        if (!subobjects[i].name.Contains(VARIACIO))
        {
            nom = subobjects[i].name;
            return;
        }

        nom = subobjects[i].name.Split(VARIACIO)[0];

        int.TryParse(subobjects[i].name.Split(VARIACIO)[1], out variacio);
    }


    void GetPes(int index)
    {
        pes = new int[] { -1 };
        //condicio = new int[] { -1 };

        Debug.Log($"Trobar pes a {nom}");
        tmpPes = nom.Split(PES);

        //Treu la part de condicio del nom final
        nom = tmpPes[0];

        //treu la part de scripts si hi es
        if (tmpPes[1].Contains(SCRIPT))
        {
            tmpPes[1] = tmpPes[1].Split(SCRIPT)[0];
        }

        //Conté condicions???
        if (!tmpPes[1].Contains(CONDICIO))
        {
            condicional = false;
            int.TryParse(tmpPes[1], out pes[0]);
            return;
        }

        //
        condicional = true;
        pesos = tmpPes[1].Substring(1,tmpPes[1].Length - 1).Split(CONDICIO);
        pes = new int[pesos.Length];
        for (int i = 0; i < pesos.Length; i++)
        {
            //Debug.Log($"Pes{i}:{pesos[i]}");

            if (pesos[i].Equals('-'))
                continue;

            int.TryParse(pesos[i], out pes[i]);
            //condicio = i;
        }
        //int.TryParse(pesos[0], out pes);
        //int.TryParse(pesos[1], out condicio);
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
        //Debug.Log($"Nom final = {nom}");
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



    void CrearPrefabTile(int index, string root, bool lliure = true)
    {
        intance = (GameObject)Instantiate(subobjects[index]);
        
        //Substituir childs pels prefabs
        Debug.Log($"{intance.transform.childCount} childs");
        float childCount = intance.transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            //if (!EsGameObject(i))
            //    continue;

            //Debug.Log($"Child{i} = {intance.transform.GetChild(i).name}");
            if (!referencies.DetallsContains(intance.transform.GetChild(i).name.Split(PUNT)[0].Substring(5)))
                continue;

            detallPrefab = referencies.GetDetall(intance.transform.GetChild(i).name.Split(PUNT)[0].Substring(5));
            
            intance.transform.GetChild(i).gameObject.SetActive(false);
            
            if (lliure && detallPrefab.TryGetComponent(out detallOcupable))
                continue;

            detall = (GameObject)PrefabUtility.InstantiatePrefab(detallPrefab, intance.transform);
            detall.transform.Copiar(intance.transform.GetChild(i));


            if (!TeScripts(intance.transform.GetChild(i).name))
                continue;

            AddScriptsByNameWithArguments(intance.transform.GetChild(i).name, detall);
        }


        if(invertit)
            intance.transform.localScale = new Vector3(-1, 1, 1);


        if (TeScripts(index))
        {
            AddScriptsByNameWithArguments(intance.name.Contains('(') ? intance.name.Split('(')[0] : intance.name, intance);
        }

        if (lliure)
        {
            if (!AssetDatabase.IsValidFolder($"{Main_Prefabs(root)}/Lliure"))
                AssetDatabase.CreateFolder(Main_Prefabs(root), "Lliure");
        }

        prefab = PrefabUtility.SaveAsPrefabAsset(intance, !lliure ? Path_PrefabTile(root) : Path_PrefabTileLliure(root));
        DestroyImmediate(intance);

    }
    void AddScriptsByNameWithArguments(string name, GameObject objectiu)
    {
        
        string component = name.Split(SCRIPT)[1];
        string nom = component;
        string[] arguments = new string[0];
        if (component.Contains(':'))
        {
            nom = component.Split(':')[0];
            if (component.Split(':')[1].Contains(','))
                arguments = component.Split(':')[1].Split(',');
            else arguments = new string[] { component.Split(':')[1] };
        }

        string debug = nom  + "|";
        for (int i = 0; i < arguments.Length; i++)
        {
            debug += arguments[i] + ", ";
        }
        Debug.Log(debug);
        if (nom == "Pis") objectiu.AddComponent<Detall_Pis>().Setup(arguments);
        if (nom == "Pisos") objectiu.AddComponent<Detall_Pisos>().Setup(arguments);
        if (nom == "Ocupable") objectiu.AddComponent<Detall_Ocupable>().Setup(arguments);
    }
    void CrearConnexions(int i)
    {
        string nomConnexio = subobjects[i].name.Substring(2).Split('=')[0];

        if (referencies.GetConnexio(nomConnexio) != null)
            return;

        connexio = CreateInstance<Connexio>();
        connexio.name = nomConnexio;
        AssetDatabase.CreateAsset(connexio, Path_Connexio(nomConnexio));

        EditorUtility.SetDirty(connexio);
        Debug.Log($"Crear Connexio {nomConnexio}");
    }
    void AddViables(int i)
    {
        Debug.Log(subobjects[i].name);
        string nomConnexio = subobjects[i].name.Substring(2).Split('=')[0];
        string[] viables = subobjects[i].name.Split('=')[1].Split(',');

        Connexio connexio = referencies.GetConnexioNew(nomConnexio);
        Connexio viable;
        for (int v = 0; v < viables.Length; v++)
        {
            viable = referencies.GetConnexioNew(viables[v]);
            //Debug.Log($"{connexio.name} > {viables[v]}: {viable != null}");
            
            if (viable == null)
                continue;

            connexio.AddViable(viable);
        }

        EditorUtility.SetDirty(connexio);
    }

    void AddConnexions(string root)
    {
        nomsConnexions = nom.Substring(root.Length + 1).Split(SEPARADO);
        connexionsTile = new Connexio[3];
        for (int nc = 0; nc < 3; nc++)
        {
            //trobat = false;
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
                    //trobat = true;
                    break;
                }
            }
            /*
            if (!trobat)
            {
                connexio = CreateInstance<Connexio>();
                connexio.name = nomsConnexions[nc];
                AssetDatabase.CreateAsset(connexio, Path_Connexio(connexio.name));
                connexionsTile[nc] = AssetDatabase.LoadAssetAtPath<Connexio>(Path_Connexio(connexio.name));

                referencies.Refresh();
            }
            */
        }
    }
    private void CrearTile(string root, bool lliure)
    {
        tile = CreateInstance<Tile>();
        tile.Setup(prefab, connexionsTile);

        if (lliure)
        {
            if(!AssetDatabase.IsValidFolder($"{Main_Tiles(root)}/Lliure"))
                AssetDatabase.CreateFolder(Main_Tiles(root), "Lliure");
        }

        AssetDatabase.CreateAsset(tile,
            lliure ?
            Path_TileLliure(root) :
            Path_Tile(root));

        Debug.Log($"Crear Tile: {nom} (Invertit = {invertit}; Variacio = {variacio})");
    }


    void SetupTileset(string root)
    {
        ocupable = false;
        if (pes.Length > 1 && pes[0] == -1)
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

        if (tileset is TileSet_Simple)
        {
            ((TileSet_Simple)tileset).TileSet.SetPes(tile, pes[0]);

            EditorUtility.SetDirty(tileset);
        }
        else if (tileset is TileSet_Ocupable)
        {
            ((TileSet_Ocupable)tileset).TileSetLliure.SetPes(tile, pes[0]);
            ((TileSet_Ocupable)tileset).TileSetOcupat.SetPes(tile, pes[0]);
            EditorUtility.SetDirty(tileset);
            ocupable = true;
        }
        else if (tileset is TileSet_Condicional)
        {
            for (int i = 0; i < pes.Length; i++)
            {
                if (pes[i] == 0)
                    continue;

                ((TileSet_Condicional)tileset).Condicions[i].TileSet.SetPes(tile, pes[i]);
                EditorUtility.SetDirty(tileset);
            }

        }
    }
    void AddTileToTileset(string root, bool lliure)
    {
        if (pes.Length > 1 && pes[0] == -1)
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

        //Debug.Log($"Add {tile.name}");
        if (tileset is TileSet_Simple)
        {
            ((TileSet_Simple)tileset).TileSet.AddTile(tile, pes[0]);
            
            EditorUtility.SetDirty(tileset);
        }
        else if (tileset is TileSet_Ocupable)
        {
            if(lliure)
                ((TileSet_Ocupable)tileset).TileSetLliure.AddTile(tile, pes[0]);
            else ((TileSet_Ocupable)tileset).TileSetOcupat.AddTile(tile, pes[0]);
            EditorUtility.SetDirty(tileset);
        }
        else if (tileset is TileSet_Condicional)
        {
            for (int i = 0; i < pes.Length; i++)
            {
                if (pes[i] == 0)
                    continue;

                ((TileSet_Condicional)tileset).Condicions[i].TileSet.AddTile(tile, pes[i]);
                EditorUtility.SetDirty(tileset);
            }
            
        }
    }


    
    void SaveTilset(string root)
    {
        //Debug.Log($"Save {Path_Tileset(root)}");
        //tileset = AssetDatabase.LoadAssetAtPath<TileSetBase>(Path_Tileset(root));
        //EditorUtility.SetDirty(tileset);

        AssetDatabase.SaveAssetIfDirty(tileset);
    }
}
