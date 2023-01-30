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
        //subestat.InformacioMostrar(this, false);
        //informacio = subestat.InformacioMostrar(this);

        gameObject.name = $"{estat.name}({coordenades})";
        condicions = this.subestat.Condicions;
        //ocupat = false;

        meshRenderers = null;
    }

    [Apartat("ESTAT")]
    [SerializeField] Estat estat;
    [SerializeField] protected Subestat subestat;

    [Apartat("CASA")]
    [SerializeField] Casa[] cases;

    [Apartat("EXTRACCIO")]
    [SerializeField] Peça extraccio;
    [SerializeField] Peça productor;
    //[SerializeField] bool ocupat;
    //[SerializeField] public List<Casa.Necessitat> necessitatsCovertes;
    [Apartat("PRDUCTES")]
    [SerializeField] public ProducteExtret[] productesExtrets;

    [Apartat("INFORMACIO")]
    //[SerializeField] Informacio.Unitat[] informacio;
    public bool blocarInformacio;

    [Apartat("MESH RENDERERS")]
    MeshRenderer[] meshRenderers;
    public bool BlocarInformacio { set => blocarInformacio = value; }

    [System.Serializable] public struct ProducteExtret
    {
        public Producte producte;
        public bool gastat;
        public Informacio.Unitat informacio;
    }

    //VARIABLES PRIVADES
    [SerializeField] TilePotencial[] tiles;
    protected Condicio[] condicions;

    //PROPIETATS
    public override bool EsPeça => true;
    public TilePotencial[] Tiles => tiles;
    public Estat Estat => estat;
    public Subestat Subestat => subestat;
    public bool EstatIgualA(Estat altreEstat) => estat.Equals(altreEstat);
    public bool SubestatIgualA(Subestat altreSubestat) => subestat.Equals(altreSubestat);
    public Condicio[] Condicions => condicions;
    public Casa Casa => cases[0];
    public bool TeCasa => cases != null && cases.Length > 0;
    public MeshRenderer[] MeshRenderers 
    {
        get
        {
            if(meshRenderers == null) meshRenderers = GetComponentsInChildren<MeshRenderer>();
            if(meshRenderers.Length == 0) meshRenderers = GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                if(meshRenderers[i] == null)
                {
                    meshRenderers = GetComponentsInChildren<MeshRenderer>();
                    break;
                }
            }
            return meshRenderers;
        }
    }
    //public Informacio.Unitat[] Informacio { get => informacio; set => informacio = value; }

    //public Producte[] ExtreureProducte() => extraccio.Subestat.Produccio();
    public ProducteExtret[] ExtreureProducte() => productesExtrets;


    public Peça Extraccio => extraccio;


    public bool Ocupat => productor != null;
    public bool LLiure => productor == null;
    public void CoordenadesToProducte(Grid grid) 
    {
        /*if (producteCooerdenada == null)
            return;

        extraccio = (Peça)grid.Get(producteCooerdenada);*/
    } 
    public Vector2Int SetCoordenadesProducte { set => extraccio = null; /*set => producteCooerdenada = value;*/ } //Canviat només perque no molesti

    public System.Action<Peça, bool> mostrarInformacio;
    public System.Action<Peça> amagarInformacio;


    public void CrearTilesPotencials()
    {
        if(tiles == null || tiles.Length == 0)
        {
            tiles = new TilePotencial[6];
            for (int i = 0; i < 6; i++)
            {
                tiles[i] = new TilePotencial(this, i);
            }
        }
        else
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i].Ambiguo();
            }
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

        amagarInformacio?.Invoke(this);
        
        mostrarInformacio -= subestat.InformacioMostrar;
        amagarInformacio -= subestat.InformacioAmagar;

        this.subestat = subestat.Setup(this);
        condicions = subestat.Condicions;

        //ocupat = false;

        TreureCasa();

        gameObject.name = $"{subestat.name.ToUpper()}({Coordenades})";

        meshRenderers = null;

        mostrarInformacio += subestat.InformacioMostrar;
        amagarInformacio += subestat.InformacioAmagar;

        
        subestat.InformacioMostrar(this, true);
    }

    public void CrearCasa(Producte producte)
    {
        cases = new Casa[] { new Casa(this, new Producte[] { producte }, () => mostrarInformacio?.Invoke(this, false)), }; 
    }
    void TreureCasa() => cases = new Casa[0];

    public void Ocupar(Peça productor) 
    {
        Debug.LogError($"{productor.gameObject.name} es el productor de {gameObject.name}");
        //ocupat = true;
        this.productor = productor;
        productor.extraccio = this;
    }
    public void DesocuparPerPrediccio()
    {
        Debug.LogError($"{gameObject.name} ja no te productor");
        if(productor.extraccio != this)
        {
            productor = null;
        }
        
    }


    //INTERACCIO
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) 
    {
        if (blocarInformacio)
            return;

        mostrarInformacio?.Invoke(this, true);
    }
    public void OnPointerExit(PointerEventData eventData) 
    {
        if (blocarInformacio)
            return;

        //amagarInformacio?.Invoke(this);
        mostrarInformacio?.Invoke(this, false);
        //informacio = amagarInformacio?.Invoke(informacio);
    } 




}



