using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;
using UnityEditor;

[CreateAssetMenu(menuName = "Xido Studio/Hex/UnpackTileset")]
public class TilesetUnpack : ScriptableObject
{
    public enum Tipus { Simple, Ocupable, Condicional}
    //Ordre: Estats, Tiles i Subtiles

    const string TERRA = "Terra";
    const string CASA = "Casa";
    const string RIU = "Riu";

    const string TIPUS_SIMPLE = "TS";
    const string TIPUS_OCUPABLE = "TO";
    const string TIPUS_CONDICIONAL = "TC";
    const string NULES = "Nules";
    const string ESPECIFIQUES = "Espes";
    const char SEPARADO = '_';
    const char SEPARADOR_INFO = ':';
    const char IGUAL = '=';


    [SerializeField] Referencies referencies;
    [SerializeField] Object tiles;
    [SerializeField] string outputPath;
    [SerializeField] TilesUnpack tilesUnpack;
    [Space(20)]
    [SerializeField] bool terra;
    [SerializeField] bool casa;
    [SerializeField] bool riu;

    //INTERN
    Object[] subobjects;
    bool tilesetCreat = false;
    Tipus tipus;
    TileSet_Simple simple;
    TileSet_Ocupable ocupable;
    TileSet_Condicional condicional;
    string debug;
    int indexCondicio;


    string Path_Folder(string root) => $"{outputPath}/{root}";
    string Path_FolderPrefab(string root) => $"{outputPath}/{root}/Prefab";


    string IndexCondicio(int i, string root) => subobjects[i].name.Substring(root.Length + 4, 1);


    void CrearFoldersSiCal(string root)
    {
        if (!AssetDatabase.IsValidFolder(Path_Folder(root)))
            AssetDatabase.CreateFolder(outputPath, root);

        if (!AssetDatabase.IsValidFolder(Path_FolderPrefab(root)))
            AssetDatabase.CreateFolder(Path_Folder(root), "Prefab");
    }


    [ContextMenu("Unpack")]
    public void Unpack()
    {
        subobjects = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(tiles));

