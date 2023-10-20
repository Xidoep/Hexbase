using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_InformacioProductor : UI_Informacio
{
    [SerializeField] TMP_Text estat;

    [Space(10)]
    [SerializeField] UI_Producte producte;
    [SerializeField] Transform parent;

    Peça peça;

    public override GameObject Setup(Hexagon hexagon)
    {
        peça = (Peça)hexagon;
        estat.text = peça.Subestat.name;

        //Instantiate(producte, parent).Setup(((Peça)hexagon).Subestat.Producte).transform.localScale = Vector3.one * 0.04f;

        for (int i = peça.Connexio.ProductesExtrets.Length - 1; i >= 0; i--)
        {

            Instantiate(producte, parent).Setup(
                peça.Connexio.ProductesExtrets[i].producte,
                peça.Connexio.ProductesExtrets[i].gastat).transform.localScale = Vector3.one * 0.04f;
        }

        return gameObject;
    }

}
