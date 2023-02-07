using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Save")]
public class SaveHex : ScriptableObject
{
    [SerializeField] int current = 0;
    [SerializeField] List<SavedFile> files;

    [Apartat("ESTATS")]
    [SerializeField] Estat[] estats;
    [Header("SUBESTATS")]
    [SerializeField] Subestat[] subestats;


    //public List<SavedFile> Files => files;


    Estat eTrobat;
    Subestat sTrobat;



    public void NouArxiu()
    {
        files.Add(new SavedFile());
        current = files.Count - 1;
    }
    public void BorrarPartida()
    {
        files.RemoveAt(current);
        if (files.Count == 0) files.Add(new SavedFile());
        current = Mathf.Clamp(current - 1, 0, files.Count - 1);
    }
    //public int Current => current;
    //public SavedFile CurrentSavedFile => files[current];
    public int Mode => files[current].Mode;
    public bool TePeces 
    {
        get 
        {
            if (files == null || files.Count == 0) files = new List<SavedFile>() {new SavedFile() };
            if (current > files.Count - 1) current = 0;
            return files[current].TePeces;
        
        }
    }
    public bool TeCaptures => files[current].Captures != null && files[current].Captures.Count > 0;
    //public List<SavedPe�a> Peces => files[current].Peces;

    public void Add(Pe�a pe�a, Grups grups) => files[current].Add(pe�a, grups);

    public void Actualitzar(List<Pe�a> pe�es, Grups grups) => files[current].Actualitzar(pe�es, grups);

    //public void Save(Grups grups) => files[current].Save(grups, FindObjectsOfType<Pe�a>());

    public void Load(Grups grups, Fase seguent) => files[current].Load(grups, seguent, EstatNomToPrefab, SubestatNomToPrefab);

    public void Load(int index, Grups grups, Fase colocar)
    {
        current = index;
        Load(grups, colocar);
    }
    public void AddCaptura(string path) => files[current].AddCaptura(path);

    public void RemoveCaptura(int index, string path)
    {
        if (index == -1)
            return;

        if(files[index].Captures.Contains(path))
            files[index].Captures.Remove(path);
    }

