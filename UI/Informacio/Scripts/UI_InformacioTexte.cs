using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class UI_InformacioTexte : UI_Informacio
{
    [SerializeField] LocalizeStringEvent localizeStringEvent;

    public void Setup(LocalizedString localizedString)
    {
        localizeStringEvent.StringReference = localizedString;
        localizeStringEvent.gameObject.SetActive(true);
    }

    public override GameObject Setup(Hexagon hexagon)
    {
        localizeStringEvent.StringReference = ((Boto)hexagon).Texte;
        localizeStringEvent.gameObject.SetActive(true);

        return gameObject;
    }


}
