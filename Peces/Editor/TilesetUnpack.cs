using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;
using UnityEditor;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Unpack/Tileset")]
public class TilesetUnpack : ScriptableObject
{
    public enum Tipus { Simple, Ocupable, Condicional}
    //Ordre: Estats, Tiles i Subtiles
    const string TIPUS_SIMPLE = "TS";
    const string TIPUS_OCUPABLE = "TO";
    const string TIPUS_CONDICIONAL = "TC";
    const string NULES = "Nules";
    const string ESPECIFIQUES = "Espes";
    const char SEPARADO = '_';
    const char SEPARADOR_INFO = ':';
    const char IGUAL = '=';
    const char PUNT = '.';


    //[SerializeField] Object tiles;
    [SerializeField] TilesUnpack tilesUnpack;


    //INTERN
    Object[] subobjects;
    string outputTiles;
    Referencies referencies;

    TileSetBase tileset;
    bool tilesetCreat = false;
    Tipus tipus;
    TileSet_Simple simple;
    TileSet_Ocupable ocupable;
    TileSet_Condicional condicional;
    //string debug;
    int indexCondicio;
    List<TileSet_Condicional.Condicio> condicions;
    string[] nomEstats;
    string[] igualacio;
    string[] nules;
    List<Connexio> connexions;
    Connexio trobada;
    List<Estat> estats;
    string[] nomConnexions;





    string Path_Folder(string root) => $"{outputTiles}/{root}/Tiles";
    string Path_FolderPrefab(string root) => $"{outputTiles}/{root}/Tiles/Prefabs";


    string IndexCondicio(int i, string root) => subobjects[i].name.Substring(root.Length + 4, 1);



    public void Unpack(string root, Object[] subobjects, string outputTiles, string outputDetalls, Referencies referencies)
    {
        //subobjects = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(tiles));
        this.subobjects = subobjects;
        this.outputTiles = outputTiles;
        this.referencies = referencies;

        CrearFoldersSiCal(root);

        tilesetCreat = false;
        simple = null;
        ocupable = null;
        condicional = null;

        for (int i = 0; i < subobjects.Length; i++)
        {
            if (!EsGameObject(i))
                continue;

            if (!EsInfo(i, root))
                continue;

            if (subobjects[i].name.Contains(PUNT))
                continue;

            EliminarAssetAntic(root);
        }

        for (int i = 0; i < subobjects.Length; i++)
        {
            if (!EsGameObject(i))
                continue;

            if (!EsInfo(i, root))
                continue;

            if (subobjects[i].name.Contains(PUNT))
                continue;

            //EliminarAssetAntic(root);

            CrearTileset(i, root);

            if (tipus == Tipus.Condicional)
            {
                if (!int.TryParse(IndexCondicio(i, root), out indexCondicio))
                {
                    Debug.LogError("¡¡¡Hauria de ser un Int!!!");
                    continue;
                }
                CrearCondicions(i, root, indexCondicio);
                Condicions(i, root, indexCondicio);
            }

            Nules(i, root, indexCondicio);
            Especifiques(i, root, indexCondicio);

            CrearAsset(root);

            AssignarAEstat(root);
        }

        tilesUnpack.Unpack(root, subobjects, outputTiles, outputDetalls, referencies);

    }

    void CrearFoldersSiCal(string root)
    {
        if (!AssetDatabase.IsValidFolder(Path_Folder(root)))
            AssetDatabase.CreateFolder($"{outputTiles}/{root}", "Tiles");

        if (!AssetDatabase.IsValidFolder(Path_FolderPrefab(root)))
            AssetDatabase.CreateFolder(Path_Folder(root), "Prefabs");
    }



    bool EsGameObject(int i) => subobjects[i].GetType().Equals(typeof(GameObject));
    bool EsInfo(int i, string root) => subobjects[i].name.StartsWith($"_{root}-");
    void EliminarAssetAntic(string root)
    {
        if (AssetDatabase.LoadAssetAtPath($"{Path_Folder(root)}/{root}.asset", typeof(Object)) == null)
            return;

        AssetDatabase.DeleteAsset($"{Path_Folder(root)}/{root}.asset");
    }



