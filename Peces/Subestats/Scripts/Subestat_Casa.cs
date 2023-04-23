using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Casa")]
public class Subestat_Casa : Subestat
{
    [Apartat("SUBESTAT CASA")]
    [SerializeField] Repoblar repoblar;
    public override Subestat Setup(Peça peça)
    {
        base.Setup(peça);

        if (!peça.TeCasa)
        {
            repoblar.AfegirLaPrimeraCasa(peça);
        }

        return this;
    }
}
