using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/General")]
public class Informacio_General : Informacio
{
    Peça peça;
    public override void Mostrar(Hexagon hexagon, bool mostrarProveides = false)
    {
        peça = (Peça)hexagon;
    }
    public override void Amagar(Hexagon hexagon)
    {

    }
}
