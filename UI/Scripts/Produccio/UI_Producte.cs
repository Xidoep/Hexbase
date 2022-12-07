using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Producte : UI_InformacioPe�a
{

    public override GameObject Setup(Pe�a pe�a, int index)
    {
        this.subestat = pe�a.Subestat;

        this.producte = subestat.Producte;
        MeshRenderer.material.SetTexture(ICONE_NOM, subestat.Producte.Icone);

        return gameObject;
    }

    //Debug
    Producte producte;
    Subestat subestat;
}
