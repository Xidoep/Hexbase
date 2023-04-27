using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Visualitzacions;

public class Peça : Hexagon, IPointerEnterHandler, IPointerExitHandler
{
    [System.Flags] public enum EstatConnexioEnum { 
        NoImporta = 0, 
        Connectat = 1, 
        Desconnectat = 2, 
        Pendent = 4
    }


    public override void Setup(Grid grid, Vector2Int coordenades, Estat estat, Subestat subestat)
    {
        base.Setup(grid, coordenades, estat, null);

        this.estat = estat;

        estatConnexio = EstatConnexioEnum.Desconnectat;

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
    [SerializeField] EstatConnexioEnum estatConnexio;
    //[SerializeField] bool pendent;
    [SerializeField] Peça connexio;
    [SerializeField] Vector2Int connexioCoordenada;
    
    
    
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
    //public Condicio[] Condicions => this.subestat.Condicions;
    public bool TeCasa => cases != null && cases.Length > 0;
    //vvv
    public Casa Casa => cases[0];
    public Casa[] Cases => cases;
    public int CasesLength => cases.Length;
    //vvv
    public ProducteExtret[] ProductesExtrets => productesExtrets;
    //vvv

    public Peça Connexio => connexio;
    public Vector2Int ConnexioCoordenada => connexioCoordenada;
    public bool Connectat => estatConnexio == EstatConnexioEnum.Connectat;
    public bool Desconnectat => estatConnexio == EstatConnexioEnum.Desconnectat;
    public TilePotencial GetTile(int index) => tiles[index];
    public bool EstatIgualA(Estat altreEstat) => estat.Equals(altreEstat);
    public bool SubestatIgualA(Subestat altreSubestat) => subestat.Equals(altreSubestat);
    




    //SETTERS
    public EstatConnexioEnum EstatConnexio { get => estatConnexio; set => estatConnexio = value; }
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

    public void CanviarSubestat(Subestat subestat)
    {
        //if (this.subestat.Condicions == null)
        //    return;

        //amagarInformacio?.Invoke(this);
        
        //mostrarInformacio -= subestat.InformacioMostrar;
        //amagarInformacio -= subestat.InformacioAmagar;

        this.subestat = subestat.Setup(this);
        cases = new Casa[0];
        //gameObject.name = $"{subestat.name.ToUpper()}({Coordenades})";

        //mostrarInformacio += subestat.InformacioMostrar;
        //amagarInformacio += subestat.InformacioAmagar;

        
        //subestat.InformacioMostrar(this, true);
    }

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
        connexio = productor;
        productor.connexio = this;
    }
    public void DesocuparPerPrediccio()
    {
        if(Connexio.connexio != this)
            connexio = null;
    }


    public void IntentarConnectar()
    {
        if (connexio != null) //Ja està connectat
            return;

        List<Peça> _v = VeinsPeça;
        for (int i = 0; i < _v.Count; i++)
        {
            if (_v[i].estatConnexio == EstatConnexioEnum.Pendent)
            {
                _v[i].EstatConnexio = EstatConnexioEnum.Connectat;
                _v[i].connexio = this;
                estatConnexio = EstatConnexioEnum.Connectat;
                connexio = _v[i];
            }
        }

        if(connexio == null) //No he trobat cap altre peça pendent de connectar. Quedo pendent que algú es connecti amb mi.
        {
            estatConnexio = EstatConnexioEnum.Pendent;
        }
    }
    public void Desconnectar()
    {
        estatConnexio = EstatConnexioEnum.Desconnectat;
        connexio = null;
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











}



