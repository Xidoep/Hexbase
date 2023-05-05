using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Grups")]
public class Informacio_Grup : Informacio
{
    [Apartat("Grup")]
    [SerializeField] Grups grups;

    public override void Mostrar(Hexagon peça)
    {
        grups.ResaltarGrup((Peça)peça);
    }
    public override void Amagar(Hexagon peça)
    {
        grups.ReixarDeResaltar();
    }

}
