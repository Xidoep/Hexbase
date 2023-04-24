using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Visualitzacions;

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
        gameObject.name = $"{estat.name}({coordenades})";
    }

    [Apartat("ESTAT")]
    [SerializeScriptableObject][SerializeField] Estat estat;
    [SerializeScriptableObject][SerializeField] protected Subestat subestat;

    [Apartat("CASA")]
    [SerializeField] Casa[] cases;

    [Apartat("PROCESSADOR")]
    public Processador processador;

    [Apartat("EXTRACCIO")]
    [SerializeField] Peça extraccio;
    [SerializeField] Vector2Int extraccioCoordenada;
    [SerializeField] Peça productor;
    [Apartat("PRDUCTES")]
    [SerializeField] ProducteExtret[] productesExtrets;

    [Apartat("INFORMACIO")]
    [SerializeField] bool blocarInformacio;




    //INTERN
    TilePotencial[] tiles;


    //GETTERS
    public override bool EsPeça => true;


    public Estat Estat => estat;
    public Subestat Subestat => subestat;
    //vvv
    public TilePotencial[] Tiles => tiles;
    public bool EstatNull => estat == null;
    public bool SubestatNull => subestat == null;
    public string SubestatNom => subestat.name;
    public string EstatNom => estat.name;


    public DetallScriptable[] Detalls => subestat.Detalls;
    public Possibilitats Possibilitats => subestat.Possibilitats(this);
    public Connexio[] ConnexionsPossibles => subestat.ConnexionsPossibles(this);
    public bool TeConnexionsNules => subestat.TeConnexionsNules(this);
    public Connexio[] ConnexionsNules => subestat.ConnexionsNules(this);
    public TileSetBase.ConnexioEspesifica ConnexionsEspesifica => subestat.ConnexionsEspesifica(this);
    public bool Caminable => subestat.Caminable;
    public bool Aquatic => subestat.Aquatic;
    public Condicio[] Condicions => this.subestat.Condicions;
    public bool TeCasa => cases != null && cases.Length > 0;
    //vvv
    public Casa Casa => cases[0];
    public Casa[] Cases => cases;
    public int CasesLength => cases.Length;
    //vvv
    public ProducteExtret[] ExtreureProducte => productesExtrets;
    //vvv
    public Peça GetExtraccio => extraccio;
    public Vector2Int GetExtraccioCoordenada => extraccioCoordenada;
    public bool Ocupat => productor != null;
    public bool LLiure => productor == null;
    public TilePotencial GetTile(int index) => tiles[index];
    public bool EstatIgualA(Estat altreEstat) => estat.Equals(altreEstat);
    public bool SubestatIgualA(Subestat altreSubestat) => subestat.Equals(altreSubestat);
    




    //SETTERS
    public ProducteExtret[] SetProductesExtrets { set => productesExtrets = value; }
    public Vector2Int SetExtraccio { set => extraccioCoordenada = value; }
    public bool SetBlocarInformacio { set => blocarInformacio = value; }
    public void ResetCases() => cases = new Casa[0];







    public void CrearTilesPotencials()
    {
        if (tiles == null || tiles.Length == 0)
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





    public void CrearTilesFisics()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i].TileFisic != null)
            {
                Destroy(tiles[i].TileFisic);
            }

            tiles[i].Crear();
        }
    }
    public void CrearDetalls()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].Detalls(subestat.Detalls);
        }
    }


    public void CanviarSubestat(object subestat) => CanviarSubestat((Subestat)subestat);
    public void CanviarSubestat(Subestat subestat)
    {
        if (this.subestat.Condicions == null)
            return;

        amagarInformacio?.Invoke(this);
        
        mostrarInformacio -= subestat.InformacioMostrar;
        amagarInformacio -= subestat.InformacioAmagar;

        this.subestat = subestat.Setup(this);
        //cases = new Casa[0];
        //gameObject.name = $"{subestat.name.ToUpper()}({Coordenades})";

        mostrarInformacio += subestat.InformacioMostrar;
        amagarInformacio += subestat.InformacioAmagar;

        
        subestat.InformacioMostrar(this, true);
    }

    //public void CrearCasa(Producte producte) => cases = new Casa[] { new Casa(new Producte[] { producte }, () => mostrarInformacio?.Invoke(this, false)), };
    public void CrearCasa(Casa casa) => cases = new Casa[] { casa };

    public void AfegirCasa(Recepta[] necessitats)
    {
        if (cases == null) cases = new Casa[0];
        List<Casa> _cases = new List<Casa>(cases)
        {
            new Casa(this, necessitats)
        };

        cases = _cases.ToArray();
    } 
    public void AfegirCasa(Producte producte) //Treure
    {
        if (cases == null) cases = new Casa[0];
        List<Casa> _cases = new List<Casa>(cases)
        {
            new Casa(new Producte[] { producte }, () => mostrarInformacio?.Invoke(this, false))
        };

        cases = _cases.ToArray();
    }
    public void TreureCasa()
    {
        if (cases == null || cases.Length == 0)
            return;

        List<Casa> _cases = new List<Casa>(cases);
        _cases.RemoveAt(_cases.Count - 1);

        cases = _cases.ToArray();
    }


    public void Ocupar(Peça productor) 
    {
        this.productor = productor;
        productor.extraccio = this;
    }
    public void DesocuparPerPrediccio()
    {
        if(productor.extraccio != this)
        {
            productor = null;
        }
        
    }










    public override void OnPointerEnter()
    {
        if (blocarInformacio)
            return;

        mostrarInformacio?.Invoke(this, true);
    }
    public override void OnPointerExit()
    {
        if (blocarInformacio)
            return;

        mostrarInformacio?.Invoke(this, false);
    }















    //INTERACCIO
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => OnPointerEnter();
    public void OnPointerExit(PointerEventData eventData) => OnPointerExit();














    [System.Serializable]
    public struct ProducteExtret
    {
        public Producte producte;
        public bool gastat;
        public Informacio.Unitat informacio;
    }
}



