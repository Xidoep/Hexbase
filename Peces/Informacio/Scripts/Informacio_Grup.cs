using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Grups")]
public class Informacio_Grup : Informacio
{
    [SerializeField] Grups grups;

    public override void Mostrar(Hexagon pe�a, bool mostrarProveides = false)
    {
        if (mostrarProveides)
            grups.ResaltarGrup((Pe�a)pe�a);
        else
            grups.ReixarDeResaltar();
    }
    public override void Amagar(Hexagon pe�a)
    {
        grups.ReixarDeResaltar();
    }

}
