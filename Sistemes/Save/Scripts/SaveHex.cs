using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Save")]
public class SaveHex : ScriptableObject
{
    [SerializeField] Grups grups;
    [Linia]
    [SerializeField] string nom;
    [SerializeField] List<SavedPeça> peçes;
    //Altres coses relacionades amb la partida.

    //INTERN
    int index;
    Grid grid;
    List<Peça> creades;
    List<Vector2Int> veins;

    public void Add(Peça peça)
    {
        if (peçes == null) peçes = new List<SavedPeça>();

        peçes.Add(new SavedPeça(peça));
    }

    [ContextMenu("Save")]
    public void Save()
    {
        peçes = new List<SavedPeça>();
        Peça[] tmp = FindObjectsOfType<Peça>();
        for (int i = 0; i < tmp.Length; i++)
        {
            peçes.Add(new SavedPeça(tmp[i]));
        }
    }

    [ContextMenu("Load")]
    public void Load()
    {
        grid = (Grid)GameObject.FindObjectOfType<Grid>();
        creades = new List<Peça>();

        //CREAR PECES INDICIDUALS
        for (int i = 0; i < peçes.Count; i++)
        {
            creades.Add(peçes[i].Load(grid));
        }

        //GET VEINS DE TILES
        for (int i = 0; i < creades.Count; i++)
        {
            creades[i].AssignarVeinsTiles(creades[i].Tiles);

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

        //CREAR RANURES
        for (int i = 0; i < creades.Count; i++)
        {
            veins = grid.VeinsCoordenades(creades[i].Coordenades);
            for (int v = 0; v < veins.Count; v++)
            {
                grid.CrearRanura(veins[v]);
            }
        }

        //EMPLENAR GRUPS
        grups.CrearGrups_FromLoad(creades);

        //DEBUG
        for (int c = 0; c < creades.Count; c++)
        {
            for (int i = 0; i < creades[c].Tiles.Length; i++)
            {
                Debug.LogError($"Tile {i} = {creades[c].Tiles[i].Veins.Length} veins");
            }
        }

        //WaveFunctionColapse.StartPendents();
        //Fer les accions que s'han de fer al final.
    }

    void CrearPeces()
    {
        index = 0;
        XS_Coroutine.StartCoroutine_Ending(0.1f, Step_CrearPeces);
    }

    void Step_CrearPeces()
    {
        creades.Add(peçes[index].Load(grid));

        index++;

        if (index > peçes.Count)
        {
            GetVeins();
            return;
        }

        XS_Coroutine.StartCoroutine_Ending(0.1f, Step_CrearPeces);
    }

    void GetVeins()
    {
        index = 0;
        XS_Coroutine.StartCoroutine_Ending(0.1f, Step_GetVeins);
    }

    void Step_GetVeins()
    {
        creades[index].AssignarVeinsTiles(creades[index].Tiles);

        index++;

        if (index > creades.Count)
        {
            GetVeins();
            return;
        }

        XS_Coroutine.StartCoroutine_Ending(0.1f, Step_GetVeins);
    }
}



[System.Serializable]
public class SavedPeça
{
    public SavedPeça(Peça peça)
    {
        coordenada = peça.Coordenades;
        esta = peça.Estat;
        subestat = peça.Subestat;
        //grup = peça.Grup;

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

    [SerializeField] Vector2Int coordenada;
    [SerializeField] Estat esta;
    [SerializeField] Subestat subestat;
    [SerializeField] int grup;
    [SerializeField] SavedCasa[] cases;
    [SerializeField] SavedTile[] tiles;
    
    public Peça Load(Grid grid)
    {
        //BASE
        GameObject tmp = MonoBehaviour.Instantiate(grid.Prefab_Peça, grid.transform);
        tmp.transform.localPosition = GridExtensions.GetWorldPosition(coordenada.x, coordenada.y);

        //PEÇA
        Peça peça = tmp.GetComponent<Peça>();
        peça.Setup(grid, coordenada, esta, subestat);
        peça.name = $"{peça.Estat.name}({peça.Coordenades})";

        //CREAR TILES
        peça.CrearTilesPotencials();
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].Load(peça);
        }
        peça.CrearTilesFisics();

        //PASSAR GRUP
        // peça.Grup = grup;

        //CASES
        for (int i = 0; i < cases.Length; i++)
        {
            peça.AddCasa(cases[i].Load());
        }
        //VEINS
        peça.AssignarVeinsTiles(peça.Tiles);



        grid.Set(peça);

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

    public SavedCasa(Casa.Necessitat[] necessitats, int nivell, Vector2Int coordPeça, Vector2Int coordFeina)
    {
        this.necessitats = necessitats;
        this.nivell = nivell;
        this.coordPeça = coordPeça;
        this.coordFeina = coordFeina;
    }

    [SerializeField] Casa.Necessitat[] necessitats;
    [SerializeField] int nivell;
    [SerializeField] Vector2Int coordPeça;
    [SerializeField] Vector2Int coordFeina;

    public Casa Load()
    {
        return new Casa(coordPeça, coordFeina, nivell, necessitats);
    }

    //ON LOAD
    //Buscar la peça i la feina despres de crear-les totes.
}

