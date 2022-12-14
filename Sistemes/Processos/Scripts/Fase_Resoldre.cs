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

    [Apartat("COLOCAR")]
    [SerializeField] int nivell;
    [SerializeField] int experiencia;
    
    System.Action<int, int> enGuanyarExperiencia;
    System.Action<int> enPujarNivell;

    public System.Action<int, int> EnGuanyarExperiencia { get => enGuanyarExperiencia; set => enGuanyarExperiencia = value; }
    public System.Action<int> EnPujarNivell { get => enPujarNivell; set => enPujarNivell = value; }


    public int ProximNivell(int nivell) => nivell * nivell * 10;
    public float FactorExperienciaNivellActual => (experiencia - (ProximNivell(nivell - 1))) / (float)((ProximNivell(nivell) - ProximNivell(nivell - 1)));


    public override void Inicialitzar()
    {
        if (this.experiencia >= ProximNivell(nivell))
        {
            nivell++;
            enPujarNivell.Invoke(nivell);
        }
    }


    public void Experiencia(int experiencia)
    {
        this.experiencia += experiencia;
        enGuanyarExperiencia.Invoke(nivell, this.experiencia);
    }










    [ContextMenu("mes 2")] void Prova2() => Experiencia(2);
    [ContextMenu("mes 20")] void Prova20() => Experiencia(20);
    [ContextMenu("mes 200")] void Prova200() => Experiencia(200);

    new void OnDisable()
    {
        base.OnDisable();
        enGuanyarExperiencia = null;
        enPujarNivell = null;
    }


}
