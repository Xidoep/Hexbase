using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class UI_InformacioTexte : MonoBehaviour
{
    public void Setup(LocalizedString localizedString)
    {
        localizeStringEvent.StringReference = localizedString;
        localizeStringEvent.gameObject.SetActive(true);
    }

    [SerializeField] LocalizeStringEvent localizeStringEvent;

}
