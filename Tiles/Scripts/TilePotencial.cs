using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[System.Serializable]
public class TilePotencial
{
    public TilePotencial(Estat estat, Peça peça, int orientacio)
    {
        this.peça = peça;
        this.estat = estat;
        estatName = estat.name;
        this.orientacio = orientacio;
        coordenades = peça.Coordenades;
        connexions = estat.ConnexionsPossibles;
        orientacioFisica = 0;
        interaccions = 0;
        assegurat = false;
        haFallat = false;

        ConnexionsNules = estat.VeiNull;
        PossibilitatsInicials = estat.Possibilitats;
        possibilitats = PossibilitatsInicials.Invoke();
    }


    Peça peça;
    Estat estat;
    public string estatName;
    int orientacio;
    public Vector2Int coordenades;
    Tile[] possibilitats;
    Connexio[] connexions;
    int orientacioFisica = 0;
    [SerializeField]int interaccions = 0;
    [SerializeField]bool assegurat;
    [SerializeField]bool haFallat;


    [SerializeField] GameObject tileFisic;
    [SerializeField] GameObject detalls;
    TilePotencial[] veins;
    
    
    
    public Func<TilePotencial, Connexio[]> ConnexionsNules;
    public Func<Tile[]> PossibilitatsInicials;
    public string ID => $"{(estat ? estat.name : "???")}-{orientacio}({(Resolt ? possibilitats[0] : "???")})|{orientacioFisica} {(assegurat ? "(ASSEGURAT)" : "")}";
    public bool Assegurat
    {
        get => assegurat;
        set
        {
            assegurat = value;
            if (tileFisic != null)
            {
                tileFisic.name = ID;
                tileFisic.GetComponent<MeshRenderer>().material.color = assegurat ? Color.grey : Color.gray * 0.5f;
            }
        }
    }
    public Peça Peça => peça;
    public Estat Estat => estat;
    public Vector2Int Coordenades => coordenades;
    public Tile[] Possibilitats => possibilitats;
    public void Interactuar() => interaccions++;
    public int Interaccions => interaccions;
    public GameObject TileFisic => tileFisic;
    public TilePotencial[] Veins => veins;
    public Connexio[] Connexions => connexions;


