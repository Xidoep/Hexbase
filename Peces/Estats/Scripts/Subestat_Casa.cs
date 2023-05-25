using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Casa")]
public class Subestat_Casa : Estat
{
    //[Apartat("SUBESTAT CASA")]
    //[SerializeField] Repoblar repoblar;
    public override Estat Setup(Peça peça)
    {
        /*if (!peça.TeCasa)
        {
            repoblar.AfegirLaPrimeraCasa(peça);
        }*/

        return base.Setup(peça);
    }
}
