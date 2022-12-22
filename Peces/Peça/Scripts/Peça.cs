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
        informacio = subestat.InformacioMostrar(informacio, this, false);
        //informacio = subestat.InformacioMostrar(this);

        gameObject.name = $"{estat.name}({coordenades})";
        condicions = this.subestat.Condicions;
        ocupat = false;
    }

    [Apartat("ESTAT")]
    [SerializeField] Estat estat;
    [SerializeField] protected Subestat subestat;

    [Apartat("CASA")]
    [SerializeField] Casa[] cases;

    [Apartat("EXTRACCIO")]
    [SerializeField] Peça extraccio;
    //[SerializeField] public List<Casa.Necessitat> necessitatsCovertes;
    bool ocupat;
    [Apartat("PRDUCTES")]
    [SerializeField] public ProducteExtret[] productesExtrets;

    [System.Serializable] public struct ProducteExtret
    {
        public Producte producte;
        public bool gastat;
    }

    //VARIABLES PRIVADES
    [SerializeField] TilePotencial[] tiles;
    protected Condicio[] condicions;
    [SerializeField] Informacio.Unitat[] informacio;

    //PROPIETATS
    public override bool EsPeça => true;
    public TilePotencial[] Tiles => tiles;
    public Estat Estat => estat;
    public Subestat Subestat => subestat;
    public bool EstatIgualA(Estat altreEstat) => estat.Equals(altreEstat);
    public bool SubestatIgualA(Subestat altreSubestat) => subestat.Equals(altreSubestat);
    public Condicio[] Condicions => condicions;
    public Casa Casa => cases[0];
    public bool TeCasa => cases.Length > 0;

    public Informacio.Unitat[] Informacio => informacio;

    //public Producte[] ExtreureProducte() => extraccio.Subestat.Produccio();
    public ProducteExtret[] ExtreureProducte() => productesExtrets;


    public Peça Extraccio => extraccio;


    public bool Ocupat => ocupat;
    public bool LLiure => !ocupat;
    public void CoordenadesToProducte(Grid grid) 
    {
        /*if (producteCooerdenada == null)
            return;

        extraccio = (Peça)grid.Get(producteCooerdenada);*/
    } 
    public Vector2Int SetCoordenadesProducte { set => extraccio = null; /*set => producteCooerdenada = value;*/ } //Canviat només perque no molesti

    public System.Func<Informacio.Unitat[], Peça, bool, Informacio.Unitat[]> mostrarInformacio;
    public System.Func<Informacio.Unitat[], Informacio.Unitat[]> amagarInformacio;


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

        informacio = amagarInformacio?.Invoke(informacio);
        
        mostrarInformacio -= subestat.InformacioMostrar;
        amagarInformacio -= subestat.InformacioAmagar;

        this.subestat = subestat.Setup(this);
        condicions = subestat.Condicions;

        ocupat = false;

        TreureCasa();

        gameObject.name = $"{subestat.name.ToUpper()}({Coordenades})";

        mostrarInformacio += subestat.InformacioMostrar;
        amagarInformacio += subestat.InformacioAmagar;
        //informacio = subestat.InformacioMostrar(this);
    }

    public void CrearCasa(Producte producte)
    {
        cases = new Casa[] { new Casa(this, new Producte[] { producte }, () => informacio = mostrarInformacio?.Invoke(informacio, this, false)), }; 
    }
    void TreureCasa() => cases = new Casa[0];

    public void Ocupar(Peça productor) 
    {
        ocupat = true;
        productor.extraccio = this;
    }


    public void InformacioDestroy(int index, float temps)
    {
        Destroy(this.informacio[index].gameObject, temps);

        List<Informacio.Unitat> informacio = new List<Informacio.Unitat>(this.informacio);
        informacio.RemoveAt(index);
        this.informacio = informacio.ToArray();

    }

    //INTERACCIO
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) 
    {
        informacio = mostrarInformacio?.Invoke(informacio, this, true);
    }
    public void OnPointerExit(PointerEventData eventData) 
    {
        informacio = mostrarInformacio?.Invoke(informacio, this, false);
        //informacio = amagarInformacio?.Invoke(informacio);
    } 




}