    public int VeinsResolts
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
    }
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
    public bool Resolt => possibilitats.Length == 1;
    public int OrientacioFisica => orientacioFisica;
    public int Orientacio => orientacio;
    public bool HaFallat { get => haFallat; set => haFallat = value; }




    //INTERN

    void ResetPossibilitats()
    {
        this.possibilitats = PossibilitatsInicials.Invoke();
        orientacioFisica = 0;
    }
    public void SetPossiblitats(WaveFunctionColapse.Possibilitats possibilitats) => this.possibilitats = possibilitats.ToArray();

    public void Ambiguo(bool inicial = false)
    {
        //??? potser podria anar aixo aqui...
        //if (Resolt)
        //    return;

        if (inicial) 
        {
            interaccions = 0;
            haFallat = false;
        } 
        #region DEBUG
        if(tileFisic) MonoBehaviour.Destroy(tileFisic);
        #endregion

        ResetPossibilitats();
        assegurat = false;
        //interaccions = 0;

        Debug.Log($"{ID} Ambiguo");

        GetVeins(peça);
        for (int i = 0; i < veins.Length; i++)
        {
            veins[i]?.GetVeins(veins[i]?.Peça);
        }
        WaveFunctionColapse.Add(this);
        //Potser falta buscar els veins dels veins
    }

    public void Escollir(WaveFunctionColapse.Possibilitats possibilitats, int index) => Escollir(possibilitats.Tile(index), possibilitats.Orietacio(index));
    public void Escollir(Tile tile, int orientacioFisica)
    {
        this.orientacioFisica = orientacioFisica;
        possibilitats = new Tile[] { tile };
        //interaccions = 0;
        haFallat = false;
        #region DEBUG
        //Crear();
        //Debug.Log($"ESCOLLIR: {tileFisic.name}");
        #endregion
    }

    public void GetVeins(Peça peça)
    {
        Hexagon[] PecesVeines = peça.Veins;
        Peça hexagonVei = PecesVeines[orientacio] != null ? (PecesVeines[orientacio].EsPeça ? (Peça)PecesVeines[orientacio] : null) : null;

        veins = new TilePotencial[]
        {
            hexagonVei != null ? hexagonVei.GetTile((orientacio + 3) % 6) : null,
            peça.GetTile((orientacio + 1) % 6),
            peça.GetTile((orientacio - 1 >= 0) ? orientacio - 1 : 5)
        };
    }
    

    public Connexio[] GetConnexions(int p) => new Connexio[] { possibilitats[p].Exterior(orientacioFisica), possibilitats[p].Esquerra(orientacioFisica), possibilitats[p].Dreta(orientacioFisica) };


    public int CompararConnexions(int p, Connexio exterior, Connexio esquerra, Connexio dreta)
    {
        /*if (exterior == possibilitats[p].Exterior(orientacioFisica) & esquerra == possibilitats[p].Esquerra(orientacioFisica) & dreta == possibilitats[p].Dreta(orientacioFisica))
            return 0;
        else
        {
            if (exterior == possibilitats[p].Esquerra(orientacioFisica) & esquerra == possibilitats[p].Dreta(orientacioFisica) & dreta == possibilitats[p].Exterior(orientacioFisica))
                return 1;
            else
            {
                if (exterior == possibilitats[p].Dreta(orientacioFisica) & esquerra == possibilitats[p].Exterior(orientacioFisica) & dreta == possibilitats[p].Esquerra(orientacioFisica))
                    return 2;
                else
                    return -1;
            }
        }*/

        if (Igualar(exterior, possibilitats[p].Exterior(orientacioFisica)) & Igualar(esquerra, possibilitats[p].Esquerra(orientacioFisica)) & Igualar(dreta, possibilitats[p].Dreta(orientacioFisica)))
            return 0;
        else
        {
            if (Igualar(exterior, possibilitats[p].Esquerra(orientacioFisica)) & Igualar(esquerra, possibilitats[p].Dreta(orientacioFisica)) & Igualar(dreta, possibilitats[p].Exterior(orientacioFisica)))
                return 1;
            else
            {
                if (Igualar(exterior, possibilitats[p].Dreta(orientacioFisica)) & Igualar(esquerra, possibilitats[p].Exterior(orientacioFisica)) & Igualar(dreta, possibilitats[p].Esquerra(orientacioFisica)))
                    return 2;
                else
                    return -1;
            }
        }
    }

    bool Igualar(Connexio a, Connexio b) => a.Viable == b && b.Viable == a;

    public void Crear()
    {
        tileFisic = GameObject.Instantiate(possibilitats[0].Prefab, peça.Parent.position, Quaternion.Euler(0, orientacio * 60, 0), peça.Parent);
        tileFisic.name = ID;

        tileFisic.transform.localEulerAngles = new Vector3(0, orientacio * 60, 0) + new Vector3(0, AngleOrientacioFisica, 0);
        if (orientacioFisica != 0)
            tileFisic.transform.position = peça.Parent.position - tileFisic.transform.forward * GridExtensions.GetWorldPosition(0, 0).z + (tileFisic.transform.right * 0.5f) * (orientacioFisica == 1 ? 1 : -1);

        tileFisic.GetComponent<MeshRenderer>().material.color = assegurat ? Color.grey : Color.gray * 0.5f;
        tileFisic.AddComponent<TileDebug>().New(veins);

        Detalls(peça.Subestat);
    }

    public void Detalls(Subestat subestat)
    {
        if(detalls != null)
        {
            MonoBehaviour.Destroy(detalls);
        }

        if (subestat.detalls == null || subestat.detalls.Length == 0)
            return;

        for (int d = 0; d < subestat.detalls.Length; d++)
        {
            int[] tiles = subestat.detalls[d].Tiles(peça);
            for (int t = 0; t < tiles.Length; t++)
            {
                if(tiles[t] == orientacio)
                {
                    if (subestat.detalls[d].GameObject(peça) == null)
                        continue;

                    //detalls = GameObject.Instantiate(subestat.detalls[d].GameObject(peça), tileFisic.transform.position, Quaternion.identity, tileFisic.transform);
                    detalls = GameObject.Instantiate(subestat.detalls[d].GameObject(peça), tileFisic.transform.position, Quaternion.identity);
                    for (int m = 0; m < subestat.detalls[d].Modificacios.Length; m++)
                    {
                        subestat.detalls[d].Modificacios[m].Modificar(this, detalls);
                    }
                    detalls.transform.SetParent(tileFisic.transform);
                }
            }
        
            
        }

    }
}


public class TileDebug : MonoBehaviour
{
    [SerializeField] TilePotencial[] veins;
    public TileDebug New(TilePotencial[] veins) 
    {
        this.veins = veins;
        return this;
    }
}

