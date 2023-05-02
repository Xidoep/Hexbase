using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using TMPro;
using UnityEngine.Localization.Components;

public class UI_InformacioPeça : MonoBehaviour
{
    [SerializeField] LocalizedString texte;

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

    //Referencies
    [SerializeField] TMP_Text etiquetaPrimera;
    [SerializeField] TMP_Text etiquetaSegona;
    [SerializeField] LocalizeStringEvent localizeStringEvent;

    //Idiomes
    [SerializeField] LocalizedString habitants;
    [SerializeField] LocalizedString necessitats;
    [SerializeField] LocalizedString potExtreure;
    [SerializeField] LocalizedString potProduir;
    [SerializeField] LocalizedString produit;

    void SetEtiqueta(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            etiquetaPrimera.gameObject.SetActive(false);
            return;
        }

        if(!etiquetaPrimera.gameObject.activeSelf)
            etiquetaPrimera.gameObject.SetActive(true);

        //etiquetaPrimera.text
    }









    [SerializeField] MeshRenderer meshRenderer;


    protected MeshRenderer MeshRenderer => meshRenderer;
}
