using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[System.Serializable]
public class TilePotencial
{
    public TilePotencial(Pe�a pe�a, int orientacio)
    {
        this.pe�a = pe�a;
        this.orientacio = orientacio;

        GetPossibilitats();
    }

    [HideInInspector] Pe�a pe�a;

    [HideInInspector] int orientacio;
    [SerializeField] Possibilitats possibilitatsVirtuals;
    int orientacioFisica = 0;

    [SerializeField] GameObject tileFisic;
    GameObject detalls;
    [HideInInspector] TilePotencial[] veins;
    [SerializeField] int altura = -1;
    public int altura0 = -1;
    public int altura1 = -1;
    public int altura2 = -1;

    public string EstatName => $"{pe�a.name}({pe�a.Subestat.name})-{orientacio}";
    public Pe�a Pe�a => pe�a;
    public bool TePossibilitats => possibilitatsVirtuals.Count > 0;
    public Possibilitats PossibilitatsVirtuals => possibilitatsVirtuals;
    public GameObject TileFisic => tileFisic;
    public TilePotencial[] Veins => veins;
    public int Altura { get => altura; set => altura = value; }
    public bool TeAltura => altura != -1;

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
        if (pe�a.EsEstatNull || pe�a.EsSubestatNull)
            return;
        orientacioFisica = 0;

