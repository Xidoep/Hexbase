using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Producte : UI_InformacioPe�a
{

    public override GameObject Setup(Pe�a pe�a, int index)
    {
        this.subestat = pe�a.Subestat;
        this.producte = pe�a.ProductesExtrets[index];

        MeshRenderer.material.SetTexture(ICONE, pe�a.ProductesExtrets[index].producte.Icone);
        MeshRenderer.material.SetFloat(GASTADA, pe�a.ProductesExtrets[index].gastat ? 1 : 0);
        MeshRenderer.material.SetFloat(PTENCIAL, pe�a.Connectat ? 0 : 1);
        MeshRenderer.material.SetFloat(START_TIME, Time.time + 1000);

        return gameObject;
    }



    public void Destruir(float time)
    {
        MeshRenderer.material.SetFloat(START_TIME, Time.time + time);
        //Destroy(gameObject, time + .5f);
    }

    //Debug
    ProducteExtret producte;
    Subestat subestat;
}