    void CrearTileset(int i, string root)
    {
        if (!tilesetCreat)
        {
            if (EsSimple(i, root))
            {
                tipus = Tipus.Simple;

                simple = CreateInstance<TileSet_Simple>();
                simple.name = root;
                simple.Setup();

                tilesetCreat = true;
                tileset = simple;
            }
            else if (EsOcupable(i, root))
            {
                tipus = Tipus.Ocupable;

                ocupable = CreateInstance<TileSet_Ocupable>();
                ocupable.name = root;
                ocupable.Setup();

                tilesetCreat = true;
                tileset = ocupable;
            }
            else if (EsCondicional(i, root))
            {
                tipus = Tipus.Condicional;

                condicional = CreateInstance<TileSet_Condicional>();
                condicional.name = root;
                condicional.Condicions = new TileSet_Condicional.Condicio[0];
                condicional.Setup();

                tilesetCreat = true;
                tileset = condicional;
            }
        }

        bool EsSimple(int i, string root) => subobjects[i].name.Substring(root.Length + 2).StartsWith(TIPUS_SIMPLE);
        bool EsOcupable(int i, string root) => subobjects[i].name.Substring(root.Length + 2).StartsWith(TIPUS_OCUPABLE);
        bool EsCondicional(int i, string root) => subobjects[i].name.Substring(root.Length + 2).StartsWith(TIPUS_CONDICIONAL);
    }


    void CrearCondicions(int i, string root, int index)
    {
        if (index > condicional.Condicions.Length)
        {
            condicions = new List<TileSet_Condicional.Condicio>(condicional.Condicions);
            for (int c = 0; c < index - condicional.Condicions.Length; c++)
            {
                condicions.Add(new TileSet_Condicional.Condicio().Setup());
            }
            condicional.Condicions = condicions.ToArray();
        }
    }
    void Condicions(int index, string root, int condicio)
    {
        if (!EsCondicio(index, root))
            return;

        condicional.Condicions[condicio - 1].id = InfoNom(index, root);
        nomEstats = InfoEstats(index, root);
        igualacio = new string[0];
        for (int i = 0; i < nomEstats.Length; i++)
        {
            if(i == nomEstats.Length - 1)
            {
                if (nomEstats[i].Contains(">="))
                {
                    igualacio = nomEstats[i].Split(">=");
                    condicional.Condicions[condicio - 1].SetCondicions(false, true, true, int.Parse(igualacio[1]));
                }
                else if (nomEstats[i].Contains("<="))
                {
                    igualacio = nomEstats[i].Split("<=");
                    condicional.Condicions[condicio - 1].SetCondicions(false, true, true, int.Parse(igualacio[1]));
                }
                else if (nomEstats[i].Contains('>')) 
                {
                    igualacio = nomEstats[i].Split('>');
                    condicional.Condicions[condicio - 1].SetCondicions(true, false, false, int.Parse(igualacio[1]));
                }
                else if (nomEstats[i].Contains('<'))
                {
                    igualacio = nomEstats[i].Split('<');
                    condicional.Condicions[condicio - 1].SetCondicions(false, false, true, int.Parse(igualacio[1]));
                }
                else if (nomEstats[i].Contains('='))
                {
                    igualacio = nomEstats[i].Split('=');
                    condicional.Condicions[condicio - 1].SetCondicions(false, true, false, int.Parse(igualacio[1]));
                }
                nomEstats[i] = igualacio[0];
            }
        }

        bool EsCondicio(int i, string root) => subobjects[i].name.Substring(root.Length + 6).StartsWith('(');
        string InfoNom(int i, string root) => subobjects[i].name.Substring(root.Length + 6).Split(SEPARADOR_INFO)[0];
        string[] InfoEstats(int i, string root) => subobjects[i].name.Substring(root.Length + 6).Split(SEPARADOR_INFO)[1].Split(SEPARADO);
    }



    void Nules(int index, string root, int condicio = 0)
    {
        if (!EsNules(index, root, tipus == Tipus.Condicional))
            return;

        if (!HiHaInfo(index, root, tipus == Tipus.Condicional))
            return;

        nules = Info(index, root, tipus == Tipus.Condicional);

        connexions = new List<Connexio>();
        for (int i = 0; i < nules.Length; i++)
        {
            trobada = referencies.GetConnexio(nules[i]);
            if (trobada != null)
                connexions.Add(trobada);
        }

        switch (tipus)
        {
            case Tipus.Simple:
                simple.TileSet.ConnexionsNules = connexions.ToArray();
                break;
            case Tipus.Ocupable:
                ocupable.TileSetLliure.ConnexionsNules = connexions.ToArray();
                ocupable.TileSetOcupat.ConnexionsNules = connexions.ToArray();
                break;
            case Tipus.Condicional:
                condicional.Condicions[condicio-1].TileSet.ConnexionsNules = connexions.ToArray();
                break;
        }

        bool EsNules(int i, string root, bool condicional = false) => subobjects[i].name.Substring(root.Length + 4 + (condicional ? 2 : 0)).StartsWith(NULES);
        bool HiHaInfo(int i, string root, bool condicional = false) => !string.IsNullOrEmpty(subobjects[i].name.Substring(root.Length + 4 + (condicional ? 2 : 0)).Split(SEPARADOR_INFO)[1]);
        string[] Info(int i, string root, bool condicional = false) => subobjects[i].name.Substring(root.Length + 4 + (condicional ? 2 : 0)).Split(SEPARADOR_INFO)[1].Split(SEPARADO);
    }