        possibilitatsVirtuals = pe�a.Possibilitats;

    }

    public TilePotencial Ambiguo()
    {
        if (WaveFunctionColpaseScriptable.veureProces)
        {
            if (tileFisic)
            {
                MonoBehaviour.Destroy(tileFisic);
            }
        }

        GetPossibilitats();

        GetVeins(pe�a);
        for (int i = 0; i < veins.Length; i++)
        {
            veins[i]?.GetVeins(veins[i]?.pe�a);
        }

        altura = -1;

        return this;
    }

    Possibilitats possibilitatsActuals;
    Possibilitats possibilitatsComodi;
    Possibilitats novesPossibilitats;
    public TilePotencial Comodi(Possibilitats all)
    {
        orientacioFisica = 0;
        possibilitatsVirtuals = all;

        GetVeins(pe�a);
        for (int i = 0; i < veins.Length; i++)
        {
            veins[i]?.GetVeins(veins[i]?.pe�a);
        }

        altura = -1;

        return this;
    }

    public void Escollir()
    {
        int r = 0;

        if (possibilitatsVirtuals.Count > 1)
        {
            int highestPriority = 0;

            for (int i = 0; i < possibilitatsVirtuals.Count; i++)
            {;
                if (possibilitatsVirtuals.Get(i).GetPes(i) > highestPriority) highestPriority = possibilitatsVirtuals.Get(i).GetPes(i);
            }

            List<Possibilitat> possibilitats = new List<Possibilitat>();
            for (int i = 0; i < possibilitatsVirtuals.Count; i++)
            {
                if(possibilitatsVirtuals.Get(i).GetPes(i) >= highestPriority)
                {
                    possibilitats.Add(possibilitatsVirtuals.Get(i));
                }
            }

            if(possibilitats.Count == 0)
            {
                Debug.Log("No hi ha possibilitats?!?!?!?!?");
            }

            Escollir(possibilitats[UnityEngine.Random.Range(0, possibilitats.Count)]);

            //----------------------------------------------------
            /*
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
            //Debugar.Log($"RANDOM = {r}");
            */
        }
        else
        {
            if(possibilitatsVirtuals.Count == 0)
            {
                Debug.LogError($"{Pe�a.name} ({orientacio}) No s'ha quedat sense possibilitats.. perque?");
            }
            Escollir(possibilitatsVirtuals.Get(0));
        }
        
        //Escollir(possibilitatsVirtuals.Tile(r), possibilitatsVirtuals.Orietacio(r));
    }
    void Escollir(Possibilitat possibilitat) => Escollir(possibilitat.Tile, possibilitat.Orientacio);
    public void Escollir(Tile tile, int orientacioFisica)
    {
        this.orientacioFisica = orientacioFisica;
        this.possibilitatsVirtuals = new Possibilitats(tile, orientacioFisica, 1000);

        Debug.Log($"Escollir => {Pe�a.name}({tile.name} ({orientacio})");

        if (WaveFunctionColpaseScriptable.veureProces)
            Crear();
        
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

    bool Igualar(Connexio a, Connexio b) => a.EncaixaAmb(b) && b.EncaixaAmb(a);

    public void Crear()
    {
        if (WaveFunctionColpaseScriptable.veureProces)
        {
            if (tileFisic != null)
                MonoBehaviour.Destroy(TileFisic);
        }
        

        //XS_InstantiateGPU.Instantiate(PossibilitatsVirtuals.Get(0).Tile.Prefab);

        tileFisic = GameObject.Instantiate(PossibilitatsVirtuals.Get(0).Tile.Prefab, pe�a.Parent.position, Quaternion.Euler(0, orientacio * 60, 0), pe�a.Parent);
        tileFisic.name = $"{orientacio}.-{PossibilitatsVirtuals.Get(0).Tile.name}({orientacio})";

        tileFisic.transform.localEulerAngles = new Vector3(0, orientacio * 60, 0) + new Vector3(0, AngleOrientacioFisica, 0);
        if (orientacioFisica != 0)
        {
            pe�a.Parent.localScale = Vector3.one;
            //tileFisic.transform.position = pe�a.Parent.position - tileFisic.transform.forward * GridExtensions.GetWorldPosition(0, 0).z + (tileFisic.transform.right * 0.5f) * (orientacioFisica == 1 ? 1 : -1);
            tileFisic.transform.position = pe�a.Parent.position - tileFisic.transform.forward * .866f + (tileFisic.transform.right * 0.5f) * (orientacioFisica == 1 ? 1 : -1);
        }

        //tileFisic.AddComponent<TileDebug>().New(veins, orientacioFisica);

        //Detalls(pe�a.Subestat);
    }

    public void Detalls(DetallScriptable[] detalls)
    {
        if (WaveFunctionColpaseScriptable.veureProces)
        {
            if (this.detalls != null)
                MonoBehaviour.Destroy(this.detalls);
        }


        if (detalls == null || detalls.Length == 0)
            return;

        for (int d = 0; d < detalls.Length; d++)
        {
            int[] tiles = detalls[d].Tiles(pe�a);
            for (int t = 0; t < tiles.Length; t++)
            {
                //Debug.LogError($"Pe�a {Pe�a.Coordenades}, Orientacio {orientacio} Tile {t}", Pe�a);
                //Tiles del detalls no retorna tots els tiles, nom�s els que s'han de instanciar. Aqui es mira si aquest est� a la llista.
                if (tiles[t] == orientacio)
                {
                    //Debug.LogError("TRobat!");
                    GameObject _detall = detalls[d].GameObject(pe�a, this);
                    if (_detall == null)
                        continue;

                    //detalls = GameObject.Instantiate(subestat.detalls[d].GameObject(pe�a), tileFisic.transform.position, Quaternion.identity, tileFisic.transform);
                    this.detalls = GameObject.Instantiate(_detall, tileFisic.transform.position, Quaternion.identity);
                    for (int m = 0; m < detalls[d].Modificacios.Length; m++)
                    {
                        detalls[d].Modificacios[m].Modificar(this, this.detalls);
                    }
                    this.detalls.transform.SetParent(tileFisic.transform);
                }
            }
        }
    }
    /*public void Detalls(Subestat subestat)
    {
        if (WaveFunctionColpaseScriptable.veureProces)
        {
            if (detalls != null)
                MonoBehaviour.Destroy(detalls);
        }


        if (subestat.Detalls == null || subestat.Detalls.Length == 0)
            return;

        for (int d = 0; d < subestat.Detalls.Length; d++)
        {
            int[] tiles = subestat.Detalls[d].Tiles(pe�a);
            for (int t = 0; t < tiles.Length; t++)
            {
                //Debug.LogError($"Pe�a {Pe�a.Coordenades}, Orientacio {orientacio} Tile {t}", Pe�a);
                //Tiles del detalls no retorna tots els tiles, nom�s els que s'han de instanciar. Aqui es mira si aquest est� a la llista.
                if(tiles[t] == orientacio)
                {
                    //Debug.LogError("TRobat!");
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

    }*/


    public struct Random
    {
        public int index;
        public Vector2Int rang;
    }
}

