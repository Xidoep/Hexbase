using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Visualitzacions;

public class Peça : Hexagon, IPointerEnterHandler, IPointerExitHandler
{
    [System.Flags] public enum ConnexioEnum { 
        NoImporta = 0, 
        Connectat = 1, 
        Desconnectat = 2
    }


    public override void Setup(Grid grid, Vector2Int coordenades, Estat estat, Subestat subestat)
    {
        base.Setup(grid, coordenades, estat, null);

        this.estat = estat;

        estatConnexio = ConnexioEnum.Desconnectat;

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
    [SerializeField] List<Casa> cases;

    [Apartat("PROCESSADOR")]
    public Processador processador;

    [Apartat("EXTRACCIO")]
    [SerializeField] ConnexioEnum estatConnexio;
    //[SerializeField] bool pendent;
    [SerializeField] Peça connexio;
    [SerializeField] Vector2Int connexioCoordenada;
    
    
    [Apartat("PRDUCTES")]
    [SerializeField] ProducteExtret[] productesExtrets;


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
    public bool TeCasa => cases != null && cases.Count > 0;
    //vvv
    public Casa[] Cases => cases.ToArray();
    public int CasesLength => cases.Count;
    //vvv
    public ProducteExtret[] ProductesExtrets => productesExtrets;
    //vvv

    public Peça Connexio => connexio;
    public Vector2Int ConnexioCoordenada => connexioCoordenada;
    public bool Connectat => estatConnexio == ConnexioEnum.Connectat;
    public bool Desconnectat => estatConnexio == ConnexioEnum.Desconnectat;
    public TilePotencial GetTile(int index) => tiles[index];
    public bool EstatIgualA(Estat altreEstat) => estat.Equals(altreEstat);
    public bool SubestatIgualA(Subestat altreSubestat) => subestat.Equals(altreSubestat);
    




    //SETTERS
    public ConnexioEnum EstatConnexio { get => estatConnexio; set => estatConnexio = value; }
    public ProducteExtret[] SetProductesExtrets { set => productesExtrets = value; }
    //public Vector2Int SetExtraccio { set => extraccioCoordenada = value; }
    public void ResetCases() => cases = new List<Casa>();






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
        cases = new List<Casa>();
        //gameObject.name = $"{subestat.name.ToUpper()}({Coordenades})";

        //mostrarInformacio += subestat.InformacioMostrar;
        //amagarInformacio += subestat.InformacioAmagar;

        
        //subestat.InformacioMostrar(this, true);
    }

    public void CrearCasa(Casa casa) => cases = new List<Casa>() { casa };
    public void AfegirCasa(Recepta[] necessitats)
    {
        if (cases == null) cases = new List<Casa>();

        cases.Add(new Casa(this, necessitats));
    }
    public void TreureCasa()
    {
        if (cases == null || cases.Count == 0)
            return;

        processador.BorrarRecepta(cases[cases.Count - 1].ReceptaActual);

        cases.RemoveAt(cases.Count - 1);
    }


    public void Connectar(Peça connexio) 
    {
        this.connexio = connexio;
        estatConnexio = ConnexioEnum.Connectat;
        connexio.connexio = this;
        connexio.EstatConnexio = ConnexioEnum.Connectat;

        processador.IntentarProcessar(this, new List<object>() { connexio }, true);
    }
    public void DesocuparPerPrediccio()
    {
        if(Connexio.connexio != this)
            connexio = null;
    }







    public override void OnPointerEnter()
    {
        CursorEstat.Mostrar(false);
        mostrarInformacio?.Invoke(this, true);
    }
    public override void OnPointerExit()
    {
        CursorEstat.Mostrar(true);
        mostrarInformacio?.Invoke(this, false);
    }















    //INTERACCIO
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => OnPointerEnter();
    public void OnPointerExit(PointerEventData eventData) => OnPointerExit();











}