    public int ExisteixCaptura(string path)
    {
        Debugar.Log($"Existeix {path}?");
        int trobat = -1;
        for (int f = 0; f < files.Count; f++)
        {
            if(files[f].Captures != null && files[f].Captures.Count > 0)
            {
                for (int c = 0; c < files[f].Captures.Count; c++)
                {
                    Debugar.Log($"{files[f].Captures[c]} =? {path}");
                    if (files[f].Captures[c] == path.Replace(@"\", "/")) ;
                    {
                        trobat = f;
                        break;
                    }
                }
                if (trobat != -1)
                    break;
            }
            
        }
        return trobat;
    }


    Estat EstatNomToPrefab(string nom)
    {
        eTrobat = null;
        for (int i = 0; i < estats.Length; i++)
        {
            if (estats[i].name == nom)
            {
                eTrobat = estats[i];
                break;
            }
        }
        return eTrobat;
    }
    Subestat SubestatNomToPrefab(string nom)
    {
        sTrobat = null;
        for (int i = 0; i < subestats.Length; i++)
        {
            if (subestats[i].name == nom)
            {
                sTrobat = subestats[i];
                break;
            }
        }
        return sTrobat;
    }

    private void OnValidate()
    {
        //if (prefab_Ranura == null) prefab_Ranura = XS_Editor.LoadAssetAtPath<GameObject>("Assets/XidoStudio/Hexbase/Pe�a/Prefabs/Ranura.prefab");
        //if (prefab_Pe�a == null) prefab_Pe�a = XS_Editor.LoadAssetAtPath<GameObject>("Assets/XidoStudio/Hexbase/Pe�a/Prefabs/Pe�a.prefab");

        estats = XS_Editor.LoadAllAssetsAtPath<Estat>("Assets/XidoStudio/Hexbase/Peces/Estats").ToArray();
        subestats = XS_Editor.LoadAllAssetsAtPath<Subestat>("Assets/XidoStudio/Hexbase/Peces/Subestats").ToArray();
    }
}

[System.Serializable]
public class SavedFile
{
    [SerializeField] List<string> captures;
    [SerializeField] int mode;
    [SerializeField] List<SavedPe�a> pe�es;


    //INTERN
    int index;
    Fase seguent;
    Grups grups;
    List<Pe�a> creades;
    List<Vector2Int> veins;

    public bool TePeces => pe�es != null && pe�es.Count > 0;
    public List<SavedPe�a> Peces => pe�es;
    public int Mode => mode;

    public List<string> Captures { get => captures; }
    public void AddCaptura(string path)
    {
        if (captures == null) captures = new List<string>();
        captures.Add(path);
    }
    public void RemoveCaptura(string path)
    {
        captures.Remove(path);
    }
    public void Add(Pe�a pe�a, Grups grups)
    {
        if (pe�es == null) pe�es = new List<SavedPe�a>();

        pe�es.Add(new SavedPe�a(pe�a, grups));
    }
    public void Actualitzar(List<Pe�a> pe�es, Grups grups)
    {
        for (int p = 1; p < pe�es.Count; p++)
        {
            for (int tp = 0; tp < this.pe�es.Count; tp++)
            {
                if (pe�es[p].Coordenades == this.pe�es[tp].Coordenada)
                {
                    this.pe�es[tp] = new SavedPe�a(pe�es[p], grups);
                    break;
                }
            }
        }
    }
    public void Save(Grups grups, Pe�a[] peces)
    {
        pe�es = new List<SavedPe�a>();
        for (int i = 0; i < peces.Length; i++)
        {
            pe�es.Add(new SavedPe�a(peces[i], grups));
        }
    }
    public void Load(Grups grups, Fase seguent, System.Func<string, Estat> estatNomToPrefab, System.Func<string, Subestat> subestatNomToPrefab)
    {
        if (this.grups == null) this.grups = grups;
        this.seguent = seguent;

        creades = new List<Pe�a>();

        index = 0;
        Step(estatNomToPrefab, subestatNomToPrefab);
    }

    void Step(System.Func<string, Estat> estatNomToPrefab, System.Func<string, Subestat> subestatNomToPrefab)
    {
        creades.Add(pe�es[index].Load(Grid.Instance, grups, estatNomToPrefab, subestatNomToPrefab));
        Grid.Instance.Dimensionar(creades[index]);
        index++;

        if (index >= pe�es.Count)
        {
            LoadSteps();
            return;
        }

        XS_Coroutine.StartCoroutine_Ending(0.5f, DoStep);

        void DoStep() => Step(estatNomToPrefab, subestatNomToPrefab);
    }

    void LoadSteps()
    {
        //GET VEINS DE TILES
        for (int i = 0; i < creades.Count; i++)
        {
            creades[i].AssignarVeinsTiles(creades[i].Tiles);
        }

        //CREAR RANURES
        for (int i = 0; i < creades.Count; i++)
        {
            veins = Grid.Instance.VeinsCoordenades(creades[i].Coordenades);
            for (int v = 0; v < veins.Count; v++)
            {
                Grid.Instance.CrearRanura(veins[v]);
            }
        }

        //CASES / TREBALLADORS
        for (int i = 0; i < creades.Count; i++)
        {
            /*for (int c = 0; c < creades[i].Cases.Count; c++)
            {
                creades[i].Cases[c].LoadLastStep(grid);
            }*/
        }

        //DETALLS
        for (int i = 0; i < creades.Count; i++)
        {
            for (int t = 0; t < creades[i].Tiles.Length; t++)
            {
                creades[i].Tiles[t].Detalls(creades[i].Subestat);
            }
        }

        //EMPLENAR GRUPS
        for (int i = 0; i < grups.Grup.Count; i++)
        {
            grups.Grup[i].TrobarVeins();
        }

        //PRODUCTES
        for (int i = 0; i < creades.Count; i++)
        {
            creades[i].CoordenadesToProducte(Grid.Instance);
        }

        //DEBUG
        for (int c = 0; c < creades.Count; c++)
        {
            for (int i = 0; i < creades[c].Tiles.Length; i++)
            {
                Debug.LogError($"Tile {i} = {creades[c].Tiles[i].Veins.Length} veins");
            }
        }

        seguent.Iniciar();
    }

}

[System.Serializable] public class SavedPe�a
{
    public SavedPe�a(Pe�a pe�a, Grups grups)
    {
        coordenada = pe�a.Coordenades;
        estat = pe�a.Estat.name;
        subestat = pe�a.Subestat.name;
        producte = pe�a.Extraccio != null ? pe�a.Extraccio.Coordenades : -Vector2Int.one;
        grup = grups.GrupByPe�a(grups.Grup, pe�a);

       /* if(pe�a.CasesCount != 0)
        {
            cases = new SavedCasa[pe�a.CasesCount];
            for (int i = 0; i < pe�a.CasesCount; i++) { cases[i] = pe�a.Cases[i].Save; }
        }*/


        tiles = new SavedTile[]
        {
            pe�a.Tiles[0].Save,
            pe�a.Tiles[1].Save,
            pe�a.Tiles[2].Save,
            pe�a.Tiles[3].Save,
            pe�a.Tiles[4].Save,
            pe�a.Tiles[5].Save
        };
    }

    [SerializeField] string subestat;
    [SerializeField] string estat;
    [SerializeField] Vector2Int coordenada;
    //[SerializeField] Estat esta;
    //[SerializeField] Subestat subestat;

    [SerializeField] Vector2Int producte;
    [SerializeField] Grup grup;
    [SerializeField] SavedCasa[] cases;
    [SerializeField] SavedTile[] tiles;

    public Vector2Int Coordenada => coordenada;

    public Pe�a Load(Grid grid, Grups grups, System.Func<string,Estat> estatNomToPrefab, System.Func<string,Subestat> subestatNomToPrefab)
    {
        //BASE
        GameObject tmp = MonoBehaviour.Instantiate(grid.Prefab_Pe�a, grid.transform);
        tmp.transform.localPosition = GridExtensions.GetWorldPosition(coordenada);

        //PE�A
        Pe�a pe�a = tmp.GetComponent<Pe�a>();
        pe�a.Setup(grid, coordenada, estatNomToPrefab.Invoke(estat), subestatNomToPrefab.Invoke(subestat));
        pe�a.name = $"{pe�a.Estat.name}({pe�a.Coordenades})";

        //CREAR TILES
        pe�a.CrearTilesPotencials();
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].Load(pe�a);
        }
        pe�a.CrearTilesFisics();

        //CREAR GRUP si cal
        grups.CrearGrups_FromLoad(grup, pe�a);

        //CASES
        if(cases != null)
        {
           /* for (int i = 0; i < cases.Length; i++)
            {
                pe�a.AddCasa(cases[i].Load());
            }*/
        }
       

        grid.Set(pe�a);

        //VEINS
        pe�a.AssignarVeinsTiles(pe�a.Tiles);

        if(producte != -Vector2Int.one) pe�a.SetCoordenadesProducte = producte;
        //pe�a.CrearDetalls();

        return pe�a;
    }

