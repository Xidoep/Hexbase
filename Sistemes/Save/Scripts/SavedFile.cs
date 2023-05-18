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
    [SerializeField] List<SavedPeça> peçes;


    //INTERN
    int index;
    Fase seguent;
    Grups grups;
    List<Peça> creades;
    List<Vector2Int> veins;
    System.Action enCarregat;

    float _tempsRanura = 0;

    public bool TePeces => peçes != null && peçes.Count > 0;
    public List<SavedPeça> Peces => peçes;
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
    public void Save(Grups grups, Peça[] peces)
    {
        peçes = new List<SavedPeça>();
        for (int i = 0; i < peces.Length; i++)
        {
            peçes.Add(new SavedPeça(peces[i], grups));
        }
    }
    public void Load(Grups grups, Fase seguent, System.Func<string, Estat> estatNomToPrefab, System.Func<string, Subestat> subestatNomToPrefab, System.Func<string, Producte> producteNomToPrefab, System.Func<string, Tile> tileNomToPrefab, System.Action<Peça> animacio, System.Action enCarregat)
    {
        if (this.grups == null) this.grups = grups;
        this.seguent = seguent;

        creades = new List<Peça>();

        index = 0;

        this.enCarregat = enCarregat;
        Step(estatNomToPrefab, subestatNomToPrefab, producteNomToPrefab, tileNomToPrefab, animacio);
    }
    public void SetMode(Mode mode) => this.mode = (int)mode;
    public void SetNivell(int nivell) => this.nivell = nivell;
    public void SetExperiencia(int experiencia) => this.experiencia = experiencia;
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

    void Step(System.Func<string, Estat> estatNomToPrefab, System.Func<string, Subestat> subestatNomToPrefab, System.Func<string, Producte> producteNomToPrefab, System.Func<string, Tile> tileNomToPrefab, System.Action<Peça> animacio)
    {
        creades.Add(peçes[index].Load(Grid.Instance, grups, estatNomToPrefab, subestatNomToPrefab, producteNomToPrefab, tileNomToPrefab, animacio));
        Grid.Instance.Dimensionar(creades[index]);
        index++;

        if (index >= peçes.Count)
        {
            LastStep();
            return;
        }

        XS_Coroutine.StartCoroutine_Ending_FrameDependant(2f / peçes.Count, DoStep);

        void DoStep() => Step(estatNomToPrefab, subestatNomToPrefab, producteNomToPrefab, tileNomToPrefab, animacio);
    }


    void LastStep()
    {
        //GET VEINS DE TILES
        for (int i = 0; i < creades.Count; i++)
        {
            creades[i].AssignarVeinsTiles(creades[i].Tiles);
        }

        _tempsRanura = 0;
        //CREAR RANURES
        for (int i = 0; i < creades.Count; i++)
        {
            veins = Grid.Instance.VeinsCoordenades(creades[i].Coordenades);
            for (int v = 0; v < veins.Count; v++)
            {
                _tempsRanura += 0.05f;
                //Grid.Instance.CrearRanura(veins[v]);
                XS_Coroutine.StartCoroutine_Ending_FrameDependant(_tempsRanura, Grid.Instance.CrearRanura, veins[v]);
            }
        }

        //DETALLS
        for (int i = 0; i < creades.Count; i++)
        {
            for (int t = 0; t < creades[i].Tiles.Length; t++)
            {
                creades[i].Tiles[t].Detalls(creades[i].Detalls);
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
            //if (creades[i].ConnexioCoordenada.EsNula())
            //    return;

            //((Peça)Grid.Instance.Get(creades[i].ConnexioCoordenada)).Connectar(creades[i]);
        }

        //DEBUG
        /*for (int c = 0; c < creades.Count; c++)
        {
            for (int i = 0; i < creades[c].Tiles.Length; i++)
            {
                Debug.LogError($"Tile {i} = {creades[c].Tiles[i].Veins.Length} veins");
            }
        }*/

        enCarregat?.Invoke();

        if(seguent != null) seguent.Iniciar();


    }
}
