using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Save")]
public class SaveHex : ScriptableObject
{
    [SerializeField] Grups grups;
    [SerializeField] Fase colocar;
    [Linia]
    [SerializeField] string nom;
    [SerializeField] List<SavedPe�a> pe�es;

    //INTERN
    int index;
    Grid grid;
    List<Pe�a> creades;
    List<Vector2Int> veins;

    public void Add(Pe�a pe�a)
    {
        if (pe�es == null) pe�es = new List<SavedPe�a>();

        pe�es.Add(new SavedPe�a(pe�a, grups));
    }

    [ContextMenu("Save")]
    public void Save()
    {
        pe�es = new List<SavedPe�a>();
        Pe�a[] tmp = FindObjectsOfType<Pe�a>();
        for (int i = 0; i < tmp.Length; i++)
        {
            pe�es.Add(new SavedPe�a(tmp[i],grups));
        }
    }

    [ContextMenu("Load")]
    void CrearPeces()
    {
        grid = (Grid)GameObject.FindObjectOfType<Grid>();
        creades = new List<Pe�a>();

        index = pe�es.Count -1;
        XS_Coroutine.StartCoroutine_Ending(0.1f, Step);
    }

    void Step()
    {
        creades.Add(pe�es[index].Load(grid, grups));
        index--;

        if (index < 0)
        {
            Load();
            return;
        }

        XS_Coroutine.StartCoroutine_Ending(0.1f, Step);
    }


    public void Load()
    {
        //grid = (Grid)GameObject.FindObjectOfType<Grid>();
        //creades = new List<Pe�a>();

        //CREAR PECES INDIVIDUALS
        /*for (int i = 0; i < pe�es.Count; i++)
        {
            creades.Add(pe�es[i].Load(grid, grups));
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



[System.Serializable]
public class SavedPe�a
{
    public SavedPe�a(Pe�a pe�a, Grups grups)
    {
        coordenada = pe�a.Coordenades;
        esta = pe�a.Estat;
        subestat = pe�a.Subestat;
        grup = grups.GrupByPe�a(pe�a);

        if(pe�a.CasesCount != 0)
        {
            cases = new SavedCasa[pe�a.CasesCount];
            for (int i = 0; i < pe�a.CasesCount; i++) { cases[i] = pe�a.Cases[i].Save; }
        }


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

    [SerializeField] Vector2Int coordenada;
    [SerializeField] Estat esta;
    [SerializeField] Subestat subestat;
    [SerializeField] Grup grup;
    [SerializeField] SavedCasa[] cases;
    [SerializeField] SavedTile[] tiles;
    


    public Pe�a Load(Grid grid, Grups grups)
    {
        //BASE
        GameObject tmp = MonoBehaviour.Instantiate(grid.Prefab_Pe�a, grid.transform);
        tmp.transform.localPosition = GridExtensions.GetWorldPosition(coordenada.x, coordenada.y);

        //PE�A
        Pe�a pe�a = tmp.GetComponent<Pe�a>();
        pe�a.Setup(grid, coordenada, esta, subestat);
        pe�a.name = $"{pe�a.Estat.name}({pe�a.Coordenades})";

        //CREAR TILES
        pe�a.CrearTilesPotencials();
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].Load(pe�a);
        }
        pe�a.CrearTilesFisics(false);

        //CREAR GRUP si cal
        grups.CrearGrups_FromLoad(grup, pe�a);

        //CASES
        for (int i = 0; i < cases.Length; i++)
        {
            pe�a.AddCasa(cases[i].Load());
        }

        grid.Set(pe�a);

        //VEINS
        pe�a.AssignarVeinsTiles(pe�a.Tiles);

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

    public SavedCasa(Casa.Necessitat[] necessitats, int nivell, Vector2Int coordPe�a, Vector2Int coordFeina)
    {
        this.necessitats = necessitats;
        this.nivell = nivell;
        this.coordPe�a = coordPe�a;
        this.coordFeina = coordFeina;
    }

    [SerializeField] Casa.Necessitat[] necessitats;
    [SerializeField] int nivell;
    [SerializeField] Vector2Int coordPe�a;
    [SerializeField] Vector2Int coordFeina;

    public Casa Load()
    {
        return new Casa(coordPe�a, coordFeina, nivell, necessitats);
    }

    //ON LOAD
    //Buscar la pe�a i la feina despres de crear-les totes.
}
