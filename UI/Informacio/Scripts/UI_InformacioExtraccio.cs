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


    Pe�a pe�a;
    public override GameObject Setup(Hexagon hexagon)
    {
        pe�a = (Pe�a)hexagon;

        estat.text = pe�a.Subestat.name;


        if (pe�a.EstaConnectat)
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

        Instantiate(producte, parent).Setup(pe�a.Subestat.Producte).transform.localScale = Vector3.one * 0.04f;
    }

    void Produit()
    {
        etiquetaPotProduir.SetActive(false);
        etiquetaProduit.SetActive(true);

        /*for (int i = pe�a.ProductesExtrets.Length - 1; i >= 0; i--)
        {

            Instantiate(producte, parent).Setup(
                pe�a.ProductesExtrets[i].producte, 
                pe�a.ProductesExtrets[i].gastat).transform.localScale = Vector3.one * 0.04f;
        }*/
    }
}
