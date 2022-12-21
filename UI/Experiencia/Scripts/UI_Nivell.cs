using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Nivell : MonoBehaviour
{
    [SerializeField] Fase_Resoldre resoldre;

    [Apartat("UI")]
    [SerializeField] Image uiCercle;
    [SerializeField] TMP_Text uiNivell;
    [SerializeField] TMP_Text uiExperencia;

    private void OnEnable()
    {
        resoldre.Nivell.EnGuanyarExperiencia += ActualitarUI;
        resoldre.Nivell.EnPujarNivell += ActualitarUI;
        resoldre.EnTornar += Amagar;
        resoldre.EnRepetir += Amagar;
        resoldre.EnContinuar += Amagar;
        ActualitarUI(1, 0);
    }

    void OnDisable()
    {
        resoldre.Nivell.EnGuanyarExperiencia -= ActualitarUI;
        resoldre.Nivell.EnPujarNivell -= ActualitarUI;
        resoldre.EnTornar -= Amagar;
        resoldre.EnRepetir -= Amagar;
        resoldre.EnContinuar -= Amagar;
    }

    void ActualitarUI(int nivell, int experiencia)
    {
        uiCercle.fillAmount = resoldre.Nivell.FactorExperienciaNivellActual;
        uiNivell.text = nivell.ToString();
        uiExperencia.text = $"{experiencia} / {resoldre.Nivell.ProximNivell(nivell)}";
    }

    public void Amagar()
    {
        Destroy(this.gameObject);
    }

}
