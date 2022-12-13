using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Peça : Hexagon, IPointerEnterHandler, IPointerExitHandler
{
    public override void Setup(Grid grid, Vector2Int coordenades, Estat estat, Subestat subestat)
    {
        base.Setup(grid, coordenades, estat, null);

        this.estat = estat;

        if (subestat == null)
            return;

        this.subestat = subestat.Setup(this);

        mostrarInformacio += subestat.InformacioMostrar;
        amagarInformacio += subestat.InformacioAmagar;
        //informacio = subestat.InformacioMostrar(this);

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
    [SerializeField] Peça producte;
    [SerializeField] public List<Casa.Necessitat> necessitatsCovertes;
    Vector2Int producteCooerdenada; 
    bool ocupat;


    //VARIABLES PRIVADES
    [SerializeField] TilePotencial[] tiles;
    protected Condicio[] condicions;
    [SerializeField] GameObject[] informacio;

    //PROPIETATS
    public override bool EsPeça => true;
    public TilePotencial[] Tiles => tiles;
    public Estat Estat => estat;
    public Subestat Subestat => subestat;
    public bool EstatIgualA(Estat altreEstat) => estat.Equals(altreEstat);
    public bool SubestatIgualA(Subestat altreSubestat) => subestat.Equals(altreSubestat);
    public Condicio[] Condicions => condicions;
    public int CasesCount => cases != null ? cases.Count : 0;
    public List<Casa> Cases => cases;

    public Producte[] ExtreureProducte() => producte.Subestat.Produccio();
    public bool TeProducte => producte != null;
    public Vector2Int ProducteCoordenades => producte.Coordenades;
    public Peça Producte => producte;

    public bool Ocupat => ocupat;
    public bool LLiure => !ocupat;
    public void CoordenadesToProducte(Grid grid) 
    {
        if (producteCooerdenada == null)
            return;

        producte = (Peça)grid.Get(producteCooerdenada);
    } 
    public Vector2Int SetCoordenadesProducte { set => producteCooerdenada = value; }

    public System.Func<Peça, GameObject[]> mostrarInformacio;
    public System.Func<GameObject[], GameObject[]> amagarInformacio;


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

        mostrarInformacio -= subestat.InformacioMostrar;
        amagarInformacio -= subestat.InformacioAmagar;

        this.subestat = subestat.Setup(this);
        condicions = subestat.Condicions;

        ocupat = false;

        RemoveAllCases();

        gameObject.name = $"{subestat.name.ToUpper()}({Coordenades})";

        mostrarInformacio += subestat.InformacioMostrar;
        amagarInformacio += subestat.InformacioAmagar;
        //informacio = subestat.InformacioMostrar(this);
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
    void RemoveAllCases() => cases.Clear();

    public void Ocupar(Peça productor) 
    {
        ocupat = true;
        productor.producte = this;
    }


    //INTERACCIO
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) 
    {
        if (informacio == null)
            return;

        if (informacio.Length > 0)
            return;

        informacio = mostrarInformacio?.Invoke(this);
    } 
    public void OnPointerExit(PointerEventData eventData) => informacio = amagarInformacio?.Invoke(informacio);




}



