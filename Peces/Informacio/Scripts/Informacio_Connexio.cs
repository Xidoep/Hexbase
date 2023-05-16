using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Connexio")]
public class Informacio_Connexio : Informacio
{
    //[Apartat("Connexio")]
    //[SerializeField] Visualitzacions visualitzacions;

    System.Action<Peça, bool> enResaltar;
    public System.Action<Peça, bool> EnResaltar { get => enResaltar; set => enResaltar = value; }

    public override void Mostrar(Hexagon hexagon)
    {
        if (!Comprovacions(hexagon))
            return;

        //visualitzacions.DestacarPeça(((Peça)hexagon).Connexio, true);
        enResaltar(((Peça)hexagon).Connexio, true);
    }
    public override void Amagar(Hexagon hexagon)
    {
        if (!Comprovacions(hexagon))
            return;

        //visualitzacions.DestacarPeça(((Peça)hexagon).Connexio, false);
        enResaltar(((Peça)hexagon).Connexio, false);
    }
}
