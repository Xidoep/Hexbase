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
    //MIRAR-HO BE AIXO
    //Aquesta fase comprovarà el nivell que has pujat, el peces que et queden i tota la resta.
    //Gestionarà el passa al seguent torn:
    //-donar necessitats
    //-pujar de nivell si cal
    //-donar peces
    //-donar la opcio per desbloquejar
    //-anunciar la fi del joc si s'acaben les peces
    //-donar la opcio de continguar jugant en mode infinit
    //-etc...
    //S'han de guardar i carregar el nivell i l'experiencia guardades per la partida.
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
    Grid grid;

    public System.Action EnTornar { get => enTornar; set => enTornar = value; }
    public System.Action EnRepetir { get => enRepetir; set => enRepetir = value; }
    public System.Action EnContinuar { get => enContinuar; set => enContinuar = value; }


    public ClasseNivell Nivell => nivell;
    public ClassePeces Peces => peces;






    //OVERRIDES
    public override void FaseStart()
    {
        if (grid == null) grid = FindObjectOfType<Grid>();

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

    void ResoldrePila()
    {
        //Si pujes de nivell, no passarà res, continues jugant
        //Si no pujes de nivell, es miren les peces que et queden, si te'n quen prous, continues jugant també.
        //Si no te'n queden és on comença lo interessant d'aquesta fase:
        //Pausa el joc. (està bloquejat el proces de posar peces, aixi que no en pots posar)
        ///T'ensenya una UI, amb els punts aconseguits, les combinacions fetes, les cases creades, els pobles creats i les necessitats cobertes.
        ///Et dona 3 opcions:
        //-Tornar:
        //Abans et pregunta si vols guardar aquest mapa o no.
        //De totes maneres es carrega tot el mon i torna a la Fase_Menu amb el botó inicial (Si guardes el mon, en fà una foto abans de carregarsel)
        //Repetir:
        //Abans et pregunta si vols guardar aquest mapa o no.
        //De totes maneres es carrega tot el mon, i en aquest cas, va a la Fase_Inciar, on coloca la peça inicial (Si guardes el mon, en fa una foto abans de carregarsel).
        //Continuar:
        //En aquest cas, l'unic que es fa es treure la ui de pila i el nivell, canviar el mode i posar la UI freeStyle.

        if (!nivell.PujarDeNivell())
        {

            if (!peces.QuedenPeces())
            {
                prefab_uiPerdre.InstantiateReturn().GetComponent<Utils_EsdevenimentDelegatInt>().Registrar(OpcionsFinalPartida);
            }
            else
            {
                colocar.Iniciar();
            }
        }
        else
        {
            //Agregar needs
            //Desbloquejar peces
        }




        void OpcionsFinalPartida(int opcio)
        {
            switch (opcio)
            {
                case 0: //TORNAR
                    prefab_uiPreguntarGuardar.InstantiateReturn().GetComponent<Utils_EsdevenimentDelegatBool>().Registrar(PopupTornar);
                    break;
                case 1: //REPETIR
                    prefab_uiPreguntarGuardar.InstantiateReturn().GetComponent<Utils_EsdevenimentDelegatBool>().Registrar(PopupRepetir);
                    break;
                case 2: //CONTINUAR
                    enContinuar?.Invoke();
                    modes.Set(Mode.FreeStyle);
                    iniciar.Iniciar();
                    break;
            }
        }

        void PopupTornar(bool guardar)
        {
            enTornar?.Invoke();
            GuardarSiCal(guardar);
            XS_Coroutine.StartCoroutine(CanviarMenu(guardar ? 3 : 0.1f, menu));
            
        }
        void PopupRepetir(bool guardar)
        {
            enRepetir?.Invoke();
            GuardarSiCal(guardar);
            XS_Coroutine.StartCoroutine(CanviarMenu(guardar ? 3 : 0.1f, iniciar));
        }

        IEnumerator CanviarMenu(float temps, Fase fase)
        {
            yield return new WaitForSeconds(temps);
            grid.Resetejar();
            Nivell.Reset();
            fase.Iniciar();
        }

        void GuardarSiCal(bool guardar)
        {
            if (guardar)
            {
                capturarPantalla.Capturar();
                save.NovaPartida();
            }
            else
            {
                save.BorrarPartida();
            }
        }
    }












    [System.Serializable] public class ClasseNivell
    {
        [SerializeField] int nivell = 1;
        [SerializeField] int experiencia = 0;
        System.Action<int, int> enGuanyarExperiencia;
        System.Action<int, int> enPujarNivell;



        //FUNCIONS PUBLIQUES
        public int ProximNivell(int nivell) => nivell * nivell * 10;
        public void GuanyarExperiencia(int experiencia)
        {
            this.experiencia += experiencia;
            enGuanyarExperiencia?.Invoke(nivell, this.experiencia);
        }
        public float FactorExperienciaNivellActual => (experiencia - (ProximNivell(nivell - 1))) / (float)((ProximNivell(nivell) - ProximNivell(nivell - 1)));
        public System.Action<int, int> EnGuanyarExperiencia { get => enGuanyarExperiencia; set => enGuanyarExperiencia = value; }
        public System.Action<int, int> EnPujarNivell { get => enPujarNivell; set => enPujarNivell = value; }



        //GENERICS
        public bool PujarDeNivell()
        {
            if (this.experiencia >= ProximNivell(nivell))
            {
                nivell++;
                enPujarNivell?.Invoke(nivell, experiencia);
                return true;
            }
            return false;
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















    //GENERICS
    new void OnDisable()
    {
        base.OnDisable();

        nivell.Reset();
    }


}
