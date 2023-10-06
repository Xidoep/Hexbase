using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using TMPro;
using UnityEngine.Localization.Components;

public class UI_InformacioCasa : UI_Informacio
{
    [SerializeField] Utils_InstantiableFromProject habitant;
    [SerializeField] UI_Producte producte;

    [Space(10)]
    [SerializeField] Transform parentHabitants;
    [SerializeField] Transform parentProductes;
    
    Peça peça;

    public override GameObject Setup(Hexagon hexagon)
    {
        peça = (Peça)hexagon;

        for (int c = 0; c < peça.CasesLength; c++)
        {
            habitant.InstantiateReturn(parentHabitants).transform.localScale = Vector3.one * 0.04f;
            for (int n = 0; n < peça.Cases[c].Necessitats.Count; n++)
            {
                Instantiate(producte, parentProductes).Setup(peça.Cases[c].Necessitats[n], false).transform.localScale = Vector3.one * 0.04f;
            }
        }

        return gameObject;
    }

}
