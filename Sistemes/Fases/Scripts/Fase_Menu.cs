using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using XS_Utils;
using Sirenix.OdinInspector;
     
/// <summary>
/// Fase inicial del joc. On esculles el mode de joc, la partida i les opcions.
/// </summary>
[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Menu")]
public class Fase_Menu : Fase
{
    //public const string SEGONA_PARTIDA = "SegonaPartida";
    [BoxGroup("GUARDAT"), SerializeScriptableObject, SerializeField] SaveHex save;
    [BoxGroup("GUARDAT"), SerializeScriptableObject, SerializeField] Guardat guardat;
    [BoxGroup("GUARDAT"), SerializeScriptableObject, SerializeField] Grups grups;
    [BoxGroup("GUARDAT"), SerializeScriptableObject, SerializeField] CapturarPantalla capturarPantalla;
    [BoxGroup("GUARDAT"), SerializeField] SavableVariable<bool> bromaVista;
    [BoxGroup("GUARDAT"), SerializeField] SavableVariable<bool> segonaPartida;

    [BoxGroup("SEGÜENT FASE"), SerializeScriptableObject, SerializeField] Fase_Iniciar iniciar;
    [BoxGroup("SEGÜENT FASE"), SerializeScriptableObject, SerializeField] Fase colocar;

    [BoxGroup("UI"), SerializeField] UI_Menu uiMenu;
    [BoxGroup("UI"), SerializeField] Utils_InstantiableFromProject titol;
    [BoxGroup("UI"), SerializeField] Utils_InstantiableFromProject sortir1;
    [BoxGroup("UI"), SerializeField] Utils_InstantiableFromProject sortir2;
    [BoxGroup("UI"), SerializeField] Utils_InstantiableFromProject fadeOut;
    [BoxGroup("UI"), SerializeField] Utils_InstantiableFromProject continuarFondoClicable;
    //[SerializeField] UI_Menu ui;

    [FoldoutGroup("BOTONS"), SerializeField] Botons novaPartida;
    [FoldoutGroup("BOTONS"), SerializeField] Botons continuar;
    [FoldoutGroup("BOTONS"), SerializeField] Botons configuracio;
    [FoldoutGroup("BOTONS"), SerializeField] Botons sortir;
    [FoldoutGroup("BOTONS"), SerializeField] Botons mostarModes;
    [FoldoutGroup("BOTONS"), SerializeField] Botons freeStyle;
    [FoldoutGroup("BOTONS"), SerializeField] Botons standard;

    [BoxGroup("MODES"), SerializeScriptableObject, SerializeField] Modes modes;

    [BoxGroup("MUSICA"), SerializeField] So so;
    [BoxGroup("MUSICA"), SerializeField] Music music;
    [BoxGroup("MUSICA"), SerializeField] MusicControlador musicControlador;
    [Space(20)]
    [BoxGroup("DEBUG"), SerializeField] bool començarEnFreeStyle;
    //INTERN
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

        musicControlador.Play(music, 0.1f);
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

        if(començarEnFreeStyle)
            botons.Add(freeStyle);
        else
            botons.Add(novaPartida);

        if (save.HiHaPartidaAnterior) botons.Add(continuar);
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
        Boto tmp = (Boto)boto.Crear(temps);
        tmp.gameObject.SetActive(false);
        botons.Add(tmp);

        XS_Coroutine.StartCoroutine_Ending_FrameDependant(temps, Mostrar);

        void Mostrar() 
        {
            tmp.gameObject.SetActive(true);
            so.PlayDelayed(so.volum.x, 0.5f + temps * 0.5f, 0.75f);
        } 
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
        //[SerializeField] So so;

        public Hexagon Crear(float temps) 
        {
            //so.PlayDelayed(so.volum.x, 1 + 0.75f + temps, temps);
            return Grid.Instance.CrearBoto(Grid.Instance.Centre + coordenadaOffset, boto);
        } 
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
