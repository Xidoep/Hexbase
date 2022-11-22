using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[System.Serializable]
public class TilePotencial
{
    public TilePotencial(Peça peça, int orientacio)
    {
        this.peça = peça;
        this.orientacio = orientacio;

        GetPossibilitats();
    }

    Peça peça;

    [SerializeField] int orientacio;
    [SerializeField] Possibilitats possibilitatsVirtuals;
    int orientacioFisica = 0;

    [SerializeField] GameObject tileFisic;
    GameObject detalls;
    [HideInInspector] TilePotencial[] veins;
    
    

    public string EstatName => $"{peça.Subestat.name}-{orientacio}";
    public Peça Peça => peça;
    public Possibilitats PossibilitatsVirtuals => possibilitatsVirtuals;
    public GameObject TileFisic => tileFisic;
    public TilePotencial[] Veins => veins;


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
        if (peça.Estat == null || peça.Subestat == null)
            return;
        orientacioFisica = 0;

        possibilitatsVirtuals = peça.Subestat.Possibilitats();

    }

    public TilePotencial Ambiguo()
    {
        if (WaveFunctionColpaseScriptable.veureProces)
        {
            if (tileFisic)
            {
                XS_GPU.RemoveGrafic(tileFisic);
                MonoBehaviour.Destroy(tileFisic);
            }
        }

        GetPossibilitats();
        GetVeins(peça);
        for (int i = 0; i < veins.Length; i++)
        {
            veins[i]?.GetVeins(veins[i]?.peça);
        }

        return this;
    }

    public void Escollir()
    {
        int r = 0;

        if (possibilitatsVirtuals.Count > 1)
        {
            int highestPriority = 0;

            //Trobar la prioritat mes alta
            for (int i = 0; i < possibilitatsVirtuals.Count; i++)
            {
                if (possibilitatsVirtuals.Get(i).Pes > highestPriority) highestPriority = possibilitatsVirtuals.Get(i).Pes;
            }

            List<Possibilitat> possibilitats;
            possibilitats = new List<Possibilitat>();
            for (int i = 0; i < possibilitatsVirtuals.Count; i++)
            {
                if(possibilitatsVirtuals.Get(i).Pes == highestPriority)
                {
                    possibilitats.Add(possibilitatsVirtuals.Get(i));
                }
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
            Escollir(possibilitatsVirtuals.Get(0));
        }
        
        //Escollir(possibilitatsVirtuals.Tile(r), possibilitatsVirtuals.Orietacio(r));
    }
    void Escollir(Possibilitat possibilitat) => Escollir(possibilitat.Tile, possibilitat.Orientacio);
    public void Escollir(Tile tile, int orientacioFisica)
    {
        this.orientacioFisica = orientacioFisica;
        this.possibilitatsVirtuals = new Possibilitats(tile, orientacioFisica, 1000);
        
        if (WaveFunctionColpaseScriptable.veureProces)
            Crear();
        
    }

    public void GetVeins(Peça peça)
    {
        List<Hexagon> PecesVeines = peça.Veins;
        Peça hexagonVei = PecesVeines[orientacio] != null ? (PecesVeines[orientacio].EsPeça ? (Peça)PecesVeines[orientacio] : null) : null;

        veins = new TilePotencial[]
        {
            hexagonVei != null ? hexagonVei.GetTile((orientacio + 3) % 6) : null,
            peça.GetTile((orientacio + 1) % 6),
            peça.GetTile((orientacio - 1 >= 0) ? orientacio - 1 : 5)
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

        tileFisic = GameObject.Instantiate(PossibilitatsVirtuals.Get(0).Tile.Prefab, peça.Parent.position, Quaternion.Euler(0, orientacio * 60, 0), peça.Parent);
        tileFisic.name = $"{orientacio}.-{PossibilitatsVirtuals.Get(0).Tile.name}({orientacio})";

        tileFisic.transform.localEulerAngles = new Vector3(0, orientacio * 60, 0) + new Vector3(0, AngleOrientacioFisica, 0);
        if (orientacioFisica != 0)
            tileFisic.transform.position = peça.Parent.position - tileFisic.transform.forward * GridExtensions.GetWorldPosition(0, 0).z + (tileFisic.transform.right * 0.5f) * (orientacioFisica == 1 ? 1 : -1);


        //tileFisic.AddComponent<TileDebug>().New(veins, orientacioFisica);

        //Detalls(peça.Subestat);
    }

    public void Detalls(Subestat subestat)
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
            int[] tiles = subestat.Detalls[d].Tiles(peça);
            for (int t = 0; t < tiles.Length; t++)
            {
                //Tiles del detalls no retorna tots els tiles, només els que s'han de instanciar. Aqui es mira si aquest està a la llista.
                if(tiles[t] == orientacio)
                {
                    GameObject _detall = subestat.Detalls[d].GameObject(peça, this);
                    if (_detall == null)
                        continue;

                    //detalls = GameObject.Instantiate(subestat.detalls[d].GameObject(peça), tileFisic.transform.position, Quaternion.identity, tileFisic.transform);
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

