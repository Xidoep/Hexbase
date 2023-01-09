using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Grups")]
public class Informacio_Grup : Informacio
{
    [SerializeField] Grups grups;

    public override void Mostrar(Peça peça, bool mostrarProveides = false)
    {
        if (mostrarProveides)
            grups.ResaltarGrup(peça);
        else
            grups.ReixarDeResaltar();
    }
    public override void Amagar(Peça peça)
    {
        grups.ReixarDeResaltar();
    }

}
