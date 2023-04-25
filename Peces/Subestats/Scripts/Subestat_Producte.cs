using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Producte")]
public class Subestat_Producte : Subestat//, IProcessable
{
    //[Apartat("SUBESTAT PRODUCTE")]
    public override Subestat Setup(Peça peça)
    {
        base.Setup(peça);

        return this;
    }

    /*new public void Processar(Peça peça)
    {
        Debug.Log($"PROCESSAR SUBESTAT PRODUCTE {this.name}");
        peça.IntentarConnectar();
        base.Processar(peça);
    }*/

    //public override bool EsProducte => true;

}
