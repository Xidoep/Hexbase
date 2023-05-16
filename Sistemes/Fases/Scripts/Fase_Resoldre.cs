using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

/// <summary>
/// Comprova l'estat general de la partida i fa les accions apropiades.
/// </summary>
[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Resoldre")]
public class Fase_Resoldre : Fase
{
    [Apartat("FASES")]
    [SerializeScriptableObject] [SerializeField] Fase menu;
    [SerializeScriptableObject] [SerializeField] Fase iniciar;
    [SerializeScriptableObject] [SerializeField] Fase colocar;

    [Apartat("NEEDS")]
    [SerializeScriptableObject] [SerializeField] Nivell nivell;
    [SerializeScriptableObject] [SerializeField] Modes modes;
    [SerializeScriptableObject] [SerializeField] CapturarPantalla capturarPantalla;
    //[SerializeScriptableObject] [SerializeField] SaveHex save;

    [Apartat("UIs")]
    [SerializeField] Utils_InstantiableFromProject prefab_uiPerdre;
    [SerializeField] Utils_InstantiableFromProject prefab_uiPreguntarGuardar;

    [Apartat("OPCIONS")]
    [SerializeField] PoolPeces poolPeces;


    System.Action enTornar;
    System.Action enRepetir;
    System.Action enContinuar;
    System.Action<Mode> enNetejar;

    public System.Action EnTornar { get => enTornar; set => enTornar = value; }
    public System.Action EnRepetir { get => enRepetir; set => enRepetir = value; }
    public System.Action EnContinuar { get => enContinuar; set => enContinuar = value; }
    public System.Action<Mode> EnNetejar { get => enNetejar; set => enNetejar = value; }



    //OVERRIDES
    public override void FaseStart()
    {
        switch (modes.Mode)
        {
            case Mode.FreeStyle:
                colocar.Iniciar();
                break;
            case Mode.Pila:
                ResoldrePila();
                break;
        }
    }


    public void TornarAMenuPrincipal() //DESDE MENU
    {
        enTornar?.Invoke();
        capturarPantalla.OnCapturatRegistrar(TornarAMenu_DespresDeCapturar);
        capturarPantalla.Capturar(true, false);
        //Capturar();
    }



    //Clar, aixo de pujar de nivell 
    void ResoldrePila()
    {
        if (nivell.HaPujatDeNivell)
        {
            Debugar.Log("PUJAR DE NIVELL!");
            colocar.Iniciar();
        }
        else
        {
            if (poolPeces.Quantitat <= 0)
            {
                prefab_uiPerdre.InstantiateReturn().GetComponent<Utils_EsdevenimentDelegatInt>().Registrar(Popup_Perdre);
            }
            else
            {
                colocar.Iniciar();
            }
        }

        nivell.HaPujatDeNivell = false;
    }

    void Popup_Perdre(int opcio)
    {
        switch (opcio)
        {
            case 0: //TORNAR
                prefab_uiPreguntarGuardar.InstantiateReturn().GetComponent<Utils_EsdevenimentDelegatBool>().Registrar(Tornar);
                break;
            case 1: //REPETIR
                prefab_uiPreguntarGuardar.InstantiateReturn().GetComponent<Utils_EsdevenimentDelegatBool>().Registrar(Repetir);
                break;
            case 2: //CONTINUAR
                Continuar_Freestyle();
                break;
        }
    }
    void Tornar(bool guardar) //DESDE POPUP PERDRE
    {
        enTornar?.Invoke();
        capturarPantalla.OnCapturatRegistrar(TornarAMenu_DespresDeCapturar);
        capturarPantalla.Capturar(false);
        //Capturar();
    }
    void Repetir(bool guardar) //DESDE POPUP PERDRE
    {
        enRepetir?.Invoke();
        capturarPantalla.OnCapturatRegistrar(TornarAIniciar_DespresDeCapturar);
        capturarPantalla.Capturar(false);
        //Capturar();
    }
    void Continuar_Freestyle() //DESDE POPUP PERDRE
    {
        enContinuar?.Invoke();
        modes.Set(Mode.FreeStyle);
        iniciar.Iniciar();
    }

    


    void TornarAMenu_DespresDeCapturar(string path)
    {
        Netejar();
        enNetejar?.Invoke(Mode.Pila);
        menu.Iniciar();

        capturarPantalla.OnCapturatDesregistrar(TornarAMenu_DespresDeCapturar);
    }
    void TornarAIniciar_DespresDeCapturar(string path)
    {
        Netejar();
        iniciar.Iniciar();

        capturarPantalla.OnCapturatDesregistrar(TornarAIniciar_DespresDeCapturar);
    }

    void Netejar()
    {
        Grid.Instance.Resetejar();
        nivell.Reset();
    }


















    //GENERICS
    new void OnDisable()
    {
        base.OnDisable();

        nivell.Reset();
    }


}
