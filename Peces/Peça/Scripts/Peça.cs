using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

public class Peça : Hexagon, IPointerEnterHandler, IPointerExitHandler
{
    /*[System.Flags]*/ public enum ConnexioEnum { 
        [LabelText(" ", SdfIconType.DashLg, IconColor = "gray")] NoImporta = 0, 
        [LabelText(" ", SdfIconType.Link45deg, IconColor = "RGB(0,1,0)")] Connectat = 1,
        [LabelText(" ", SdfIconType.XCircle, IconColor = "red")] Desconnectat = 2,
        [LabelText(" ", SdfIconType.People, IconColor = "blue")] ConnectatAmbMi = 4
    }


    public override void Setup(Grid grid, Vector2Int coordenades, EstatColocable estat, Estat subestat)
    {
        base.Setup(grid, coordenades, estat, null);
        this.estat = estat;

        connexio = null;

        this.subestat = subestat.Setup(this);
        gameObject.name = $"{estat.name}({coordenades})";
    }

    [Apartat("ESTAT")]
    [SerializeScriptableObject][SerializeField] EstatColocable estat;
    [SerializeScriptableObject][SerializeField] protected Estat subestat;

    [Apartat("CASA")]
    [SerializeField] List<Casa> cases;

    [Apartat("PROCESSADOR")]
    public Processador processador;

    [Apartat("EXTRACCIO")]
    //[SerializeField] ConnexioEnum estatConnexio;
    //[SerializeField] bool pendent;
    [SerializeField] Peça connexio;
    //[SerializeField] Vector2Int connexioCoordenada;
    
    
    [Apartat("PRDUCTES")]
    [SerializeField] ProducteExtret[] productesExtrets;


    //INTERN
    TilePotencial[] tiles;


    [ReadOnly]
    string[] veins => tiles != null ? new string[] {
        tiles[0].Veins[0] != null ?  tiles[0].Veins[0].EstatName : "null",
        tiles[1].Veins[0] != null ?  tiles[0].Veins[0].EstatName : "null",
        tiles[2].Veins[0] != null ?  tiles[0].Veins[0].EstatName : "null",
        tiles[3].Veins[0] != null ?  tiles[0].Veins[0].EstatName : "null",
        tiles[4].Veins[0] != null ?  tiles[0].Veins[0].EstatName : "null",
        tiles[5].Veins[0] != null ?  tiles[0].Veins[0].EstatName : "null"
    } : new string[0];


    //GETTERS
    public override bool EsPeça => true;
    public bool EsEstatNull => estat == null;
    public bool EsSubestatNull => subestat == null;
    public bool EsCaminable => subestat.Caminable;
    public bool EsAquatic => subestat.Aquatic;
    public bool TeConnexionsNules => subestat.TeConnexionsNules(this);
    public bool TeCasa => cases != null && cases.Count > 0;
    public bool EstaConnectat => Connexio != null;


    public EstatColocable Estat => estat;
    public Estat Subestat => subestat;
    //vvv
    public TilePotencial[] Tiles => tiles;
    public string SubestatNom => subestat.name;
    public string EstatNom => estat.name;


    public DetallScriptable[] Detalls => subestat.Detalls;
    public Possibilitats Possibilitats => subestat.Possibilitats(this);
    public Connexio[] ConnexionsPossibles => subestat.ConnexionsPossibles(this);
    public Connexio[] ConnexionsNules => subestat.ConnexionsNules(this);
    public ConnexioEspesifica ConnexionsEspesifica => subestat.ConnexionsEspesifica(this);
    //public Condicio[] Condicions => this.subestat.Condicions;
    //vvv
    public Casa[] Cases => cases.ToArray();
    public int CasesLength => cases.Count;
    //vvv
    public ProducteExtret[] ProductesExtrets => productesExtrets;
    //vvv

    public Peça Connexio => connexio;
    //public Vector2Int ConnexioCoordenada => connexioCoordenada;
    public TilePotencial GetTile(int index) => tiles[index];
    public bool EstatIgualA(EstatColocable altreEstat) => estat.Equals(altreEstat);
    public bool SubestatIgualA(Estat altreSubestat) => subestat.Equals(altreSubestat);
    




    //SETTERS
    public ConnexioEnum GetEstatConnexio => connexio != null ? ConnexioEnum.Connectat : ConnexioEnum.Desconnectat;
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
        CrearDetalls();
    }
    public void CrearDetalls()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].Detalls(subestat.Detalls);
        }
    }

    public void CanviarSubestat(Estat subestat)
    {
        productesExtrets = new ProducteExtret[0];
        subestat.InformacioAmagar(this);

        this.subestat = subestat.Setup(this);
        cases = new List<Casa>();
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
        this.connexio = connexio; //la meva
        connexio.connexio = this; //L'altre

        //processador.IntentarProcessar(this, new List<object>() { connexio }, true);
    }
    public void Desconnectar()
    {
        connexio.connexio = null; //L'altre
        connexio = null; //La meva
    }
    public void DesocuparPerPrediccio()
    {
        if(Connexio.connexio != this)
            connexio = null;
    }







    public override void OnPointerEnter()
    {
        CursorEstat.Mostrar(false);
        subestat.InformacioMostrar(this);
    }
    public override void OnPointerExit()
    {
        CursorEstat.Mostrar(true);
        subestat.InformacioAmagar(this);
    }















    //INTERACCIO
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => OnPointerEnter();
    public void OnPointerExit(PointerEventData eventData) => OnPointerExit();











}



