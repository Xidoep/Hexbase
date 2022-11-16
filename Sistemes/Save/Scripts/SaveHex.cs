using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Save")]
public class SaveHex : ScriptableObject
{

    [SerializeField] int current = 0;
    [SerializeField] List<SavedFile> files;



    public List<SavedFile> Files => files;
    public void NovaPartida()
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
    public int Current => current;
    public SavedFile CurrentSavedFile => files[current];
    public bool TePeces 
    {
        get 
        {
            if (files == null || files.Count == 0) files = new List<SavedFile>() {new SavedFile() };
            if (current > files.Count - 1) current = 0;
            return files[current].TePeces;
        
        }
    } 

    public void Add(Peça peça, Grups grups) => files[current].Add(peça, grups);

    public void Actualitzar(List<Peça> peçes, Grups grups) => files[current].Actualitzar(peçes, grups);


    [ContextMenu("Save")]
    public void Save(Grups grups) => files[current].Save(grups, FindObjectsOfType<Peça>());

    [ContextMenu("Load")]
    public void Load(Grups grups, Fase colocar) => files[current].Load(grups, colocar);

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
            for (int c = 0; c < files[f].Captures.Count; c++)
            {
                Debugar.Log($"{files[f].Captures[c]} =? {path}");
                if(files[f].Captures[c] == path.Replace(@"\","/"));
                {
                    trobat = f;
                    break;
                }
            }
            if (trobat != -1)
                break;
        }
        return trobat;
    }

}

[System.Serializable]
public class SavedFile
{
    [SerializeField] List<string> captures;
    [SerializeField] List<SavedPeça> peçes;

    //INTERN
    int index;
    Fase colocar;
    Grups grups;
    Grid grid;
    List<Peça> creades;
    List<Vector2Int> veins;

    public bool TePeces => peçes != null && peçes.Count > 0;

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
    public void Add(Peça peça, Grups grups)
    {
        if (peçes == null) peçes = new List<SavedPeça>();

        peçes.Add(new SavedPeça(peça, grups));
    }
    public void Actualitzar(List<Peça> peçes, Grups grups)
    {
        for (int p = 1; p < peçes.Count; p++)
        {
            for (int tp = 0; tp < this.peçes.Count; tp++)
            {
                if (peçes[p].Coordenades == this.peçes[tp].Coordenada)
                {
                    this.peçes[tp] = new SavedPeça(peçes[p], grups);
                    break;
                }
            }
        }
    }

    [ContextMenu("Save")]
    public void Save(Grups grups, Peça[] peces)
    {
        peçes = new List<SavedPeça>();
        for (int i = 0; i < peces.Length; i++)
        {
            peçes.Add(new SavedPeça(peces[i], grups));
        }
    }

    [ContextMenu("Load")]
    public void Load(Grups grups, Fase colocar)
    {
        if (grid == null) grid = (Grid)GameObject.FindObjectOfType<Grid>();
        if (this.grups == null) this.grups = grups;
        if (this.colocar == null) this.colocar = colocar;

        creades = new List<Peça>();

        index = 0;
        Step();
    }

    void Step()
    {
        creades.Add(peçes[index].Load(grid, grups));
        grid.Dimensionar(creades[index]);
        index++;

        if (index >= peçes.Count)
        {
            LoadSteps();
            return;
        }

        XS_Coroutine.StartCoroutine_Ending(0.1f, Step);
    }

    void LoadSteps()
    {
        //grid = (Grid)GameObject.FindObjectOfType<Grid>();
        //creades = new List<Peça>();

        //CREAR PECES INDIVIDUALS
        /*for (int i = 0; i < peçes.Count; i++)
        {
            creades.Add(peçes[i].Load(grid, grups));
        }*/

        //GET VEINS DE TILES
        for (int i = 0; i < creades.Count; i++)
        {
            creades[i].AssignarVeinsTiles(creades[i].Tiles);
        }

        //CREAR RANURES
        for (int i = 0; i < creades.Count; i++)
        {
            veins = grid.VeinsCoordenades(creades[i].Coordenades);
            for (int v = 0; v < veins.Count; v++)
            {
                grid.CrearRanura(veins[v]);
            }
        }

        //CASES / TREBALLADORS
        for (int i = 0; i < creades.Count; i++)
        {
            for (int c = 0; c < creades[i].Cases.Count; c++)
            {
                creades[i].Cases[c].LoadLastStep(grid);
            }
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
            creades[i].CoordenadesToProducte(grid);
        }

        //DEBUG
        /*for (int c = 0; c < creades.Count; c++)
        {
            for (int i = 0; i < creades[c].Tiles.Length; i++)
            {
                Debug.LogError($"Tile {i} = {creades[c].Tiles[i].Veins.Length} veins");
            }
        }*/

        colocar.Iniciar();
    }

}

