using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/General")]
public class Informacio_General : Informacio
{
    Pe�a pe�a;
    public override void Mostrar(Hexagon hexagon, bool mostrarProveides = false)
    {
        pe�a = (Pe�a)hexagon;
    }
    public override void Amagar(Hexagon hexagon)
    {

    }
}
