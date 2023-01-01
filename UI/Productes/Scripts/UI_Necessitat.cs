using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Necessitat : UI_InformacioPe�a
{

    public override GameObject Setup(Pe�a pe�a, int index)
    {
        this.necessitat = pe�a.Casa.Necessitats[index];
        this.producte = necessitat.Producte;

        MeshRenderer.material.SetTexture(ICONE, producte.Icone);
        MeshRenderer.material.SetFloat(COVERTA, necessitat.Proveit ? 1 : 0);
        //MeshRenderer.transform.localRotation = Quaternion.Euler(0, 0, Rotacio(index));

        return gameObject;
    }

    float Rotacio(int i) => (((i + 1) / 2) * 12f) * (i % 2 == 0 ? -1 : 1);


    //Debug
    Casa.Necessitat necessitat;
    Producte producte;
}
