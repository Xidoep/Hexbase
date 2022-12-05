using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Necessitat : MonoBehaviour
{
    const string ICONE_NOM = "Icone";
    public GameObject Setup(Casa casa)
    {
        this.casa = casa;
        this.producte = casa.Necessitats[0].Producte;

        meshRenderer.material.SetTexture(ICONE_NOM, producte.Icone);
        return this.gameObject;
    }

    [SerializeField] MeshRenderer meshRenderer;

    //Debug
    public Casa casa;
    public Producte producte;
}
