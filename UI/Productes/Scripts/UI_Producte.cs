using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Producte : MonoBehaviour
{
    [SerializeField] Pe�a pe�a;
    [SerializeField] Producte producte;
    [SerializeField] int index;
    [Space(20)]
    [SerializeField] Image image;
    [SerializeField] SpriteRenderer sprite;
    [Space(20)]
    [SerializeField] XS_Button boto;


    System.Action<Pe�a, Producte> resaltar;
    System.Action clicar;
    System.Action<Pe�a> desresaltar;
    bool keepit;

    public bool Keepit { get => keepit; set => keepit = value; }
    //public Pe�a Pe�a => pe�a;
    //public Producte Producte => producte;
    //public int Index => index;
    public Pe�a SetPe�a { set => this.pe�a = value; }

    public bool Iguals(Sumari.Informacio info) => info.pe�a == pe�a && info.index == index;

    //Quan s'ha de mostrar amb informacio i ser interactuable.
    public UI_Producte Setup(Pe�a pe�a, Producte producte, int index, System.Action<Pe�a, Producte> resaltar, System.Action<Pe�a> desresaltar, System.Action clicar)
    {
        if (image) image.sprite = producte.Sprite;
        if (sprite) 
        {
            sprite.sprite = producte.Sprite;
        } 
        this.pe�a = pe�a;
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
        if (image) image.sprite = !gastat ? producte.Sprite : producte.Gastada;
        if (sprite) 
        {
            sprite.sprite = !gastat ? producte.Sprite : producte.Gastada;
            //sprite.material.SetTexture("_Icone", (RenderTexture)producte.Sprite);
        } 

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
        desresaltar.Invoke(pe�a);
    } 

    public void Resaltar() => resaltar.Invoke(pe�a, producte);
    public void Clicar()
    {
        clicar.Invoke();
        desresaltar.Invoke(pe�a);
    }
    public void Desresaltar() => desresaltar.Invoke(pe�a);

    public void Borrar() => Destroy(boto.gameObject);
}
