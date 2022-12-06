using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Producte : MonoBehaviour
{
    const string ICONE_NOM = "_Icone";
    public GameObject Setup(Subestat_Producte subestat)
    {
        this.subestat = subestat;
        this.producte = subestat.Producte;
        meshRenderer.material.SetTexture(ICONE_NOM, subestat.Producte.Icone);
        return this.gameObject;
    }

    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Animacio apareixre;

    //Debug
    Producte producte;
    Subestat_Producte subestat;
}
