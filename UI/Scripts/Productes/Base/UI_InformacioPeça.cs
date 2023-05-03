using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using TMPro;
using UnityEngine.Localization.Components;

public class UI_InformacioPeça : MonoBehaviour
{
    //Aixo ha de ser per la bombolla general
    protected const string ICONE = "_Icone";
    protected const string COVERTA = "_Coverta";
    protected const string GASTADA = "_Gastada";
    protected const string PTENCIAL = "_Potencial";
    protected const string START_TIME = "_StartTime";
    public virtual GameObject Setup(Peça peça, int index)
    {
        switch (peça.Subestat.Tipus)
        {
            case Subestat.TipusEnum.Normal:
                //res, no necessita info
                break;
            case Subestat.TipusEnum.Casa:
                //habitants:
                //AAA
                //Needs:
                //[][][]
                break;
            case Subestat.TipusEnum.Productor:
                //-No connectat
                //Can extract:
                //A
                //-Connectat
                //nothing. "Resaltar extraccio"
                break;
            case Subestat.TipusEnum.Extraccio:
                //-No connectat
                //can produce:
                //A
                //-Connectat:
                //producced:
                //00[] Tots els productes, els gastats i no
                break;
            default:
                break;
        }   

        return gameObject;
    }

    [SerializeField] Sprite iconeHabitants;

    //Referencies
    [SerializeField] Transform Informacio1;
    [SerializeField] LocalizeStringEvent localizeStringEvent1;
    //[SerializeField] TMP_Text Informacio1_texte;

    [SerializeField] Transform Informacio2;
    [SerializeField] LocalizeStringEvent localizeStringEvent2;
    //[SerializeField] TMP_Text Informacio2_texte;



    //Idiomes
    [SerializeField] LocalizedString habitants;
    [SerializeField] LocalizedString necessitats;
    [SerializeField] LocalizedString potExtreure;
    [SerializeField] LocalizedString potProduir;
    [SerializeField] LocalizedString produit;

    void SeInformacio1(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            localizeStringEvent1.gameObject.SetActive(false);
            return;
        }

        if(!localizeStringEvent1.gameObject.activeSelf)
            localizeStringEvent1.gameObject.SetActive(true);


    }


}
