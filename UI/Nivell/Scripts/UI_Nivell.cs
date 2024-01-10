using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class UI_Nivell : MonoBehaviour
{
    [SerializeScriptableObject] [SerializeField] Fase_Resoldre resoldre;
    [SerializeScriptableObject] [SerializeField] Nivell nivell;

    [Apartat("UI")]
    [SerializeField] Transform pivot;
    [SerializeField] Image uiCercle;
    [SerializeField] TMP_Text uiNivell;
    [SerializeField] TMP_Text uiExperencia;
    [Space(10)]
    [FoldoutGroup("Animacions"), SerializeField, SerializeScriptableObject] AnimacioPerCodi pujarNivell;
    [FoldoutGroup("Animacions"), SerializeField, SerializeScriptableObject] AnimacioPerCodi guanyarExperiencia;
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
        pujarNivell?.Play(pivot);
        //ANIMACIOOOO!!!
        SetNivell(nivell);
    }
    void PujarExperiencia(int total, int guanyada)
    {
        for (int i = 0; i < guanyada; i++)
        {
            StartCoroutine(AnimacioExperiencia(i * 0.4f, total - guanyada + i + 1));
        }

        //guanyarExperiencia.Play(pivot);
        Debug.Log("UI Pujar experiencia");
        //ANIMACIO!!!
        //SetExperiencia(total);
    }



    void SetNivell(int nivell) => uiNivell.text = nivell.ToString();
    void SetExperiencia(int experiencia)
    {
        uiCercle.fillAmount = this.nivell.FactorExperienciaNivellActual(experiencia);
        uiExperencia.text = $"{experiencia} / {this.nivell.ExperienciaNecessariaProximNivell}";
    }

    public void Amagar() => Destroy(this.gameObject);

    IEnumerator AnimacioExperiencia(float temps, int experiencia)
    {
        yield return new WaitForSeconds(temps);
        SetExperiencia(experiencia);
        guanyarExperiencia.Play(pivot);
    }
}
