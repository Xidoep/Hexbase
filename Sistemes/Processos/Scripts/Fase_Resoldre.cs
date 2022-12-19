using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [Apartat("POSSIBLES FASES")]
    [SerializeField] Fase_Menu menu;
    [SerializeField] Fase continuar;
    [SerializeField] Fase perdre;

    [Space(20)]
    [SerializeField] ClasseNivell nivell;
    [SerializeField] ClassePeces peces;

    public ClasseNivell Nivell => nivell;
    public ClassePeces Peces => peces;



    [ContextMenu("mes 2")] void Prova2() => nivell.GuanyarExperiencia(2);
    [ContextMenu("mes 20")] void Prova20() => nivell.GuanyarExperiencia(20);
    [ContextMenu("mes 200")] void Prova200() => nivell.GuanyarExperiencia(200);


    //OVERRIDES
    public override void Inicialitzar()
    {
        switch (menu.Mode)
        {
            case Mode.FreeSyle:
                break;
            case Mode.pila:
                if (!nivell.PujarDeNivell())
                {
                    if (!peces.QuedenPeces())
                    {
                        perdre.Iniciar();
                    }
                }
                break;
        }
        
        continuar.Iniciar();
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
            enGuanyarExperiencia = null;
            enPujarNivell = null;
        }
    }


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
