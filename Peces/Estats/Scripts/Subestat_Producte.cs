using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Producte")]
public class Subestat_Producte : Estat//, IProcessable
{
    //[Apartat("SUBESTAT PRODUCTE")]
    public override Estat Setup(Peça peça)
    {
        return base.Setup(peça);
    }

    /*new public void Processar(Peça peça)
    {
        Debug.Log($"PROCESSAR SUBESTAT PRODUCTE {this.name}");
        peça.IntentarConnectar();
        base.Processar(peça);
    }*/

    //public override bool EsProducte => true;

}
