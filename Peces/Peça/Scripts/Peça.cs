using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

public class Peça : Hexagon, IPointerEnterHandler, IPointerExitHandler
{
    /*[System.Flags]*/ public enum ConnexioEnum { 
        [LabelText(" ", SdfIconType.DashLg, IconColor = "gray"), Tooltip("No importa")] NoImporta = 0, 
        [LabelText(" ", SdfIconType.Link45deg, IconColor = "RGB(0,1,0)"), Tooltip("Connectat")] Connectat = 1,
        [LabelText(" ", SdfIconType.XCircle, IconColor = "red"), Tooltip("Desconnectat")] Desconnectat = 2,
        [LabelText(" ", SdfIconType.People, IconColor = "blue"), Tooltip("Connectat amb mi")] ConnectatAmbMi = 4
    }


    public override void Setup(Grid grid, Vector2Int coordenades, EstatColocable estat, Estat subestat)
    {
        base.Setup(grid, coordenades, estat, null);
        this.estat = estat;

        connexio = null;

        this.subestat = subestat.Setup(this);
        gameObject.name = $"{estat.name}{coordenades}";
    }

    [Apartat("ESTAT")]
    [SerializeScriptableObject][SerializeField] EstatColocable estat;
    [SerializeScriptableObject][SerializeField] protected Estat subestat;

    [Apartat("CASA")]
    [SerializeField] List<Casa> cases;
    [SerializeField] Utils_InstantiableFromProject efecteNouHabitant;
    [SerializeField] Utils_InstantiableFromProject efecteNouProducte;

    [Apartat("PROCESSADOR")]
    public Processador processador;

    [Apartat("EXTRACCIO")]
    //[SerializeField] ConnexioEnum estatConnexio;
    //[SerializeField] bool pendent;
    [SerializeField] Peça connexio;
    //[SerializeField] Vector2Int connexioCoordenada;
    
    
    [Apartat("PRDUCTES")]
    [SerializeField] ProducteExtret[] productesExtrets;

    [SerializeField] public System.Action enCrearDetalls;


    //INTERN


    TilePotencial[] tiles;




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
    public ConnexioEspesifica[] ConnexionsEspesifiques => subestat.ConnexionsEspesifiques(this);
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
    public ProducteExtret[] SetProductesExtrets 
    {
        set 
        {
            productesExtrets = value;
            efecteNouProducte.Instantiate(transform.position, productesExtrets.Length);
        } 
    }
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
        enCrearDetalls = null;
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
        enCrearDetalls?.Invoke();
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].Detalls(subestat.Detalls);
        }
    }

    public void CanviarSubestat(Estat subestat)
    {
        productesExtrets = new ProducteExtret[0];
        this.subestat.InformacioAmagar(this);
        //subestat.InformacioAmagar(this);

        this.subestat = subestat.Setup(this);
        cases = new List<Casa>();
    }

    public void CrearCasa(Casa casa) => cases = new List<Casa>() { casa };
    public void AfegirCasa(Recepta[] necessitats, float delay = 0)
    {
        if (cases == null) cases = new List<Casa>();

        cases.Add(new Casa(this, necessitats));
        if (delay == 0)
             efecteNouHabitant.Instantiate(transform.position);
        else efecteNouHabitant.Instantiate(transform.position, delay);
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









    [ShowInInspector] List<string> DebugPossibilitats0 => DebugGetNomsPossibilitats(0);
    [ShowInInspector] List<string> DebugPossibilitats1 => DebugGetNomsPossibilitats(1);
    [ShowInInspector] List<string> DebugPossibilitats2 => DebugGetNomsPossibilitats(2);
    [ShowInInspector] List<string> DebugPossibilitats3 => DebugGetNomsPossibilitats(3);
    [ShowInInspector] List<string> DebugPossibilitats4 => DebugGetNomsPossibilitats(4);
    [ShowInInspector] List<string> DebugPossibilitats5 => DebugGetNomsPossibilitats(5);

    [ShowInInspector]
    string[] veins => tiles != null && tiles.Length > 0 ? new string[] {
        tiles[0].Veins[0] != null ?  tiles[0].Veins[0].EstatName : "-",
        tiles[1].Veins[0] != null ?  tiles[1].Veins[0].EstatName : "-",
        tiles[2].Veins[0] != null ?  tiles[2].Veins[0].EstatName : "-",
        tiles[3].Veins[0] != null ?  tiles[3].Veins[0].EstatName : "-",
        tiles[4].Veins[0] != null ?  tiles[4].Veins[0].EstatName : "-",
        tiles[5].Veins[0] != null ?  tiles[5].Veins[0].EstatName : "-"
    } : new string[0];



    List<string> DebugGetNomsPossibilitats(int index)
    {
        List<string> tmp = new List<string>();
        if (tiles == null || tiles.Length == 0)
            return tmp;

        for (int p = 0; p < tiles[index].PossibilitatsVirtuals.Count; p++)
        {
            tmp.Add(tiles[index].PossibilitatsVirtuals.Get(p).Tile.name);
        }
        return tmp;
    }
}



