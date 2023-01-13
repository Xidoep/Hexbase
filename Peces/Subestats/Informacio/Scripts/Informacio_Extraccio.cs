using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Extraccio")]
public class Informacio_Extraccio : Informacio
{
    [SerializeField] Visualitzacions visualitzacions;
    public override void Mostrar(Peça peça, bool mostrarProveides = false)
    {
        visualitzacions.DestacarPeça(peça.Extraccio, mostrarProveides);
    }
    public override void Amagar(Peça peça)
    {
        visualitzacions.DestacarPeça(peça, false);
    }
}
