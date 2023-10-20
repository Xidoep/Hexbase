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

    Pe�a pe�a;

    public override GameObject Setup(Hexagon hexagon)
    {
        pe�a = (Pe�a)hexagon;
        estat.text = pe�a.Subestat.name;

        //Instantiate(producte, parent).Setup(((Pe�a)hexagon).Subestat.Producte).transform.localScale = Vector3.one * 0.04f;

        for (int i = pe�a.Connexio.ProductesExtrets.Length - 1; i >= 0; i--)
        {

            Instantiate(producte, parent).Setup(
                pe�a.Connexio.ProductesExtrets[i].producte,
                pe�a.Connexio.ProductesExtrets[i].gastat).transform.localScale = Vector3.one * 0.04f;
        }

        return gameObject;
    }

}
