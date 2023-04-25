using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Producte : UI_InformacioPeça
{

    public override GameObject Setup(Peça peça, int index)
    {
        this.subestat = peça.Subestat;
        this.producte = peça.ExtreureProducte[index];

        MeshRenderer.material.SetTexture(ICONE, peça.ExtreureProducte[index].producte.Icone);
        MeshRenderer.material.SetFloat(GASTADA, peça.ExtreureProducte[index].gastat ? 1 : 0);
        MeshRenderer.material.SetFloat(PTENCIAL, peça.Connectat ? 0 : 1);
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
