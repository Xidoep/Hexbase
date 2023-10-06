using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_InformacioEstat : UI_Informacio
{
    [SerializeField] TMP_Text estat;
    public override GameObject Setup(Hexagon hexagon)
    {
        estat.text = ((Pe�a)hexagon).Estat.name;

        return gameObject;
    }
}
