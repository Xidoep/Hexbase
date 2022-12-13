using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Producte : UI_InformacioPeça
{

    public override GameObject Setup(Peça peça, int index)
    {
        this.subestat = peça.Subestat;

        this.producte = subestat.Producte;
        MeshRenderer.material.SetTexture(ICONE_NOM, subestat.Producte.Icone);

        return gameObject;
    }

    //Debug
    Producte producte;
    Subestat subestat;
}
