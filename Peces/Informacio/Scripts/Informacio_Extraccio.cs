using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Extraccio")]
public class Informacio_Extraccio : Informacio
{
    [SerializeField] Visualitzacions visualitzacions;
    public override void Mostrar(Hexagon hexagon, bool mostrarProveides = false)
    {
        visualitzacions.DestacarPeša(((Peša)hexagon).GetExtraccio, mostrarProveides);
    }
    public override void Amagar(Hexagon hexagon)
    {
        visualitzacions.DestacarPeša(hexagon, false);
    }
}
