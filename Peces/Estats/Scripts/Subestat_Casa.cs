using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Casa")]
public class Subestat_Casa : Estat
{
    //[Apartat("SUBESTAT CASA")]
    //[SerializeField] Repoblar repoblar;
    public override Estat Setup(Pe�a pe�a)
    {
        /*if (!pe�a.TeCasa)
        {
            repoblar.AfegirLaPrimeraCasa(pe�a);
        }*/

        return base.Setup(pe�a);
    }
}
