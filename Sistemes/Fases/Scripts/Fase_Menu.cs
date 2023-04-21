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
    //public const string SEGONA_PARTIDA = "SegonaPartida";

    [Apartat("GUARDAT")]
    [SerializeField] SaveHex save;
    [SerializeField] Guardat guardat;
    [SerializeField] Grups grups;
    [SerializeField] CapturarPantalla capturarPantalla;
    [SerializeField] SavableVariable<bool> bromaVista;
    [SerializeField] SavableVariable<bool> segonaPartida;

    [Apartat("SEGÜENT FASE")]
    [SerializeField] Fase_Iniciar iniciar;
    [SerializeField] Fase colocar;

    [Apartat("UI")]
    [SerializeField] UI_Menu uiMenu;
    [SerializeField] Utils_InstantiableFromProject titol;
    [SerializeField] Utils_InstantiableFromProject sortir1;
    [SerializeField] Utils_InstantiableFromProject sortir2;
    [SerializeField] Utils_InstantiableFromProject fadeOut;
    [SerializeField] Utils_InstantiableFromProject continuarFondoClicable;
    //[SerializeField] UI_Menu ui;

    [Apartat("BOTONS")]
    [SerializeField] Botons novaPartida;
    [SerializeField] Botons continuar;
    [SerializeField] Botons configuracio;
    [SerializeField] Botons sortir;
    [SerializeField] Botons mostarModes;
    [SerializeField] Botons freeStyle;
    [SerializeField] Botons standard;
    [Apartat("MODES")]
    [SerializeField] Modes modes;

    bool inici = true;
    List<Boto> botons;
    List<Coroutine> coroutines;
    bool _modesMostrats;
    AnimacioPerCodi_GameObject_Referencia animacioTitol;

    //OVERRIDES
    public override void FaseStart()
    {
        OnFinish -= uiMenu.DesregistrarAccions; //perque no es multipliqui
        uiMenu.DesregistrarAccions();

        OnFinish += MarcarComIniciat;
        OnFinish += uiMenu.RegistrarAccions;
        OnFinish += AmagarTitol;

        Grid.Instance.CrearGrid();
        _modesMostrats = false;

        if (inici)
        {
            if (save.TePeces)
            {
                Carregar();
                //continuarFondoClicable.Instantiate();
            }
            else
            {
                MostrarBotonsMenu();
            }
        }
        else
        {
            MostrarBotonsMenu();
        }

        if(botons.Count > 0)
            OnFinish += NetejarBotonsDelGrid;

        animacioTitol = titol.InstantiateReturn().GetComponent<AnimacioPerCodi_GameObject_Referencia>();
    }

    //PUBLIQUES
    public void Sortir() //Sortir del joc
    {
        Debugar.Log("SORTIR?");
        if (!save.TePeces)
            return;

        //ui.Resume();
        Debugar.Log("SORTIR??");

        bromaVista.Valor = true;

        if (!save.TePeces)
        {
            //Borra la partida sense pesses que es cree al anar al menu, per poder veure les opcions.
            save.BorrarPartida();
        }

        fadeOut.Instantiate();
        XS_Coroutine.StartCoroutine_Ending(1, Application.Quit);

    }

    /*public void Carregar(int partida)
    {
        save.Actual = partida;
        //save.SetActual(partida);
        Carregar();
    }*/
    public void Carregar(int partida = -1) //Carrega una partida guardada (Si "partida" és -1 Carregarà la partida "actual")
    {
        Grid.Instance.Resetejar();
        iniciar.GridBrut();
        modes.Set((Mode)save.Mode);

        if (partida == -1)
            save.Load(grups, null, continuarFondoClicable.Instantiate);
        else save.Load(partida, grups, null, continuarFondoClicable.Instantiate);
    }

    public void Modes() //Mostre les opcions dels modes
    {
        if (_modesMostrats)
            return;

        _modesMostrats = true;

        List<Botons> botons = new List<Botons>();
        botons.Add(standard);
        botons.Add(freeStyle);
        CrearBotons(botons.ToArray(), true);
    }
    public void NovaPartida()
    {
        if (save.TePeces)
        {
            save.NouArxiu(modes.Mode);
        }
        save.NouArxiu(modes.Mode);
        iniciar.GridNet();
        iniciar.Iniciar();

        segonaPartida.Valor = true;
    }
    public void Continuar()
    {
        NetejarBotonsDelGrid();
        iniciar.GridBrut();
        save.Continuar(grups, iniciar, modes);
    }

    public void ActivarBotons() => ActivarBotons(true);
    public void DesactivarBotons() => ActivarBotons(false);
    void ActivarBotons(bool activar) //Activa o desactiva els botons per prevenir multiples interaccions
    {
        if (botons.Count == 0)
            return;

        if (activar) botons[0].Seleccionar();
        for (int i = 0; i < botons.Count; i++)
        {
            botons[i].Navegacio(activar);
        }
    }

    //PRIVADES
    void MostrarBotonsMenu()
    {
        List<Botons> botons = new List<Botons>();

        if (!segonaPartida.Valor)
            botons.Add(novaPartida);
        else botons.Add(mostarModes);

        if (save.HiHaPartidaAnterior) botons.Add(continuar);

        /*if (segonaPartida.Valor)
        {
            botons.Add(freeStyle);
        }*/

        botons.Add(configuracio);
        botons.Add(sortir);

        CrearBotons(botons.ToArray());
    }

    void CrearBotons(Botons[] opcions, bool add = false)
    {
        if (!add)
        {
            if (botons == null) botons = new List<Boto>();
            else botons.Clear();

            if (coroutines == null) coroutines = new List<Coroutine>();
            else coroutines.Clear();

            Grid.Instance.Resetejar();
        }
       
        for (int i = 0; i < opcions.Length; i++)
        {
            CrearBoto(opcions[i], 1 + i * 0.5f);
        }

        Grid.Instance.Dimensionar(botons[0]);
    }

    void CrearBoto(Botons boto, float temps)
    {
        Boto tmp = (Boto)boto.Crear(Grid.Instance);
        tmp.gameObject.SetActive(false);
        botons.Add(tmp);

        XS_Coroutine.StartCoroutine_Ending_FrameDependant(temps, Mostrar);

        void Mostrar() => tmp.gameObject.SetActive(true);
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


        botons = new List<Boto>();
        Grid.Instance.Resetejar();
        
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
    void AmagarTitol() => animacioTitol.Destroy();


    public void PopupSortir() => sortir1.InstantiateReturn().GetComponent<Utils_EsdevenimentDelegatBool>().Registrar(BromaSortir);
    void BromaSortir(bool sortir)
    {
        if (sortir)
        {
            if (bromaVista.Valor)
            {
                Sortir();
                fadeOut.Instantiate();
            }
            else
            {
                bromaVista.Valor = true;
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
        bromaVista = new SavableVariable<bool>(SaveHex.KEY_BROMA_SORTIR_VISTA, Guardat.Direccio.Cloud, false);
        segonaPartida = new SavableVariable<bool>(SaveHex.KEY_SEGONA_PARTIDA, Guardat.Direccio.Cloud, false);
    }
}
