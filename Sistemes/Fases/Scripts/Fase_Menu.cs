using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
    [SerializeField] UI_Menu uiMenu;
    [SerializeField] Utils_InstantiableFromProject sortir1;
    [SerializeField] Utils_InstantiableFromProject sortir2;
    [SerializeField] Utils_InstantiableFromProject fadeOut;
    [SerializeField] Utils_InstantiableFromProject continuarFondoClicable;
    //[SerializeField] UI_Menu ui;

    [Apartat("BOTONS")]
    [SerializeField] Botons[] opcions;
    [SerializeField] GameObject novaPartida;
    [SerializeField] GameObject continuar;
    [SerializeField] GameObject configuracio;
    [SerializeField] GameObject sortir;
    [Apartat("MODES")]
    [SerializeField] Modes modes;

    bool inici = true;
    List<Hexagon> botons;
    List<Coroutine> coroutines;

    //OVERRIDES
    public override void FaseStart()
    {
        OnFinish -= uiMenu.DesregistrarAccions; //perque no es multipliqui
        uiMenu.DesregistrarAccions();

        OnFinish += MarcarComIniciat;
        OnFinish += NetejarBotonsDelGrid;
        OnFinish += uiMenu.RegistrarAccions;

        Grid.Instance.CrearGrid();

        if (inici)
        {
            if (save.TePeces)
            {
                iniciar.GridBrut();
                modes.Set((Mode)save.Mode);
                save.Load(grups, null);
                continuarFondoClicable.Instantiate();
            }
            else
                Opcions(opcions);
        }
        else
            Opcions(opcions);


    }

    //PUBLIQUES
    public void Sortir()
    {
        Debugar.Log("SORTIR?");
        if (!save.TePeces)
            return;

        //ui.Resume();
        Debugar.Log("SORTIR??");

        guardat.Set(SEGONA_PARTIDA, true, true);
        
        if (!save.TeCaptures)
        {
            capturarPantalla.CapturarSenseVisuals();
            guardat.Guardar();
            XS_Coroutine.StartCoroutine_Ending(2, fadeOut.Instantiate);
            XS_Coroutine.StartCoroutine_Ending(3, Application.Quit);
        }
        else
        {
            fadeOut.Instantiate();
            XS_Coroutine.StartCoroutine_Ending(1, Application.Quit);
        }

    }

    public void NovaPartida()
    {
        save.NouArxiu(modes.Mode);
        iniciar.Iniciar();
    }
    public void Continuar()
    {
        NetejarBotonsDelGrid();
        iniciar.GridBrut();
        save.Load(grups, iniciar);
    }


    //PRIVADES
    void Opcions()
    {
        if (botons == null) botons = new List<Hexagon>();
        else botons.Clear();

        if (coroutines == null) coroutines = new List<Coroutine>();
        else coroutines.Clear();

        Grid.Instance.Resetejar();



        botons.Add(Grid.Instance.CrearBoto(Grid.Instance.Centre, novaPartida));
        if (save.TePeces)
        {
            coroutines.Add(XS_Coroutine.StartCoroutine(CrearBotoDelayed(continuar, new Vector2Int(-1, -1), 0.5f)));
        }
        iniciar.Reset();
    }
    void Opcions(Botons[] opcions)
    {
        if (botons == null) botons = new List<Hexagon>();
        else botons.Clear();

        if (coroutines == null) coroutines = new List<Coroutine>();
        else coroutines.Clear();

        Grid.Instance.Resetejar();
        int index = 0;
        for (index = 0; index < opcions.Length; index++)
        {
            if(index == 0)
                botons.Add(opcions[index].Crear(Grid.Instance));
            else 
                XS_Coroutine.StartCoroutine_Ending(index * 0.5f, Crear);
            //Sembla que la variable es la "local" per tant, encara que s'envii, quan la Corrutina es planteja actuar, la variable ja ha canviat.
            //Concretament ha canviat a 4, que es el maxim i en el punt en que es trenca el loop.
            //Aixo es pel que no es capa� de intanciar boto, perque no existeix en boto[4].
            
        }

        void Crear() => botons.Add(opcions[index].Crear(Grid.Instance));
    }



    IEnumerator CrearBotoDelayed(GameObject prefab, Vector2Int posicio, float delay)
    {
        yield return new WaitForSeconds(delay);
        botons.Add(Grid.Instance.CrearBoto(Grid.Instance.Centre + posicio, prefab));
    }

    void NetejarBotonsDelGrid()
    {
        if(coroutines != null)
        {
            for (int i = 0; i < coroutines.Count; i++)
            {
                XS_Coroutine.StopCoroutine(coroutines[i]);
            }
        }
        if(botons != null)
        {
            for (int i = 0; i < botons.Count; i++)
            {
                Grid.Instance.Netejar(botons[i].Coordenades);
                //Animacio de amagar...
                Destroy(botons[i].gameObject, 1);
            }
        }
        
        OnFinish -= NetejarBotonsDelGrid;
    }
    void MarcarComIniciat()
    {
        inici = true;
        OnFinish -= MarcarComIniciat;
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
                guardat.Set(Fase_Menu.SEGONA_PARTIDA, true, true);
                sortir2.Instantiate();
            }
        }
    }





    [System.Serializable]
    public struct Botons
    {
        [SerializeField] GameObject boto;
        [SerializeField] Vector2Int coordenadaOffset;

        public Hexagon Crear(Grid grid) => Grid.Instance.CrearBoto(Grid.Instance.Centre + coordenadaOffset, boto);
    }




    new void OnDisable()
    {
        base.OnDisable();
        botons = null;
        inici = true;
    }

    private void OnValidate()
    {
        if (guardat == null) guardat = XS_Editor.LoadGuardat<Guardat>();
        if (uiMenu == null) uiMenu = XS_Editor.LoadAssetAtPath<UI_Menu>("Assets/XidoStudio/UI/UI.asset");
        if (capturarPantalla == null) capturarPantalla = XS_Editor.LoadAssetAtPath<CapturarPantalla>("Assets/XidoStudio/Capturar/CapturarPantalla.asset");
    }
}
