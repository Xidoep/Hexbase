using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Producte")]
public class Subestat_Producte : Subestat//, IProcessable
{
    //[Apartat("SUBESTAT PRODUCTE")]
    public override Subestat Setup(Pe�a pe�a)
    {
        return base.Setup(pe�a);
    }

    /*new public void Processar(Pe�a pe�a)
    {
        Debug.Log($"PROCESSAR SUBESTAT PRODUCTE {this.name}");
        pe�a.IntentarConnectar();
        base.Processar(pe�a);
    }*/

    //public override bool EsProducte => true;

}
