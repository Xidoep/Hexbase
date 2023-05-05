using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Grups")]
public class Informacio_Grup : Informacio
{
    [Apartat("Grup")]
    [SerializeField] Grups grups;

    public override void Mostrar(Hexagon pe�a)
    {
        grups.ResaltarGrup((Pe�a)pe�a);
    }
    public override void Amagar(Hexagon pe�a)
    {
        grups.ReixarDeResaltar();
    }

}
