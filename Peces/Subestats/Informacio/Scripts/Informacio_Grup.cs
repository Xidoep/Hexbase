using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Grups")]
public class Informacio_Grup : Informacio
{
    [SerializeField] Grups grups;

    public override void Mostrar(Pe�a pe�a, bool mostrarProveides = false)
    {
        if (mostrarProveides)
            grups.ResaltarGrup(pe�a);
        else
            grups.ReixarDeResaltar();
    }
    public override void Amagar(Pe�a pe�a)
    {
        grups.ReixarDeResaltar();
    }

}
