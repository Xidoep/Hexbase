using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Nivell : MonoBehaviour
{
    [SerializeScriptableObject] [SerializeField] Fase_Resoldre resoldre;
    [SerializeScriptableObject] [SerializeField] Nivell nivell;

    [Apartat("UI")]
    [SerializeField] Image uiCercle;
    [SerializeField] TMP_Text uiNivell;
    [SerializeField] TMP_Text uiExperencia;

    private void OnEnable()
    {
        nivell.EnGuanyarExperiencia += PujarExperiencia;
        nivell.EnPujarNivell += PujarNivell;
        resoldre.EnTornar += Amagar;
        resoldre.EnRepetir += Amagar;
        resoldre.EnContinuar += Amagar;

        SetNivell(1);
        SetExperiencia(0);
    }

    void OnDisable()
    {
        nivell.EnGuanyarExperiencia -= PujarExperiencia;
        nivell.EnPujarNivell -= PujarNivell;
        resoldre.EnTornar -= Amagar;
        resoldre.EnRepetir -= Amagar;
        resoldre.EnContinuar -= Amagar;
    }

    void PujarNivell(int nivell)
    {
        //ANIMACIOOOO!!!
        SetNivell(nivell);
    }
    void PujarExperiencia(int experiencia)
    {
        Debug.Log("UI Pujar experiencia");
        //ANIMACIO!!!
        SetExperiencia(experiencia);
    }



    void SetNivell(int nivell) => uiNivell.text = nivell.ToString();
    void SetExperiencia(int experiencia)
    {
        uiCercle.fillAmount = this.nivell.FactorExperienciaNivellActual;
        uiExperencia.text = $"{experiencia} / {this.nivell.ExperienciaNecessariaProximNivell}";
    }

    public void Amagar() => Destroy(this.gameObject);

}
