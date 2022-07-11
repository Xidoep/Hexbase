using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XS_Utils;



public class Peça : Hexagon, IPointerEnterHandler, IPointerExitHandler
{
    public override void Setup(Grid grid, Vector2Int coordenades, Estat estat)
    {
        base.Setup(grid, coordenades, estat);

        this.estat = estat;
        subestat = estat.SubestatInicial;
        //subestat = estat.SubestatInicial.Get(this);

        condicions = estat.Condicions(subestat);
        /*List<Condicio> _tmp = new List<Condicio>(estat.Condicions);
        _tmp.AddRange(subestat.Condicions);
        condicions = _tmp.ToArray();*/
        //condicions = new List<Condicio>(estat.Condicions);
        //condicions.AddRange(estat.SubestatInicial.Condicions);
    }

    //VARIABLES PUBLIQUES
    [Header("ESTAT")]
    [SerializeField] Estat estat;
    [SerializeField] protected Subestat subestat;

    [Header("CASA")]
    [SerializeField] List<Casa> cases;
    //VARIABLES PRIVADES
    TilePotencial[] tiles;
    protected Condicio[] condicions;



    //PROPIETATS   
    public override bool EsPeça => true;
    public TilePotencial[] Tiles => tiles;
    public Estat Estat => estat;
    public string EstatName => estat.name;
    public bool EstatIgualA(Estat altreEstat) => estat.Equals(altreEstat);
    public Condicio[] Condicions => condicions;
    public Subestat Subestat => subestat;
    public bool TeCases => cases != null || cases.Count > 0;
    public int Cases => cases != null ? cases.Count : 0;
    public Habitant HabitantLLiure
    {
        get
        {
            Habitant hab = null;
            for (int i = 0; i < cases.Count; i++)
            {
                hab = cases[i].HabitantDisponible();
                if (hab != null) break;
            }
            return hab;
        }
    }



    public void Actualitzar()
    {
        Debug.Log($"Actualitzar ({EstatName}({Coordenades}))");
        name = $"{EstatName}({Coordenades})";

        CrearTilesPotencials();
        AssignarVeinsTiles(tiles);
        #region DEBUG
        for (int i = 0; i < tiles.Length; i++)
        {
            string _debug = "";
            _debug += $"ID = {tiles[i].ID} | Potencials = (";
            for (int p = 0; p < tiles[i].Possibilitats.Length; p++)
            {
                _debug += tiles[i].Possibilitats[p].name + ", ";
            }
            _debug += ") \n";

            _debug += $"|A- {(tiles[i].Veins[0] != null ? $"{tiles[i].Veins[0].ID}\n" : "NULL\n")}";
            _debug += $"|E- {(tiles[i].Veins[1] != null ? $"{tiles[i].Veins[1].ID}\n" : "NULL\n")}";
            _debug += $"|D- {(tiles[i].Veins[2] != null ? $"{tiles[i].Veins[2].ID}\n" : "NULL\n")}";
            _debug += " ";
            Debugar.Log(_debug);
        }
        Debugar.Log("------------------------");
        #endregion
        Estat.TilesInicials(tiles);
        TornarVeinsAmbiguus();
        //estat.OnCreate(this);
    }




    void CrearTilesPotencials()
    {
        tiles = new TilePotencial[6];
        for (int i = 0; i < 6; i++)
        {
            tiles[i] = new TilePotencial(Estat, this, i);
        }
    }
    void AssignarVeinsTiles(TilePotencial[] tilesPotencials)
    {
        for (int i = 0; i < tilesPotencials.Length; i++)
        {
            tilesPotencials[i].GetVeins(this);

            if (tiles[i].Veins[0] == null)
                continue;

            tiles[i].Veins[0].Veins[0] = tiles[i];
        }
    }

    void TornarVeinsAmbiguus()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i].Veins[0] == null)
                continue;

            //if (tiles[i].Veins[0].Peça.acabadaDeCrear)
            //    continue;

            tiles[i].Veins[0].Ambiguo(true);
            tiles[i].Veins[0].Veins[0] = tiles[i];
            tiles[i].Veins[0].Veins[1].Ambiguo(true);
            tiles[i].Veins[0].Veins[2].Ambiguo(true);
        }
    }





    public TilePotencial GetTile(int index) => tiles[index];

    public void CrearTilesFisics()
    {
        //*************************
        //ANALITZAR PATRONS INTERNS
        //*************************

        //if(!acabadaDeCrear)
            //animacioPerCodi.Play();

        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i].TileFisic != null)
                Destroy(tiles[i].TileFisic);

            tiles[i].Crear();
        }
    }

    public void CanviarSubestat(Subestat subestat)
    {
        if (condicions == null)
            return;

        /*for (int i = 0; i < this.subestat.Condicions.Length; i++)
        {
            condicions.Remove(this.subestat.Condicions[i]);
        }*/
        this.subestat = subestat;
        //condicions.AddRange(subestat.Condicions);

        condicions = estat.Condicions(subestat);

        for (int i = 0; i < Tiles.Length; i++)
        {
            Tiles[i].Detalls(subestat);
        }
    }

    public void AddCasa()
    {
        if (cases == null) cases = new List<Casa>();

        cases.Add(new Casa(this, 2));
    }
    public void RemoveCasa()
    {
        cases.RemoveAt(cases.Count - 1);
    }

    /*public void AfegirCasa(int habitants)
    {
        if (this.cases == null) this.cases = new List<Casa>();

        this.cases.Add(new Casa(this, habitants));

    }*/

    //INTERACCIO
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {/*Show info*/ }
    public void OnPointerExit(PointerEventData eventData) {/*Hide info*/ }




}



