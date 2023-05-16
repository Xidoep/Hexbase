using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Connexio")]
public class Informacio_Connexio : Informacio
{
    //[Apartat("Connexio")]
    //[SerializeField] Visualitzacions visualitzacions;

    System.Action<Pe�a, bool> enResaltar;
    public System.Action<Pe�a, bool> EnResaltar { get => enResaltar; set => enResaltar = value; }

    public override void Mostrar(Hexagon hexagon)
    {
        if (!Comprovacions(hexagon))
            return;

        //visualitzacions.DestacarPe�a(((Pe�a)hexagon).Connexio, true);
        enResaltar(((Pe�a)hexagon).Connexio, true);
    }
    public override void Amagar(Hexagon hexagon)
    {
        if (!Comprovacions(hexagon))
            return;

        //visualitzacions.DestacarPe�a(((Pe�a)hexagon).Connexio, false);
        enResaltar(((Pe�a)hexagon).Connexio, false);
    }
}
