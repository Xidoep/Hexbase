using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Necessitat : UI_InformacioPeça
{

    public override GameObject Setup(Peça peça, int index)
    {
        this.casa = peça.Cases[index];
        this.producte = casa.Necessitats[0].Producte;

        MeshRenderer.material.SetTexture(ICONE_NOM, producte.Icone);
        MeshRenderer.material.SetFloat(COVERTA_NOM, casa.Necessitats[0].Proveit ? 1 : 0);
        MeshRenderer.transform.localRotation = Quaternion.Euler(0, 0, Rotacio(index));

        return gameObject;
    }

    float Rotacio(int i) => (((i + 1) / 2) * 12f) * (i % 2 == 0 ? -1 : 1);


    //Debug
    Casa casa;
    Producte producte;
}
