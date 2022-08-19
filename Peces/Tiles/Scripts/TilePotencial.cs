using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[System.Serializable]
public class TilePotencial
{
    public TilePotencial(Estat estat, Pe�a pe�a, int orientacio)
    {
        this.pe�a = pe�a;
        this.estat = estat;
        estatName = estat.name;
        this.orientacio = orientacio;
        //coordenades = pe�a.Coordenades;
        connexions = estat.ConnexionsPossibles;
        //interaccions = 0;
        //assegurat = false;
        //haFallat = false;

        GetPossibilitats();
        /*orientacioFisica = 0;

        if(estat != null)
        {
            //ConnexionsNules = estat.VeiNull;
            //PossibilitatsInicials = estat.Possibilitats;
            //possibilitats = PossibilitatsInicials.Invoke();
            possibilitatsVirtuals = estat.PossibilitatsVirutals();
        }*/
    }

    Pe�a pe�a;
    Estat estat;
    [SerializeField] string estatName;
    [SerializeField] int orientacio;
    //Vector2Int coordenades;
    //Tile[] possibilitats;
    [SerializeField] Possibilitats possibilitatsVirtuals;
    Connexio[] connexions;
    int orientacioFisica = 0;
    //int interaccions = 0;
    //bool assegurat;
    //bool haFallat;


    [SerializeField] GameObject tileFisic;
    GameObject detalls;
    [HideInInspector] TilePotencial[] veins;
    
    
    
    //public Func<TilePotencial, Connexio[]> ConnexionsNules;
    //public Func<Tile[]> PossibilitatsInicials;
    //public string ID => $"{(estat ? estat.name : "???")}-{orientacio}({(Resolt ? possibilitats[0] : "???")})|{orientacioFisica} {(assegurat ? "(ASSEGURAT)" : "")}";
    /*public bool Assegurat
    {
        get => assegurat;
        set
        {
            assegurat = value;
            if (tileFisic != null)
            {
                //tileFisic.name = ID;
                tileFisic.GetComponent<MeshRenderer>().material.color = assegurat ? Color.grey : Color.gray * 0.5f;
            }
        }
    }*/
    public string EstatName => $"{estatName}-{orientacio}";
    public Pe�a Pe�a => pe�a;
    public Estat Estat => estat;
    //public Vector2Int Coordenades => coordenades;
    //public Tile[] Possibilitats => possibilitats;
    public Possibilitats PossibilitatsVirtuals => possibilitatsVirtuals;
    //public void Interactuar() => interaccions++;
    //public int Interaccions => interaccions;
    public GameObject TileFisic => tileFisic;
    public TilePotencial[] Veins => veins;
    public Connexio[] Connexions => connexions;


    /*public int VeinsResolts
    {
        get
        {
            int tmp = 0;
            for (int i = 0; i < Veins.Length; i++)
            {
                if (Veins[i] == null)
                    continue;

                if (Veins[i].Resolt) tmp++;
            }
            return tmp;
        }
    }*/
    float AngleOrientacioFisica
    {
        get
        {
            switch (orientacioFisica)
            {
                default:
                    return 0;
                case 1:
                    return -120;
                case 2:
                    return 120;
            }
        }
    }
    public bool Resolt => possibilitatsVirtuals.Count == 1;
    public int OrientacioFisica => orientacioFisica;
    public int Orientacio => orientacio;
    //public bool HaFallat { get => haFallat; set => haFallat = value; }

    public SavedTile Save => new SavedTile(PossibilitatsVirtuals.Get(0).Tile, orientacio, orientacioFisica);


    //INTERN

    public void GetPossibilitats()
    {
        if (estat == null || pe�a.Subestat == null)
            return;
        orientacioFisica = 0;

        if (pe�a.Subestat.TilesPropis) 
            possibilitatsVirtuals = estat.Possibilitats(pe�a.Subestat.TilesAlternatius);
        else possibilitatsVirtuals = estat.Possibilitats();
    }
    //public void SetPossiblitats(Possibilitats possibilitats) => this.possibilitats = possibilitats.ToArray();

    public TilePotencial Ambiguo()
    {
        //??? potser podria anar aixo aqui...
        //if (Resolt)
        //    return;

        /*if (inicial) 
        {
            interaccions = 0;
            haFallat = false;
        } */

        #region DEBUG
        if (tileFisic) 
        {
            XS_InstantiateGPU.RemoveGrafic(tileFisic);
            MonoBehaviour.Destroy(tileFisic);
        } 
        #endregion

        GetPossibilitats();
        //assegurat = false;
        //interaccions = 0;

        //Debug.Log($"{ID} Ambiguo");

        GetVeins(pe�a);
        for (int i = 0; i < veins.Length; i++)
        {
            veins[i]?.GetVeins(veins[i]?.Pe�a);
        }

        return this;
        //WaveFunctionColapse.Add(this);

    }

