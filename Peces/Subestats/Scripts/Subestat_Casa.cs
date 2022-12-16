using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Casa")]
public class Subestat_Casa : Subestat
{
    [Apartat("SUBESTAT CASA")]
    [SerializeField] Repoblar repoblar;
    public override Subestat Setup(Pe�a pe�a)
    {
        if (!pe�a.TeCasa)
            pe�a.CrearCasa(repoblar.NecessitatInicial);
        
        return base.Setup(pe�a);
    }
}
