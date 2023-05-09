using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Producte : MonoBehaviour
{
    [SerializeField] Pe�a pe�a;
    [SerializeField] Producte producte;
    [Space(20)]
    [SerializeField] Image image;
    [SerializeField] XS_Button boto;


    System.Action<Pe�a, Producte> resaltar;
    bool keepit;

    public bool Keepit { get => keepit; set => keepit = value; }
    public Producte Producte => producte;
    public Pe�a SetPe�a { set => this.pe�a = value; }

    //Quan s'ha de mostrar amb informacio i ser interactuable.
    public UI_Producte Setup(Pe�a pe�a, Producte producte, System.Action<Pe�a, Producte> resaltar, System.Action desresaltar)
    {
        image.sprite = producte.Sprite;
        this.pe�a = pe�a;
        this.producte = producte;
        this.resaltar = resaltar;
        boto.OnEnter = Resaltar;
        boto.OnExit = desresaltar;
        return this;
    }

    //Quan s'ha de mostrar sense cap tipus d'infurmacio
    public GameObject Setup(Producte producte, bool gastat = false)
    {
        image.sprite = !gastat ? producte.Sprite : producte.Gastada;

        return gameObject;
    }


    public void Mostrar() => gameObject.SetActive(true);

    public void Amagar() => boto.Disable();

    public void Resaltar() => resaltar.Invoke(pe�a, producte);

    public void Borrar() => Destroy(boto.gameObject);
}
