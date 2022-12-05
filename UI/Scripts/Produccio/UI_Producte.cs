using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Producte : MonoBehaviour
{
    const string ICONE_NOM = "Icone";
    public GameObject Setup(Subestat_Producte subestat, Producte producte)
    {
        this.subestat = subestat;
        this.producte = producte;
        meshRenderer.material.SetTexture(ICONE_NOM, producte.Icone);
        return this.gameObject;
    }

    [SerializeField] MeshRenderer meshRenderer;

    //Debug
    public Producte producte;
    public Subestat_Producte subestat;
}
