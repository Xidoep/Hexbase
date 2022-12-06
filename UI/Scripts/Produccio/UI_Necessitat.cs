using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Necessitat : MonoBehaviour
{
    const string ICONE_NOM = "_Icone";
    public GameObject Setup(Casa casa, float rotacio)
    {
        this.casa = casa;
        this.producte = casa.Necessitats[0].Producte;

        meshRenderer.material.SetTexture(ICONE_NOM, producte.Icone);
        meshRenderer.transform.localRotation = Quaternion.Euler(0, 0, rotacio);
        return this.gameObject;
    }

    [SerializeField] MeshRenderer meshRenderer;

    //Debug
    Casa casa;
    Producte producte;
}
