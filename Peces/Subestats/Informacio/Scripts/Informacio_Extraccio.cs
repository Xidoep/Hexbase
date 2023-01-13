using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Extraccio")]
public class Informacio_Extraccio : Informacio
{
    [SerializeField] Visualitzacions visualitzacions;
    public override void Mostrar(Pe�a pe�a, bool mostrarProveides = false)
    {
        visualitzacions.DestacarPe�a(pe�a.Extraccio, mostrarProveides);
    }
    public override void Amagar(Pe�a pe�a)
    {
        visualitzacions.DestacarPe�a(pe�a, false);
    }
}