[System.Serializable] public class SavedPeça
{
    public SavedPeça(Peça peça, Grups grups)
    {
        coordenada = peça.Coordenades;
        esta = peça.Estat.name;
        subestat = peça.Subestat.name;
        producte = peça.TeProducte ? peça.ProducteCoordenades : -Vector2Int.one;
        grup = grups.GrupByPeça(peça);

        if(peça.CasesCount != 0)
        {
            cases = new SavedCasa[peça.CasesCount];
            for (int i = 0; i < peça.CasesCount; i++) { cases[i] = peça.Cases[i].Save; }
        }


        tiles = new SavedTile[]
        {
            peça.Tiles[0].Save,
            peça.Tiles[1].Save,
            peça.Tiles[2].Save,
            peça.Tiles[3].Save,
            peça.Tiles[4].Save,
            peça.Tiles[5].Save
        };
    }

    [SerializeField] string subestat;
    [SerializeField] string esta;
    [SerializeField] Vector2Int coordenada;
    //[SerializeField] Estat esta;
    //[SerializeField] Subestat subestat;

    [SerializeField] Vector2Int producte;
    [SerializeField] Grup grup;
    [SerializeField] SavedCasa[] cases;
    [SerializeField] SavedTile[] tiles;

    public Vector2Int Coordenada => coordenada;

    public Peça Load(Grid grid, Grups grups)
    {
        //BASE
        GameObject tmp = MonoBehaviour.Instantiate(grid.Prefab_Peça, grid.transform);
        tmp.transform.localPosition = GridExtensions.GetWorldPosition(coordenada.x, coordenada.y);

        //PEÇA
        Peça peça = tmp.GetComponent<Peça>();
        peça.Setup(grid, coordenada, grid.Estat(esta), grid.Subestat(subestat));
        peça.name = $"{peça.Estat.name}({peça.Coordenades})";

        //CREAR TILES
        peça.CrearTilesPotencials();
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].Load(peça);
        }
        peça.CrearTilesFisics(false);

        //CREAR GRUP si cal
        grups.CrearGrups_FromLoad(grup, peça);

        //CASES
        if(cases != null)
        {
            for (int i = 0; i < cases.Length; i++)
            {
                peça.AddCasa(cases[i].Load());
            }
        }
       

        grid.Set(peça);

        //VEINS
        peça.AssignarVeinsTiles(peça.Tiles);

        if(producte != -Vector2Int.one) peça.SetCoordenadesProducte = producte;
        //peça.CrearDetalls();

        return peça;
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

    public void Load(Peça peça)
    {
        peça.Tiles[orientacio] = new TilePotencial(peça, orientacio);
        peça.Tiles[orientacio].Escollir(tile, orientacioFisica);
        //peça.Tiles[orientacio].Assegurat = true;
    }

    //ON LOAD
    //Instanciar detalls despres de crear-los tots.
    //Aconseguir els veins(TilesPotencial) del tile despres de crear-los tots.
}



[System.Serializable] public class SavedCasa
{

    public SavedCasa(Casa.Necessitat[] necessitats, int nivell, Vector2Int coordPeça)
    {
        this.necessitats = necessitats;
        this.nivell = nivell;
        this.coordPeça = coordPeça;
    }

    [SerializeField] Casa.Necessitat[] necessitats;
    [SerializeField] int nivell;
    [SerializeField] Vector2Int coordPeça;

    public Casa Load()
    {
        return new Casa(coordPeça, nivell, necessitats);
    }

    //ON LOAD
    //Buscar la peça i la feina despres de crear-les totes.
}
