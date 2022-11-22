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

        this.subestat = subestat.Setup(this);
        gameObject.name = $"{estat.name}({coordenades})";
        condicions = this.subestat.Condicions;
        ocupat = false;
    }

    [Apartat("ESTAT")]
    [SerializeField] Estat estat;
    [SerializeField] protected Subestat subestat;

    [Apartat("CASA")]
    [SerializeField] List<Casa> cases;

    [Apartat("PRODUCTOR/PRODUCTE")]
    [SerializeField] Peça[] producte;
    Vector2Int[] producteCooerdenada; 
    bool ocupat;


    //VARIABLES PRIVADES
    [SerializeField] TilePotencial[] tiles;
    protected Condicio[] condicions;


    //PROPIETATS
    //public int Grup { set => grup = value; get => grup; }
    //public string Grup { set => grupId = value; get => grupId; }
    public override bool EsPeça => true;
    public TilePotencial[] Tiles => tiles;
    public Estat Estat => estat;
    public Subestat Subestat => subestat;
    public bool EstatIgualA(Estat altreEstat) => estat.Equals(altreEstat);
    public bool SubestatIgualA(Subestat altreSubestat) => subestat.Equals(altreSubestat);
    public Condicio[] Condicions => condicions;
    //public bool TeCases => cases != null || cases.Count > 0;
    public int CasesCount => cases != null ? cases.Count : 0;
    public List<Casa> Cases => cases;

    public Producte[] ExtreureProducte() => producte[0].Subestat.Produccio();
    public bool TeProducte => producte.Length > 0;
    public Vector2Int ProducteCoordenades => producte[0].Coordenades;
    public bool Ocupat => ocupat;
    public bool LLiure => !ocupat;
    public void CoordenadesToProducte(Grid grid) 
    {
        if (producteCooerdenada == null)
            return;

        if (producteCooerdenada.Length == 0)
            return;

        producte = new Peça[] { (Peça)grid.Get(producteCooerdenada[0]) };
    } 
    public Vector2Int SetCoordenadesProducte { set => producteCooerdenada = new Vector2Int[] { value }; }



    public void CrearTilesPotencials()
    {
        tiles = new TilePotencial[6];
        for (int i = 0; i < 6; i++)
        {
            tiles[i] = new TilePotencial(this, i);
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




    public TilePotencial GetTile(int index) => tiles[index];

    public void CrearTilesFisics()
    {
        //***************************************************************
        //Abans d'arribar a aquest punt. s'han hagut d'analitzar els tiles i buscar patrons on quadrin les peces multiples.
        //***************************************************************

        //if(!acabadaDeCrear)
        //animacioPerCodi.Play();

        //XS_InstantiateGPU.Render();
        
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i].TileFisic != null)
            {
                //XS_InstantiateGPU.RemoveGrafic(tiles[i].TileFisic);
                Destroy(tiles[i].TileFisic);
            }

            tiles[i].Crear();
            //if(detalls)
            //    tiles[i].Detalls(subestat);
        }
    }
    public void Detalls()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].Detalls(subestat);
        }
    }

    public void CanviarSubestat(Subestat subestat)
    {
        if (condicions == null)
            return;

        this.subestat = subestat.Setup(this);
        condicions = subestat.Condicions;

        ocupat = false;

        for (int i = 0; i < cases.Count; i++)
        {
            RemoveCasa();
        }

        gameObject.name = $"{subestat.name.ToUpper()}({Coordenades})";
    }



    public void AddCasa()
    {
        if (cases == null) cases = new List<Casa>();
        cases.Add(new Casa(this, ((Estat_Casa)Estat).Necessitats));
    }
    public void AddCasa(Casa casa)
    {
        if (cases == null) cases = new List<Casa>();
        cases.Add(casa);
    }
    public void RemoveCasa() => cases.RemoveAt(cases.Count - 1);

    public void Ocupar(Peça productor) 
    {
        ocupat = true;
        productor.producte = new Peça[] { this };
    } 


    //INTERACCIO
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {/*Show info*/ }
    public void OnPointerExit(PointerEventData eventData) {/*Hide info*/ }




}



