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


    public override GameObject Setup(Hexagon hexagon)
    {
        estat.text = ((Peça)hexagon).Estat.name;

        Instantiate(producte, parent).Setup(((Peça)hexagon).Subestat.Producte).transform.localScale = Vector3.one * 0.04f;

        return gameObject;
    }

}