    void Especifiques(int index, string root, int condicio = 0)
    {
        if (!EsEspecifiques(index, root, tipus == Tipus.Condicional))
            return;

        if (!HiHaInfo(index, root, tipus == Tipus.Condicional))
            return;

        nomEstats = InfoEstats(index, root, tipus == Tipus.Condicional);
        estats = new List<Estat>();
        for (int i = 0; i < nomEstats.Length; i++)
        {
            Estat trobat = referencies.GetEstat(nomEstats[i]);
            if(trobat != null)
                estats.Add(trobat);
        }
        nomConnexions = InfoConnexions(index, root, tipus == Tipus.Condicional);
        connexions = new List<Connexio>();
        for (int i = 0; i < nomConnexions.Length; i++)
        {
            Connexio trobada = referencies.GetConnexio(nomConnexions[i]);
            if(trobada != null)
                connexions.Add(trobada);
        }

        if(estats.Count == 0 || connexions.Count == 0)
        {
            Debug.LogError("Falta informacio per completar les connexions especifiques, així que es borra.");
            return;
        }

        switch (tipus)
        {
            case Tipus.Simple:
                simple.TileSet.ConnexioEspesifica = new ConnexioEspesifica(estats, connexions);
                break;
            case Tipus.Ocupable:
                ocupable.TileSetLliure.ConnexioEspesifica = new ConnexioEspesifica(estats, connexions);
                ocupable.TileSetOcupat.ConnexioEspesifica = new ConnexioEspesifica(estats, connexions);
                break;
            case Tipus.Condicional:
                condicional.Condicions[condicio-1].TileSet.ConnexioEspesifica = new ConnexioEspesifica(estats, connexions);
                break;
        }

        bool EsEspecifiques(int i, string root, bool condicional = false) => subobjects[i].name.Substring(root.Length + 4 + (condicional ? 2 : 0)).StartsWith(ESPECIFIQUES);
        bool HiHaInfo(int i, string root, bool condicional = false) => !string.IsNullOrEmpty(subobjects[i].name.Substring(root.Length + 4 + (condicional ? 2 : 0)).Split(SEPARADOR_INFO)[1]);
        string[] InfoEstats(int i, string root, bool condicional = false) => subobjects[i].name.Substring(root.Length + 4 + (condicional ? 2 : 0)).Split(SEPARADOR_INFO)[1].Split(IGUAL)[0].Split(SEPARADO);
        string[] InfoConnexions(int i, string root, bool condicional = false) => subobjects[i].name.Substring(root.Length + 4 + (condicional ? 2 : 0)).Split(SEPARADOR_INFO)[1].Split(IGUAL)[1].Split(SEPARADO);
    }



    void CrearAsset(string root)
    {
        if (!tilesetCreat)
            return;

        if (AssetDatabase.LoadAssetAtPath($"{Path_Folder(root)}/{root}.asset", typeof(Object)) != null)
            return;

        switch (tipus)
        {
            case Tipus.Simple:
                AssetDatabase.CreateAsset(simple, $"{Path_Folder(root)}/{root}.asset");
                break;
            case Tipus.Ocupable:
                AssetDatabase.CreateAsset(ocupable, $"{Path_Folder(root)}/{root}.asset");
                break;
            case Tipus.Condicional:
                AssetDatabase.CreateAsset(condicional, $"{Path_Folder(root)}/{root}.asset");
                break;
        }

        EditorUtility.SetDirty(AssetDatabase.LoadAssetAtPath($"{Path_Folder(root)}/{root}.asset", typeof(Object)));
        Debug.Log($"Crear Tilset: {root}");



        Debug.LogError("FALTA: Agregar tiles connectables i guardar tots els tiles possibles en algun lloc");
    }



    void AssignarAEstat(string root)
    {
        Debug.Log($"...assignar a {outputTiles}/{root}.asset");
        Estat estat = AssetDatabase.LoadAssetAtPath<Estat>($"{outputTiles}/{root.ToUpper()}.asset");
        estat.SetTileset = tileset;
    }








    private void OnValidate()
    {
        tilesetCreat = false;
    }
}