        if (terra) Unpack(TERRA);
        if (casa) Unpack(CASA);
        if (riu) Unpack(RIU);
    }

    void Unpack(string root)
    {
        CrearFoldersSiCal(root);

        debug = "";

        tilesetCreat = false;
        simple = null;
        ocupable = null;
        condicional = null;

        //ELIMINAR
        for (int i = 0; i < subobjects.Length; i++)
        {
            if (!EsGameObject(i))
                continue;

            if (!EsInfo(i, root))
                continue;

            EliminarAssetAntic(root);
        }

        //CREAR
        for (int i = 0; i < subobjects.Length; i++)
        {
            if (!EsGameObject(i))
                continue;

            if (!EsInfo(i, root))
                continue;

            Debug.Log(subobjects[i].name);

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

        }
    }

    bool EsGameObject(int i) => subobjects[i].GetType().Equals(typeof(GameObject));
    bool EsInfo(int i, string root) => subobjects[i].name.StartsWith($"_{root}-");
    void EliminarAssetAntic(string root)
    {
        if (AssetDatabase.LoadAssetAtPath($"{outputPath}/{root}/{root}.asset", typeof(Object)) == null)
            return;

        AssetDatabase.DeleteAsset($"{outputPath}/{root}/{root}.asset");
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
                Debug.Log("Crear TileSet Tipus simple"); 
                tilesetCreat = true;
            }
            else if (EsOcupable(i, root))
            {
                tipus = Tipus.Ocupable;
                ocupable = CreateInstance<TileSet_Ocupable>();
                ocupable.name = root;
                ocupable.Setup();
                Debug.Log("Crear TileSet Tipus ocupable");
                tilesetCreat = true;
            }
            else if (EsCondicional(i, root))
            {
                tipus = Tipus.Condicional;
                condicional = CreateInstance<TileSet_Condicional>();
                condicional.name = root;
                condicional.Condicions = new TileSet_Condicional.Condicio[0];
                condicional.Setup();
                Debug.Log("Crear TileSet Tipus condicional");
                tilesetCreat = true;
            }
        }

        bool EsSimple(int i, string root) => subobjects[i].name.Substring(root.Length + 2).StartsWith(TIPUS_SIMPLE);
        bool EsOcupable(int i, string root) => subobjects[i].name.Substring(root.Length + 2).StartsWith(TIPUS_OCUPABLE);
        bool EsCondicional(int i, string root) => subobjects[i].name.Substring(root.Length + 2).StartsWith(TIPUS_CONDICIONAL);
    }


    void CrearCondicions(int i, string root, int index)
    {
        debug = IndexCondicio(i, root);
        if (index > condicional.Condicions.Length)
        {
            List<TileSet_Condicional.Condicio> condicions = new List<TileSet_Condicional.Condicio>(condicional.Condicions);
            for (int c = 0; c < index - condicional.Condicions.Length; c++)
            {
                condicions.Add(new TileSet_Condicional.Condicio().Setup());
                debug += "Augmentar!";
            }
            condicional.Condicions = condicions.ToArray();
        }
        Debug.Log(debug);
    }
    void Condicions(int index, string root, int condicio)
    {
        if (!EsCondicio(index, root))
            return;

        debug = "Condicions:";
        condicional.Condicions[condicio - 1].id = InfoNom(index, root);
        string[] nomEstats = InfoEstats(index, root);
        string[] igualacio = new string[0];
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
            debug += $" {nomEstats[i]},";
        }

        Debug.Log(debug);


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

        debug = "Nules:";

        string[] nules = Info(index, root, tipus == Tipus.Condicional);

        List<Connexio> connexions = new List<Connexio>();
        for (int i = 0; i < nules.Length; i++)
        {
            debug += $" {nules[i]},";
            Connexio trobada = referencies.GetConnexio(nules[i]);
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
        

        Debug.Log(debug);

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

        debug = "Especifiques:";

        string[] nomEstats = InfoEstats(index, root, tipus == Tipus.Condicional);
        List<Estat> estats = new List<Estat>();
        for (int i = 0; i < nomEstats.Length; i++)
        {
            debug += $" {nomEstats[i]},";
            Estat trobat = referencies.GetEstat(nomEstats[i]);
            if(trobat != null)
                estats.Add(trobat);
        }
        debug += "=";
        string[] nomConnexions = InfoConnexions(index, root, tipus == Tipus.Condicional);
        List<Connexio> connexions = new List<Connexio>();
        for (int i = 0; i < nomConnexions.Length; i++)
        {
            debug += $" {nomConnexions[i]},";
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


        Debug.Log(debug);

        bool EsEspecifiques(int i, string root, bool condicional = false) => subobjects[i].name.Substring(root.Length + 4 + (condicional ? 2 : 0)).StartsWith(ESPECIFIQUES);
        bool HiHaInfo(int i, string root, bool condicional = false) => !string.IsNullOrEmpty(subobjects[i].name.Substring(root.Length + 4 + (condicional ? 2 : 0)).Split(SEPARADOR_INFO)[1]);
        string[] InfoEstats(int i, string root, bool condicional = false) => subobjects[i].name.Substring(root.Length + 4 + (condicional ? 2 : 0)).Split(SEPARADOR_INFO)[1].Split(IGUAL)[0].Split(SEPARADO);
        string[] InfoConnexions(int i, string root, bool condicional = false) => subobjects[i].name.Substring(root.Length + 4 + (condicional ? 2 : 0)).Split(SEPARADOR_INFO)[1].Split(IGUAL)[1].Split(SEPARADO);
    }


    void CrearAsset(string root)
    {
        if (!tilesetCreat)
            return;

        if (AssetDatabase.LoadAssetAtPath($"{outputPath}/{root}/{root}.asset", typeof(Object)) != null)
            return;

        switch (tipus)
        {
            case Tipus.Simple:
                AssetDatabase.CreateAsset(simple, $"{outputPath}/{root}/{root}.asset");
                break;
            case Tipus.Ocupable:
                AssetDatabase.CreateAsset(ocupable, $"{outputPath}/{root}/{root}.asset");
                break;
            case Tipus.Condicional:
                AssetDatabase.CreateAsset(condicional, $"{outputPath}/{root}/{root}.asset");
                break;
        }

    }













    private void OnValidate()
    {
        tilesetCreat = false;
    }
}
