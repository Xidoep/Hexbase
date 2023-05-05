using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_InformacioProductor : UI_Informacio
{
    [SerializeField] UI_Producte producte;

    [SerializeField] Transform parent;


    public override GameObject Setup(Hexagon hexagon)
    {
        Instantiate(producte, parent).Setup(((Peça)hexagon).Subestat.Producte).transform.localScale = Vector3.one * 0.04f;

        return gameObject;
    }

}
