using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XS_Utils;



public class Peça : Hexagon, IPointerEnterHandler, IPointerExitHandler
{
    public override void Setup(Grid grid, Vector2Int coordenades, Estat estat, Subestat subestat)
    {
        base.Setup(grid, coordenades, estat, null);

        this.estat = estat;

        if (subestat == null)
            return;

        this.subestat = subestat;
        if (!this.estat.EsCasa) this.subestat.Productor(this);

        condicions = this.estat.Condicions(this.subestat);
    }
    /*public override void Setup(Grid grid, Vector2Int coordenades, Estat estat, Subestat subestat)
    {
        base.Setup(grid, coordenades, estat);

        this.estat = estat;

        this.subestat = this.estat.SubestatInicial;
        if (!this.estat.EsCasa) subestat.Productor(this);

        condicions = this.estat.Condicions(subestat);
    }*/

    //VARIABLES PUBLIQUES
    [Apartat("ESTAT")]
    [SerializeField] Estat estat;
    [SerializeField] protected Subestat subestat;

    [Apartat("GRUP")]
    [SerializeField] int grup;

    [Apartat("CASA")]
    [SerializeField] List<Casa> cases;

    [Apartat("HABITANT")]
    [SerializeField] Casa[] treballador;

    //VARIABLES PRIVADES
    [SerializeField] TilePotencial[] tiles;
    protected Condicio[] condicions;



    //PROPIETATS
    public int Grup { set => grup = value; get => grup; }

    public override bool EsPeça => true;
    public TilePotencial[] Tiles => tiles;
    public Estat Estat => estat;
    public Subestat Subestat => subestat;
    public string EstatName => estat.name;
    public bool EstatIgualA(Estat altreEstat) => estat.Equals(altreEstat);
    public bool SubestatIgualA(Subestat altreSubestat) => subestat.Equals(altreSubestat);
    public Condicio[] Condicions => condicions;
    //public bool TeCases => cases != null || cases.Count > 0;
    public int CasesCount => cases != null ? cases.Count : 0;
    public List<Casa> Cases => cases;
    public Casa TreballadorLLiure
    {
        get
        {
            Casa disponible = null;
            for (int i = 0; i < cases.Count; i++)
            {
                if (cases[i].Disponible)
                {
                    disponible = cases[i];
                    break;
                }
            }
            return disponible;
        }
    }
    public bool HiHaTreballador => treballador != null && treballador.Length != 0;
    public Casa Treballador => treballador[0];

    public void Actualitzar()
    {
        Debug.Log($"Actualitzar ({estat.name}({Coordenades}))");
        name = $"{estat.name}({Coordenades})";

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




    public void CrearTilesPotencials()
    {
        tiles = new TilePotencial[6];
        for (int i = 0; i < 6; i++)
        {
            tiles[i] = new TilePotencial(Estat, this, i);
        }
    }
    public void AssignarVeinsTiles(TilePotencial[] tilesPotencials)
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
        //***************************************************************
        //Abans d'arribar a aquest punt. s'han hagut d'analitzar els tiles i buscar patrons on quadrin les peces multiples.
        //***************************************************************

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

        Debug.LogError($"DONAR {subestat.Punts} PUNTS!");
        

        /*for (int i = 0; i < this.subestat.Condicions.Length; i++)
        {
            condicions.Remove(this.subestat.Condicions[i]);
        }*/

        this.subestat = subestat;
        if (!estat.EsCasa) subestat.Productor(this);

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

        //cases.Add(new Casa(this, 2));
        cases.Add(new Casa(this, ((Estat_Casa)Estat).Necessitats));
    }
    public void AddCasa(Casa casa)
    {
        if (cases == null) cases = new List<Casa>();
        cases.Add(casa);
    }
    public void RemoveCasa() 
    {
        cases[cases.Count - 1].Desocupar();
        cases.RemoveAt(cases.Count - 1);
    } 
    public void AddTreballador(Casa treballador) => this.treballador = new Casa[] { treballador };
    public void RemoveTreballador() => treballador = null;



    //public void AddHabitant(Habitant habitant) => treballador = habitant;

    /*public void AfegirCasa(int habitants)
    {
        if (this.cases == null) this.cases = new List<Casa>();

        this.cases.Add(new Casa(this, habitants));

    }*/

    //INTERACCIO
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {/*Show info*/ }
    public void OnPointerExit(PointerEventData eventData) {/*Hide info*/ }




}



