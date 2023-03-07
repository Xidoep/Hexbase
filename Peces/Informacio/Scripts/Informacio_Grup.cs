using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Grups")]
public class Informacio_Grup : Informacio
{
    [SerializeField] Grups grups;

    public override void Mostrar(Hexagon peça, bool mostrarProveides = false)
    {
        if (mostrarProveides)
            grups.ResaltarGrup((Peça)peça);
        else
            grups.ReixarDeResaltar();
    }
    public override void Amagar(Hexagon peça)
    {
        grups.ReixarDeResaltar();
    }

}
