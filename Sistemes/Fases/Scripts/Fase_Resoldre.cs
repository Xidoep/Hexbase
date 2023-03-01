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
    [SerializeField] Fase menu;
    [SerializeField] Fase iniciar;
    [SerializeField] Fase colocar;

    [Apartat("NEEDS")]
    [SerializeField] Modes modes;
    [SerializeField] CapturarPantalla capturarPantalla;
    [SerializeField] SaveHex save;

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


    public void TornarAMenuPrincipal() 
    {
        //save.NouArxiu(Mode.Pila);
        enTornar?.Invoke();
        capturarPantalla.OnCapturatRegistrar(CanviarMenuDesoresDeCapturar);
        GuardarOBorrar(true);
        //XS_Coroutine.StartCoroutine(CanviarMenu(1, menu));
        //menu.Iniciar();
        //prefab_uiPreguntarGuardar.InstantiateReturn().GetComponent<Utils_EsdevenimentDelegatBool>().Registrar(Tornar);
    }
    /*public void NovaPartida()
    {
        GuardarSiCal(true);
        ((Fase_Iniciar)iniciar).NovaPartida();
    }*/

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
    void Tornar(bool guardar)
    {
        enTornar?.Invoke();
        GuardarOBorrar(guardar);
        XS_Coroutine.StartCoroutine(CanviarMenu(guardar ? 1 : 0.1f, menu));
    }
    void Repetir(bool guardar)
    {
        enRepetir?.Invoke();
        GuardarOBorrar(guardar);
        XS_Coroutine.StartCoroutine(CanviarMenu(guardar ? 1 : 0.1f, iniciar));
    }
    void Continuar_Freestyle()
    {
        enContinuar?.Invoke();
        modes.Set(Mode.FreeStyle);
        iniciar.Iniciar();
    }
    /*public void TornarMenuPrincipal()
    {
        save.NouArxiu(Mode.Pila);
        menu.Iniciar();
    }*/





    IEnumerator CanviarMenu(float temps, Fase fase)
    {
        yield return new WaitForSeconds(temps);
        Grid.Instance.Resetejar();
        Nivell.Reset();
        fase.Iniciar();
    }

    public void GuardarOBorrar(bool guardar)
    {
        if (guardar)
        {
            capturarPantalla.OnCapturatRegistrar(NouArxiuDespresDeCapturar);
            capturarPantalla.CapturarSenseVisuals();
            save.NouArxiu(Mode.Pila);
        }
        else
        {
            save.BorrarPartida();
        }
    }
    public void CapturarSiNoEnTe()
    {
        if (!save.TeCaptures) capturarPantalla.CapturarSenseVisuals();
    }

    void NouArxiuDespresDeCapturar(string path)
    {
        save.NouArxiu(Mode.Pila);
        capturarPantalla.OnCapturatDesregistrar(NouArxiuDespresDeCapturar);
    }
    void CanviarMenuDesoresDeCapturar(string path) 
    {
        Grid.Instance.Resetejar();
        Nivell.Reset();
        menu.Iniciar();
        capturarPantalla.OnCapturatDesregistrar(CanviarMenuDesoresDeCapturar);
    }











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
