using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Producte : MonoBehaviour
{
    [SerializeField] Peça peça;
    [SerializeField] Producte producte;
    [SerializeField] int index;
    [Space(20)]
    [SerializeField] Image image;
    [SerializeField] XS_Button boto;


    System.Action<Peça, Producte> resaltar;
    System.Action clicar;
    System.Action desresaltar;
    bool keepit;

    public bool Keepit { get => keepit; set => keepit = value; }
    //public Peça Peça => peça;
    //public Producte Producte => producte;
    //public int Index => index;
    public Peça SetPeça { set => this.peça = value; }

    public bool Iguals(Sumari.Informacio info) => info.peça == peça && info.index == index;

    //Quan s'ha de mostrar amb informacio i ser interactuable.
    public UI_Producte Setup(Peça peça, Producte producte, int index, System.Action<Peça, Producte> resaltar, System.Action desresaltar, System.Action clicar)
    {
        image.sprite = producte.Sprite;
        this.peça = peça;
        this.producte = producte;
        this.index = index;

        this.resaltar = resaltar;
        this.clicar = clicar;
        this.desresaltar = desresaltar;

        boto.OnEnter = Resaltar;
        boto.onClick.AddListener(Clicar);
        boto.OnExit = Desresaltar;

        return this;
    }

    //Quan s'ha de mostrar sense cap tipus d'infurmacio
    public GameObject Setup(Producte producte, bool gastat = false)
    {
        image.sprite = !gastat ? producte.Sprite : producte.Gastada;

        return gameObject;
    }


    public void Mostrar() 
    {
        gameObject.SetActive(true);
        boto.Interactable(true);
    }

    public void Amagar() 
    {
        boto.Disable();
        boto.Interactable(false);
        desresaltar.Invoke();
    } 

    public void Resaltar() => resaltar.Invoke(peça, producte);
    public void Clicar()
    {
        clicar.Invoke();
        desresaltar.Invoke();
    }
    public void Desresaltar() => desresaltar.Invoke();

    public void Borrar() => Destroy(boto.gameObject);
}
