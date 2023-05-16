using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Grups")]
public class Informacio_Grup : Informacio
{
    //[Apartat("Grup")]
    //[SerializeField] Grups grups;

    System.Action<Peça> enResaltar;
    System.Action enDesresaltar;
    public System.Action<Peça> EnResaltar { get => enResaltar; set => enResaltar = value; }
    public System.Action EnDesresaltar { get => enDesresaltar; set => enDesresaltar = value; }

    public override void Mostrar(Hexagon hexagon)
    {
        //grups.ResaltarGrup((Peça)hexagon);
        enResaltar((Peça)hexagon);
    }
    public override void Amagar(Hexagon hexagon)
    {
        //grups.ReixarDeResaltar();
        enDesresaltar();
    }

}