    public void Escollir()
    {
        int r = 0;

        if (possibilitatsVirtuals.Count > 1)
        {
            int randomMax = 0;

            List<Random> randoms = new List<Random>();

            for (int i = 0; i < possibilitatsVirtuals.Count; i++)
            {
                randoms.Add(new Random() { index = i, rang = new Vector2Int(randomMax + (i > 0 ? 1 : 0), randomMax + possibilitatsVirtuals.Get(i).Pes) });
                randomMax += possibilitatsVirtuals.Get(i).Pes;
            }
            int randomNumber = UnityEngine.Random.Range(0, randomMax);
            //Debug.LogError($"GIVEN RANDOM NUMBER = {randomNumber}");
            for (int i = 0; i < randoms.Count; i++)
            {
                //Debug.LogError($"P{i} RANDOM {randoms[i].rang}");
                if (randomNumber == Mathf.Clamp(randomNumber, randoms[i].rang.x, randoms[i].rang.y))
                {
                    r = randoms[i].index;
                    //break;
                }
            }
            Debug.Log($"RANDOM = {r}");
        }
       
        Escollir(possibilitatsVirtuals.Tile(r), possibilitatsVirtuals.Orietacio(r));
    }

    public void Escollir(Tile tile, int orientacioFisica)
    {
        this.orientacioFisica = orientacioFisica;
        this.possibilitatsVirtuals = new Possibilitats(tile, orientacioFisica, 1000);

        #region DEBUG
        Crear();
        #endregion
    }

    public void GetVeins(Pe�a pe�a)
    {
        List<Hexagon> PecesVeines = pe�a.Veins;
        Pe�a hexagonVei = PecesVeines[orientacio] != null ? (PecesVeines[orientacio].EsPe�a ? (Pe�a)PecesVeines[orientacio] : null) : null;

        veins = new TilePotencial[]
        {
            hexagonVei != null ? hexagonVei.GetTile((orientacio + 3) % 6) : null,
            pe�a.GetTile((orientacio + 1) % 6),
            pe�a.GetTile((orientacio - 1 >= 0) ? orientacio - 1 : 5)
        };
    }
    

    public Connexio[] GetConnexions(Possibilitat p) => new Connexio[] { p.Tile.Exterior(p.Orientacio), p.Tile.Esquerra(p.Orientacio), p.Tile.Dreta(p.Orientacio) };
    public bool CompararConnexions(Possibilitat possibilitat, Connexio exterior, Connexio esquerra, Connexio dreta) =>
        Igualar(exterior, possibilitat.Tile.Exterior(possibilitat.Orientacio)) &&
        Igualar(esquerra, possibilitat.Tile.Esquerra(possibilitat.Orientacio)) &&
        Igualar(dreta, possibilitat.Tile.Dreta(possibilitat.Orientacio));


    bool Igualar(Connexio a, Connexio b) => a.Viable == b && b.Viable == a;

    public void Crear()
    {
        if (tileFisic != null) 
        {
            XS_InstantiateGPU.RemoveGrafic(tileFisic);
            MonoBehaviour.Destroy(TileFisic);
        }

        //XS_InstantiateGPU.Instantiate(PossibilitatsVirtuals.Get(0).Tile.Prefab);

        tileFisic = GameObject.Instantiate(PossibilitatsVirtuals.Get(0).Tile.Prefab, pe�a.Parent.position, Quaternion.Euler(0, orientacio * 60, 0), pe�a.Parent);
        tileFisic.name = $"{estatName}-{orientacio}";

        tileFisic.transform.localEulerAngles = new Vector3(0, orientacio * 60, 0) + new Vector3(0, AngleOrientacioFisica, 0);
        if (orientacioFisica != 0)
            tileFisic.transform.position = pe�a.Parent.position - tileFisic.transform.forward * GridExtensions.GetWorldPosition(0, 0).z + (tileFisic.transform.right * 0.5f) * (orientacioFisica == 1 ? 1 : -1);

        //tileFisic.GetComponent<MeshRenderer>().material.color = assegurat ? Color.grey : Color.gray * 0.5f;
        XS_InstantiateGPU.AddGrafics(tileFisic);
        
        tileFisic.AddComponent<TileDebug>().New(veins, orientacioFisica);

        //Detalls(pe�a.Subestat);
    }

    public void Detalls(Subestat subestat)
    {
        if(detalls != null)
        {
            MonoBehaviour.Destroy(detalls);
        }

        if (subestat.Detalls == null || subestat.Detalls.Length == 0)
            return;

        for (int d = 0; d < subestat.Detalls.Length; d++)
        {
            int[] tiles = subestat.Detalls[d].Tiles(pe�a);
            for (int t = 0; t < tiles.Length; t++)
            {
                //Tiles del detalls no retorna tots els tiles, nom�s els que s'han de instanciar. Aqui es mira si aquest est� a la llista.
                if(tiles[t] == orientacio)
                {
                    GameObject _detall = subestat.Detalls[d].GameObject(pe�a, this);
                    if (_detall == null)
                        continue;

                    //detalls = GameObject.Instantiate(subestat.detalls[d].GameObject(pe�a), tileFisic.transform.position, Quaternion.identity, tileFisic.transform);
                    detalls = GameObject.Instantiate(_detall, tileFisic.transform.position, Quaternion.identity);
                    for (int m = 0; m < subestat.Detalls[d].Modificacios.Length; m++)
                    {
                        subestat.Detalls[d].Modificacios[m].Modificar(this, detalls);
                    }
                    detalls.transform.SetParent(tileFisic.transform);
                }
            }
        
            
        }

    }
    public struct Random
    {
        public int index;
        public Vector2Int rang;
    }
}


public class TileDebug : MonoBehaviour
{
    [SerializeField] TilePotencial[] veins;
    [SerializeField] int orientacioFisica;
    public TileDebug New(TilePotencial[] veins, int orientacioFisica) 
    {
        this.veins = veins;
        this.orientacioFisica = orientacioFisica;
        return this;
    }
}

