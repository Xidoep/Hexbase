using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Nivell : MonoBehaviour
{
    [SerializeField] Fase_Resoldre resoldre;
    [SerializeField] Visualitzacions visualitzacions;

    [Apartat("UI")]
    [SerializeField] Transform pivot;
    [SerializeField] Image uiCercle;
    [SerializeField] TMP_Text uiNivell;
    [SerializeField] TMP_Text uiExperencia;

    int nivell, experiencia;

    private void OnEnable()
    {
        resoldre.Nivell.EnGuanyarExperiencia += EsperarAnimacioGuanyarPunts;
        resoldre.Nivell.EnPujarNivell += EsperarAnimacioGuanyarPunts;
        resoldre.EnTornar += Amagar;
        resoldre.EnRepetir += Amagar;
        resoldre.EnContinuar += Amagar;

        ActualitarUI(1, 0);
    }

    void OnDisable()
    {
        resoldre.Nivell.EnGuanyarExperiencia -= EsperarAnimacioGuanyarPunts;
        resoldre.Nivell.EnPujarNivell -= EsperarAnimacioGuanyarPunts;
        resoldre.EnTornar -= Amagar;
        resoldre.EnRepetir -= Amagar;
        resoldre.EnContinuar -= Amagar;
    }

    void ActualitarUI() => ActualitarUI(nivell, experiencia);
    void ActualitarUI(int nivell, int experiencia)
    {
        uiCercle.fillAmount = resoldre.Nivell.FactorExperienciaNivellActual;
        uiNivell.text = nivell.ToString();
        uiExperencia.text = $"{experiencia} / {resoldre.Nivell.ProximNivell(nivell)}";
    }
    void EsperarAnimacioGuanyarPunts(int nivell, int experiencia)
    {
        this.nivell = nivell;
        this.experiencia = experiencia;
        visualitzacions.Delegar_ActualitzarNivell(pivot, ActualitarUI);
    }

    public void Amagar()
    {
        Destroy(this.gameObject);
    }

}
