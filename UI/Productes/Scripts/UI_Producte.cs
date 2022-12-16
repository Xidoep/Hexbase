using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Producte : UI_InformacioPeça
{

    public override GameObject Setup(Peça peça, int index)
    {
        this.subestat = peça.Subestat;
        this.producte = peça.productesExtrets[index];

        MeshRenderer.material.SetTexture(ICONE_NOM, subestat.Producte.Icone);
        MeshRenderer.material.SetFloat(COVERTA_NOM, !peça.productesExtrets[index].gastat ? 1 : 0);

        return gameObject;
    }

    //Debug
    Peça.ProducteExtret producte;
    Subestat subestat;
}
