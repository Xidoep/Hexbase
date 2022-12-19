using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Producte : UI_InformacioPe�a
{

    public override GameObject Setup(Pe�a pe�a, int index)
    {
        this.subestat = pe�a.Subestat;
        this.producte = pe�a.productesExtrets[index];

        MeshRenderer.material.SetTexture(ICONE, subestat.Producte.Icone);
        MeshRenderer.material.SetFloat(GASTADA, pe�a.productesExtrets[index].gastat ? 1 : 0);
        MeshRenderer.material.SetFloat(START_TIME, Time.time + 1000);

        return gameObject;
    }



    public void Destruir(float time)
    {
        MeshRenderer.material.SetFloat(START_TIME, Time.time + time);
    }

    //Debug
    Pe�a.ProducteExtret producte;
    Subestat subestat;
}
