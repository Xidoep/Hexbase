using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using XS_Utils;

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
    [SerializeField] AnimacioPerCodi_GameObject_Referencia prefab_informacioNouHabitant;
    [SerializeField] AnimacioPerCodi_GameObject_Referencia prefab_informacioProducte;

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
    GameObject prefab;



    //GETTERS
    public override bool EsPeça => true;
    public bool EsEstatNull => estat == null;
    public bool EsSubestatNull => subestat == null;
    //public bool EsCaminable => subestat.Caminable;
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

    bool TeNecessitats
    {
        get
        {
            bool enTe = false;
            for (int c = 0; c < cases.Count; c++)
            {
                if (cases[c].Necessitats.Count > 0)
                {
                    enTe = true;
                    break;
                }
            }
            return enTe;
        }
    }


    AnimacioPerCodi_GameObject_Referencia informacio;

    //SETTERS
    public ConnexioEnum GetEstatConnexio => connexio != null ? ConnexioEnum.Connectat : ConnexioEnum.Desconnectat;
    public ProducteExtret[] SetProductesExtrets 
    {
        set 
        {
            productesExtrets = value;
            XS_Coroutine.StartCoroutine_Ending_FrameDependant(productesExtrets.Length * 0.75f, EfecteNouProducte);
            //efecteNouProducte.Instantiate(transform.position, productesExtrets.Length * 0.5f);
        } 
    }
    void EfecteNouProducte()
    {
        GameObject efecte = efecteNouProducte.InstantiateReturn(transform.position);

        new Animacio_Posicio(
                        transform.position,
                        connexio.transform.position
                        ).Play(efecte.transform, .75f, .5f, Transicio.clamp, false);

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



    public void CrearPreafabTemporal()
    {
        if (WaveFunctionColpaseScriptable.veureProces)
            return;
            
        prefab = Instantiate(estat.Prefab.gameObject, transform.position, transform.rotation, transform);
    }

    public void CrearTilesFisics()
    {
        if (prefab) Destroy(prefab);

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


        if (informacio != null)
            return;

        informacio = Instantiate(prefab_informacioNouHabitant, transform.position, Quaternion.identity);
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









    public void MostrarInformacio()
    {
        if (!TeNecessitats)
            return;

        AmagarInformacio();

        if(cases != null && cases.Count > 0)
        {
            informacio = Instantiate(prefab_informacioNouHabitant, transform.position, Quaternion.identity);
        }
        else
        {
            if(connexio != null)
                informacio = Instantiate(prefab_informacioProducte, transform.position, Quaternion.identity);
        }
    }
    public void AmagarInformacio()
    {
        if (!informacio)
            return;

        informacio.Destroy();
    }
    public void ResaltarInformacio() => informacio.PointerEnter();
    public void DesresaltarInformacio() => informacio.PointerExit();










    public override void OnPointerEnter()
    {
        CursorEstat.Mostrar(false);
        subestat.InformacioMostrar(this);

        AmagarInformacio();
    }
    public override void OnPointerExit()
    {
        CursorEstat.Mostrar(true);
        subestat.InformacioAmagar(this);

        MostrarInformacio();
    }




    [SerializeField, Range(0,5)] int tileIndexDebug;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, .5f);
        if (tiles[tileIndexDebug].TileFisic == null) 
            return;

        if(tiles[tileIndexDebug].TileFisic.TryGetComponent(out MeshRenderer mr))
        {
            Gizmos.DrawCube(mr.bounds.center, Vector3.one * .5f);
        }

        Gizmos.color = new Color(1, 1, 0, .5f);
        if (tiles[tileIndexDebug].Veins[0] != null && tiles[tileIndexDebug].Veins[0].TileFisic != null) Gizmos.DrawCube(tiles[tileIndexDebug].Veins[0].TileFisic.GetComponent<MeshRenderer>().bounds.center, Vector3.one * .5f);
        if (tiles[tileIndexDebug].Veins[1] != null  && tiles[tileIndexDebug].Veins[1].TileFisic != null) Gizmos.DrawCube(tiles[tileIndexDebug].Veins[1].TileFisic.GetComponent<MeshRenderer>().bounds.center, Vector3.one * .5f);
        if (tiles[tileIndexDebug].Veins[2] != null && tiles[tileIndexDebug].Veins[2].TileFisic != null) Gizmos.DrawCube(tiles[tileIndexDebug].Veins[2].TileFisic.GetComponent<MeshRenderer>().bounds.center, Vector3.one * .5f);
    }








    //INTERACCIO
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => OnPointerEnter();
    public void OnPointerExit(PointerEventData eventData) => OnPointerExit();








    [TitleGroup("Tile 1"), ShowInInspector] List<string> DebugPossibilitats0 => DebugGetNomsPossibilitats(0);
    [TitleGroup("Tile 1"), ShowInInspector] int DebugOrientacio0 => DebugOrientacio(0);
    [TitleGroup("Tile 1"), ShowInInspector] string DebugVei0 => DebugVei(0);

    [TitleGroup("Tile 2"), ShowInInspector] List<string> DebugPossibilitats1 => DebugGetNomsPossibilitats(1);
    [TitleGroup("Tile 2"), ShowInInspector] int DebugOrientacio1 => DebugOrientacio(1);
    [TitleGroup("Tile 2"), ShowInInspector] string DebugVei1 => DebugVei(1);

    [TitleGroup("Tile 3"), ShowInInspector] List<string> DebugPossibilitats2 => DebugGetNomsPossibilitats(2);
    [TitleGroup("Tile 3"), ShowInInspector] int DebugOrientacio2 => DebugOrientacio(2);
    [TitleGroup("Tile 3"), ShowInInspector] string DebugVei20 => DebugVei(2);

    [TitleGroup("Tile 4"), ShowInInspector] List<string> DebugPossibilitats3 => DebugGetNomsPossibilitats(3);
    [TitleGroup("Tile 4"), ShowInInspector] int DebugOrientacio3 => DebugOrientacio(3);
    [TitleGroup("Tile 4"), ShowInInspector] string DebugVei3 => DebugVei(3);

    [TitleGroup("Tile 5"), ShowInInspector] List<string> DebugPossibilitats4 => DebugGetNomsPossibilitats(4);
    [TitleGroup("Tile 5"), ShowInInspector] int DebugOrientacio4 => DebugOrientacio(4);
    [TitleGroup("Tile 5"), ShowInInspector] string DebugVei4 => DebugVei(4);

    [TitleGroup("Tile 6"), ShowInInspector] List<string> DebugPossibilitats5 => DebugGetNomsPossibilitats(5);
    [TitleGroup("Tile 6"), ShowInInspector] int DebugOrientacio5 => DebugOrientacio(5);
    [TitleGroup("Tile 6"), ShowInInspector] string DebugVei5 => DebugVei(5);

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
    string DebugVei(int index)
    {
        if (tiles[index].Veins[0] == null)
            return "null";
        else
        {
            if (tiles[index].Veins[0].Resolt) return Tiles[index].Veins[0].PossibilitatsVirtuals.Get(0).Tile.name;
            else
                return "mes d'una possibilitat?...";
        }
    }
    int DebugOrientacio(int index) => tiles[index].OrientacioFisica;
}