    //ON LOAD
    //Grup, Si no hi es l'index que es busca, es crea.
    //Crear ranures despres de crear totes les peces.
}



[System.Serializable] public class SavedTile
{
    public SavedTile(Tile tile, int orientacio, int orientacioFisica)
    {
        this.tile = tile;
        this.orientacio = orientacio;
        this.orientacioFisica = orientacioFisica;
    }

    [SerializeField] Tile tile;
    [SerializeField] int orientacio;
    [SerializeField] int orientacioFisica;

    public void Load(Pe�a pe�a)
    {
        pe�a.Tiles[orientacio] = new TilePotencial(pe�a, orientacio);
        pe�a.Tiles[orientacio].Escollir(tile, orientacioFisica);
        //pe�a.Tiles[orientacio].Assegurat = true;
    }

    //ON LOAD
    //Instanciar detalls despres de crear-los tots.
    //Aconseguir els veins(TilesPotencial) del tile despres de crear-los tots.
}



[System.Serializable] public class SavedCasa
{

    public SavedCasa(Casa.Necessitat[] necessitats, Vector2Int coordPe�a)
    {
        this.necessitats = necessitats;
        this.coordPe�a = coordPe�a;
    }

    [SerializeField] Casa.Necessitat[] necessitats;
    [SerializeField] Vector2Int coordPe�a;

    public Casa Load()
    {
        return new Casa(coordPe�a, necessitats);
    }

    //ON LOAD
    //Buscar la pe�a i la feina despres de crear-les totes.
}
