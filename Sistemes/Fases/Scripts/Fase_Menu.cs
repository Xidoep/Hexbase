using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;
     
/// <summary>
/// Fase inicial del joc. On esculles el mode de joc, la partida i les opcions.
/// </summary>
[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Menu")]
public class Fase_Menu : Fase
{
    public const string SEGONA_PARTIDA = "SegonaPartida";

    [Apartat("GUARDAT")]
    [SerializeField] SaveHex save;
    [SerializeField] Guardat guardat;
    [SerializeField] Grups grups;
    [SerializeField] CapturarPantalla capturarPantalla;

    [Apartat("SEG�ENT FASE")]
    [SerializeField] Fase_Iniciar iniciar;
    [SerializeField] Fase colocar;

    [Apartat("UI")]
    [SerializeField] Utils_InstantiableFromProject sortir1;
    [SerializeField] Utils_InstantiableFromProject sortir2;
    [SerializeField] Utils_InstantiableFromProject fadeOut;

    [Apartat("BOTONS")]
    [SerializeField] GameObject novaPartida;
    [SerializeField] GameObject continuar;

    Grid grid;
    bool inici = true;
    List<Hexagon> botons;
    List<Coroutine> coroutines;

    //OVERRIDES
    public override void FaseStart()
    {
        OnFinish += MarcarComIniciat;
        OnFinish += NetejarGrid;

        if (grid == null) grid = FindObjectOfType<Grid>();

        grid.CrearGrid();

        if (inici)
        {
            //if (save.TePeces)
            //    save.Load(grups, colocar);
            //else
            Opcions();
        }
        else
            Opcions();


    }

    //PUBLIQUES
    void Sortir()
    {
        if (!save.TePeces)
            return;

        if (!save.TeCaptures)
        {
            capturarPantalla.Capturar();
            XS_Coroutine.StartCoroutine(SortirTemps(3));
        }
        else
        {
            XS_Coroutine.StartCoroutine(SortirTemps(1));
        }

        guardat.SetLocal(SEGONA_PARTIDA, true);

        IEnumerator SortirTemps(float temps)
        {
            yield return new WaitForSeconds(temps);
            Debugar.Log("SORTIR");
            Application.Quit();
        }
    }



    void MarcarComIniciat()
    {
        inici = true;
        OnFinish -= MarcarComIniciat;
    }

    //PRIVADES
    void Opcions()
    {
        if (botons == null) botons = new List<Hexagon>();
        else botons.Clear();
        if (coroutines == null) coroutines = new List<Coroutine>();
        else coroutines.Clear();

        grid.Resetejar();

        botons.Add(grid.CrearBoto(grid.Centre, novaPartida));
        if (save.TePeces)
        {
            coroutines.Add(XS_Coroutine.StartCoroutine(CrearBotoDelayed(continuar, new Vector2Int(-1, -1), 0.5f)));
        }
        iniciar.Reset();
    }

    IEnumerator CrearBotoDelayed(GameObject prefab, Vector2Int posicio, float delay)
    {
        yield return new WaitForSeconds(delay);
        botons.Add(grid.CrearBoto(grid.Centre + posicio, prefab));
    }

    void NetejarGrid()
    {
        for (int i = 0; i < coroutines.Count; i++)
        {
            XS_Coroutine.StopCoroutine(coroutines[i]);
        }
        for (int i = 0; i < botons.Count; i++)
        {
            grid.Netejar(botons[i].Coordenades);
            //Animacio de amagar...
            Destroy(botons[i].gameObject, 1);
        }
        OnFinish -= NetejarGrid;
    }

    public void NovaPartida()
    {
        save.NovaPartida();
        iniciar.Iniciar();
    }
    public void Continuar() 
    {
        NetejarGrid();
        iniciar.GridBrut();
        save.Load(grups, iniciar);
    } 

    void ConfigurarMode() 
    {
        inici = false;
    }

    


    public void PopupSortir() => sortir1.InstantiateReturn().GetComponent<Utils_EsdevenimentDelegatBool>().Registrar(BromaSortir);
    void BromaSortir(bool sortir)
    {
        if (sortir)
        {
            bool segonaPartida = (bool)guardat.Get(Fase_Menu.SEGONA_PARTIDA, false);

            if (segonaPartida)
            {
                Sortir();
                fadeOut.Instantiate();
            }
            else
            {
                sortir2.Instantiate();
            }
        }
    }






    new void OnDisable()
    {
        base.OnDisable();
        inici = true;
    }

    private void OnValidate()
    {
        if (guardat == null) guardat = XS_Editor.LoadGuardat<Guardat>();
    }
}
