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



    bool EsGameObject(int i) => subobjects[i].GetType().Equals(typeof(GameObject));
    bool EsInfo(int i, string root) => subobjects[i].name.StartsWith($"_{root}-");
    string IndexCondicio(int i, string root) => subobjects[i].name.Substring(root.Length + 4, 1);





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
        debug = "";

        tilesetCreat = false;
        simple = null;
        ocupable = null;
        condicional = null;

        //Eliminar
        for (int i = 0; i < subobjects.Length; i++)
        {
            if (!EsGameObject(i))
                continue;

            if (!EsInfo(i, root))
                continue;
            
            if(AssetDatabase.LoadAssetAtPath($"{outputPath}/{root}/{root}.asset", typeof(Object)) != null)
                AssetDatabase.DeleteAsset($"{outputPath}/{root}/{root}.asset");
        }

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

            if (tilesetCreat)
            {
                CrearAsset(root);

            }

            Debug.Log("FALTA! Assignar el tile set a l'estat. I potser per reordenar i que estigui tot en sucarpetes de Estats...");
        }
    }







    void CrearAsset(string root)
    {
        if (AssetDatabase.LoadAssetAtPath($"{outputPath}/{root}/{root}.asset", typeof(Object)) == null)
        {
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
        
    }






    void CrearTileset(int i, string root)
    {
        if (!tilesetCreat)
        {
            if (EsTipusSimple(i, root))
            {
                tipus = Tipus.Simple;
                simple = CreateInstance<TileSet_Simple>();
                simple.name = root;
                simple.Setup();
                Debug.Log("Crear TileSet Tipus simple"); 
                tilesetCreat = true;
            }
            else if (EsTipusOcupable(i, root))
            {
                tipus = Tipus.Ocupable;
                ocupable = CreateInstance<TileSet_Ocupable>();
                ocupable.name = root;
                ocupable.Setup();
                Debug.Log("Crear TileSet Tipus ocupable");
                tilesetCreat = true;
            }
            else if (EsTipusCondicional(i, root))
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

        bool EsTipusSimple(int i, string root) => subobjects[i].name.Substring(root.Length + 2).StartsWith(TIPUS_SIMPLE);
        bool EsTipusOcupable(int i, string root) => subobjects[i].name.Substring(root.Length + 2).StartsWith(TIPUS_OCUPABLE);
        bool EsTipusCondicional(int i, string root) => subobjects[i].name.Substring(root.Length + 2).StartsWith(TIPUS_CONDICIONAL);
    }


    void Nules(int index, string root, int condicio = 0)
    {
        if (!EsInfoNules(index, root, tipus == Tipus.Condicional))
            return;

        if (!HiHaInfoNules(index, root, tipus == Tipus.Condicional))
            return;

        debug = "Nules:";

        string[] nules = InfoNules(index, root, tipus == Tipus.Condicional);

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
                simple.GetTileSet.ConnexionsNules = connexions.ToArray();
                break;
            case Tipus.Ocupable:
                ocupable.GetTileSetLliure.ConnexionsNules = connexions.ToArray();
                ocupable.GetTileSetOcupat.ConnexionsNules = connexions.ToArray();
                break;
            case Tipus.Condicional:
                condicional.Condicions[condicio-1].TileSet.ConnexionsNules = connexions.ToArray();
                break;
        }
        

        Debug.Log(debug);

        bool EsInfoNules(int i, string root, bool condicional = false) => subobjects[i].name.Substring(root.Length + 4 + (condicional ? 2 : 0)).StartsWith(NULES);
        bool HiHaInfoNules(int i, string root, bool condicional = false) => !string.IsNullOrEmpty(subobjects[i].name.Substring(root.Length + 4 + (condicional ? 2 : 0)).Split(SEPARADOR_INFO)[1]);
        string[] InfoNules(int i, string root, bool condicional = false) => subobjects[i].name.Substring(root.Length + 4 + (condicional ? 2 : 0)).Split(SEPARADOR_INFO)[1].Split(SEPARADO);
    }

    void Especifiques(int index, string root, int condicio = 0)
    {
        if (!EsInfoEspecifiques(index, root, tipus == Tipus.Condicional))
            return;

        if (!HiHaInfoEspecifiques(index, root, tipus == Tipus.Condicional))
            return;

        debug = "Especifiques:";

        string[] nomEstats = InfoEspecifiquesEstats(index, root, tipus == Tipus.Condicional);
        List<Estat> estats = new List<Estat>();
        for (int i = 0; i < nomEstats.Length; i++)
        {
            debug += $" {nomEstats[i]},";
            Estat trobat = referencies.GetEstat(nomEstats[i]);
            if(trobat != null)
                estats.Add(trobat);
        }
        debug += "=";
        string[] nomConnexions = InfoEspecifiquesConnexions(index, root, tipus == Tipus.Condicional);
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
                simple.GetTileSet.ConnexioEspesifica = new TileSetBase.ConnexioEspesifica(estats, connexions);
                break;
            case Tipus.Ocupable:
                ocupable.GetTileSetLliure.ConnexioEspesifica = new TileSetBase.ConnexioEspesifica(estats, connexions);
                ocupable.GetTileSetOcupat.ConnexioEspesifica = new TileSetBase.ConnexioEspesifica(estats, connexions);
                break;
            case Tipus.Condicional:
                condicional.Condicions[condicio-1].TileSet.ConnexioEspesifica = new TileSetBase.ConnexioEspesifica(estats, connexions);
                break;
        }


        Debug.Log(debug);

        bool EsInfoEspecifiques(int i, string root, bool condicional = false) => subobjects[i].name.Substring(root.Length + 4 + (condicional ? 2 : 0)).StartsWith(ESPECIFIQUES);
        bool HiHaInfoEspecifiques(int i, string root, bool condicional = false) => !string.IsNullOrEmpty(subobjects[i].name.Substring(root.Length + 4 + (condicional ? 2 : 0)).Split(SEPARADOR_INFO)[1]);
        string[] InfoEspecifiquesEstats(int i, string root, bool condicional = false) => subobjects[i].name.Substring(root.Length + 4 + (condicional ? 2 : 0)).Split(SEPARADOR_INFO)[1].Split(IGUAL)[0].Split(SEPARADO);
        string[] InfoEspecifiquesConnexions(int i, string root, bool condicional = false) => subobjects[i].name.Substring(root.Length + 4 + (condicional ? 2 : 0)).Split(SEPARADOR_INFO)[1].Split(IGUAL)[1].Split(SEPARADO);
    }

    void Condicions(int index, string root, int condicio)
    {

        if (!EsInfoCondicio(index, root))
            return;


        debug = "Condicions:";
        condicional.Condicions[condicio - 1].id = InfoCondicioNom(index, root);
        string[] nomEstats = InfoCondicioEstats(index, root);
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
    }
    bool EsInfoCondicio(int i, string root) => subobjects[i].name.Substring(root.Length + 6).StartsWith('(');
    string InfoCondicioNom(int i, string root) => subobjects[i].name.Substring(root.Length + 6).Split(SEPARADOR_INFO)[0];
    string[] InfoCondicioEstats(int i, string root) => subobjects[i].name.Substring(root.Length + 6).Split(SEPARADOR_INFO)[1].Split(SEPARADO);
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




    private void OnValidate()
    {
        tilesetCreat = false;
    }
}
