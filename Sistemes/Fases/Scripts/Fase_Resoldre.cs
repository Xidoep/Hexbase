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
    [SerializeScriptableObject] [SerializeField] Modes modes;
    [SerializeScriptableObject] [SerializeField] CapturarPantalla capturarPantalla;
    [SerializeScriptableObject] [SerializeField] SaveHex save;

    [Apartat("UIs")]
    [SerializeField] Utils_InstantiableFromProject prefab_uiPerdre;
    [SerializeField] Utils_InstantiableFromProject prefab_uiPreguntarGuardar;

    [Apartat("OPCIONS")]
    [SerializeField] ClasseNivell nivell;
    [SerializeField] ClassePeces peces;



    System.Action enTornar;
    System.Action enRepetir;
    System.Action enContinuar;

    public System.Action EnTornar { get => enTornar; set => enTornar = value; }
    public System.Action EnRepetir { get => enRepetir; set => enRepetir = value; }
    public System.Action EnContinuar { get => enContinuar; set => enContinuar = value; }


    public ClasseNivell Nivell => nivell;
    public ClassePeces Peces => peces;






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




    void ResoldrePila()
    {
        if (!nivell.PujarDeNivell)
        {

            if (!peces.QuedenPeces())
            {
                prefab_uiPerdre.InstantiateReturn().GetComponent<Utils_EsdevenimentDelegatInt>().Registrar(Popup_Perdre);
            }
            else
            {
                colocar.Iniciar();
            }
        }
        else
        {
            Debugar.Log("PUJAR DE NIVELL!");
            colocar.Iniciar();
            //Agregar needs
            //Desbloquejar peces
        }
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

    


    void Capturar()
    {
        //capturarPantalla.OnCapturatRegistrar(NouArxiuDespresDeCapturar);
        capturarPantalla.Capturar(false);

        /*void NouArxiuDespresDeCapturar(string path)
        {
            save.NouArxiu(Mode.Pila);
            capturarPantalla.OnCapturatDesregistrar(NouArxiuDespresDeCapturar);
        }*/
    }

    void TornarAMenu_DespresDeCapturar(string path)
    {
        Netejar();
        save.NouArxiu(Mode.Pila);
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
        Nivell.Reset();
    }





    /*void CanviarMenuDespresDeCapturar(string path) 
    {
        Grid.Instance.Resetejar();
        Nivell.Reset();
        menu.Iniciar();

        capturarPantalla.OnCapturatDesregistrar(CanviarMenuDespresDeCapturar);
    }*/










    [System.Serializable] public class ClasseNivell
    {
        [SerializeField] int nivell = 1;
        [SerializeField] int experiencia = 0;
        [Space(20)]
        [SerializeField] Visualitzacions visualitzacions;
        System.Action<int, int> enGuanyarExperiencia;
        System.Action<int, int> enPujarNivell;

        //PROPIETATS
        public bool PujarDeNivell
        {
            get
            {
                if (this.experiencia >= ProximNivell(nivell))
                {
                    nivell++;
                    enPujarNivell?.Invoke(nivell, experiencia);
                    return true;
                }
                return false;
            }
        }
        public float FactorExperienciaNivellActual => (experiencia - (ProximNivell(nivell - 1))) / (float)((ProximNivell(nivell) - ProximNivell(nivell - 1)));
        public System.Action<int, int> EnGuanyarExperiencia { get => enGuanyarExperiencia; set => enGuanyarExperiencia = value; }
        public System.Action<int, int> EnPujarNivell { get => enPujarNivell; set => enPujarNivell = value; }


        //FUNCIONS PUBLIQUES
        public int ProximNivell(int nivell) => nivell * nivell * 10;
        public void GuanyarExperiencia(int experiencia)
        {
            this.experiencia += experiencia;
            enGuanyarExperiencia?.Invoke(nivell, this.experiencia);
            visualitzacions.GuanyarExperienciaProduccio();
        }



        public void Reset()
        {
            nivell = 1;
            experiencia = 0;
            enGuanyarExperiencia?.Invoke(nivell, this.experiencia);

            enGuanyarExperiencia = null;
            enPujarNivell = null;
        }
    }
    [ContextMenu("mes 2")] void Prova2() => nivell.GuanyarExperiencia(2);
    [ContextMenu("mes 20")] void Prova20() => nivell.GuanyarExperiencia(20);
    [ContextMenu("mes 200")] void Prova200() => nivell.GuanyarExperiencia(200);


    [System.Serializable] public class ClassePeces
    {
        [SerializeField] PoolPeces pool;

        public bool QuedenPeces()
        {
            return pool.Quantitat > 0;
        }

    }








    private void OnEnable()
    {
        nivell.EnGuanyarExperiencia += save.ActualitzarExperiencia;
    }


    //GENERICS
    new void OnDisable()
    {
        base.OnDisable();

        nivell.Reset();
        nivell.EnGuanyarExperiencia -= save.ActualitzarExperiencia;
    }


}
