using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Producte : UI_InformacioPeça
{

    public override GameObject Setup(Peça peça, int index)
    {
        this.subestat = peça.Subestat;
        this.producte = peça.productesExtrets[index];

        MeshRenderer.material.SetTexture(ICONE, subestat.Producte.Icone);
        MeshRenderer.material.SetFloat(GASTADA, peça.productesExtrets[index].gastat ? 1 : 0);
        MeshRenderer.material.SetFloat(START_TIME, Time.time + 1000);

        return gameObject;
    }



    public void Destruir(float time)
    {
        MeshRenderer.material.SetFloat(START_TIME, Time.time + time);
    }

    //Debug
    Peça.ProducteExtret producte;
    Subestat subestat;
}
