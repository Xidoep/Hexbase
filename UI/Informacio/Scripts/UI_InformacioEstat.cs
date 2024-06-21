using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_InformacioEstat : UI_Informacio
{
    [SerializeField] TMP_Text estat;
    public override GameObject Setup(Hexagon hexagon)
    {
        estat.text = ((Peça)hexagon).Subestat.name;

        return gameObject;
    }
    public GameObject Setup(Estat estat)
    {
        this.estat.text = estat.name;

        return gameObject;
    }
}
