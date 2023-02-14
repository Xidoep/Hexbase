using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[System.Serializable]
public class SavedFile
{
    [SerializeField] List<string> captures;
    [SerializeField] string nom;
    [SerializeField] int mode;
    [SerializeField] int experiencia;
    [SerializeField] int nivell;
    [SerializeField] List<string> pila;
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
    public int Experiencia => experiencia;
    public int Nivell => nivell;
    public bool PilaPlena => pila != null && pila.Count > 0;

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
    public void Load(Grups grups, Fase seguent, System.Func<string, Estat> estatNomToPrefab, System.Func<string, Subestat> subestatNomToPrefab, System.Func<string, Producte> producteNomToPrefab)
    {
        if (this.grups == null) this.grups = grups;
        this.seguent = seguent;

        creades = new List<Pe�a>();

        index = 0;
        Step(estatNomToPrefab, subestatNomToPrefab, producteNomToPrefab);
    }
    public void SetMode(Mode mode) => this.mode = (int)mode;
    public void SetExperienciaNivell(int experiencia, int nivell)
    {
        this.experiencia = experiencia;
        this.nivell = nivell;
    }
    public void AddPila(Estat estat)
    {
        if (pila == null) pila = new List<string>();
        pila.Add(estat.name);
    }
    public void RemoveLastPila()
    {
        if(pila != null) 
            pila.RemoveAt(0);
    }
    public List<Estat> Pila(System.Func<string, Estat> estatNomToPrefab)
    {
        List<Estat> estats = new List<Estat>();
        for (int i = 0; i < pila.Count; i++)
        {
            estats.Add(estatNomToPrefab(pila[i]));
        }
        return estats;
    }

    void Step(System.Func<string, Estat> estatNomToPrefab, System.Func<string, Subestat> subestatNomToPrefab, System.Func<string, Producte> producteNomToPrefab)
    {
        creades.Add(pe�es[index].Load(Grid.Instance, grups, estatNomToPrefab, subestatNomToPrefab, producteNomToPrefab));
        Grid.Instance.Dimensionar(creades[index]);
        index++;

        if (index >= pe�es.Count)
        {
            LoadSteps();
            return;
        }

        XS_Coroutine.StartCoroutine_Ending(0.5f, DoStep);

        void DoStep() => Step(estatNomToPrefab, subestatNomToPrefab, producteNomToPrefab);
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
        /*for (int i = 0; i < creades.Count; i++)
        {
            for (int c = 0; c < creades[i].Cases.Count; c++)
            {
                creades[i].Cases[c].LoadLastStep(grid);
            }
        }*/

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
