using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Producte : UI_InformacioPe�a
{

    public override GameObject Setup(Pe�a pe�a, int index)
    {
        this.subestat = pe�a.Subestat;
        this.producte = pe�a.productesExtrets[index];

        MeshRenderer.material.SetTexture(ICONE_NOM, subestat.Producte.Icone);
        MeshRenderer.material.SetFloat(COVERTA_NOM, !pe�a.productesExtrets[index].gastat ? 1 : 0);

        return gameObject;
    }

    //Debug
    Pe�a.ProducteExtret producte;
    Subestat subestat;
}
