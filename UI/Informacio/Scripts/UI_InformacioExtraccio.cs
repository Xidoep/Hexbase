using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_InformacioExtraccio : UI_Informacio
{

    [SerializeField] TMP_Text estat;

    [Space(10)]
    [SerializeField] UI_Producte producte;

    [Space(10)]
    [SerializeField] Transform parent;
    [SerializeField] GameObject etiquetaPotProduir;
    [SerializeField] GameObject etiquetaProduit;


    Peça peça;
    public override GameObject Setup(Hexagon hexagon)
    {
        peça = (Peça)hexagon;

        estat.text = peça.Subestat.name;


        if (peça.EstaConnectat)
        {
            Produit();
        }
        else
        {
            PotProduir();
        }


        return gameObject;
    }

    void PotProduir()
    {
        etiquetaPotProduir.SetActive(true);
        etiquetaProduit.SetActive(false);

        Instantiate(producte, parent).Setup(peça.Subestat.Producte).transform.localScale = Vector3.one * 0.04f;
    }

    void Produit()
    {
        etiquetaPotProduir.SetActive(false);
        etiquetaProduit.SetActive(true);

        /*for (int i = peça.ProductesExtrets.Length - 1; i >= 0; i--)
        {

            Instantiate(producte, parent).Setup(
                peça.ProductesExtrets[i].producte, 
                peça.ProductesExtrets[i].gastat).transform.localScale = Vector3.one * 0.04f;
        }*/
    }
}
