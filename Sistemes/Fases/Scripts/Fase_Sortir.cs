using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Sortir")]
public class Fase_Sortir : Fase
{
    [SerializeField]
    public override void Actualitzar()
    {
        //Guardar quan es premi el boto.
        //pero pots apretar el boto a mitg procesar.
        //S'hauria d'enviar un miss al proces actual en realitat, i que comenci aquest quan aquest acabi.
        //potser puc innhabilitar el boto mentre estigui processant.
    }
    public override void Finalitzar()
    {

    }

    public void Exit()
    {
        Application.Quit();
    }

}
