using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Nivell : MonoBehaviour
{
    [Header("VISUALITZACIO")]
    [SerializeField] int nivell;
    [SerializeField] int experiencia;

    [Apartat("REFERENCIES")]
    [SerializeField] Image uiCercle;
    [SerializeField] TMP_Text uiNivell;
    [SerializeField] TMP_Text uiExperencia;

    [Apartat("EVENTS")]
    [SerializeField] Canal_Integre EnPujarNivell;

    int ProximNivell(int nivell) => nivell * nivell * 10;
    float Cercle => (experiencia - (ProximNivell(nivell - 1))) / (float)((ProximNivell(nivell) - ProximNivell(nivell - 1)));

    private void OnEnable()
    {
        ActualitarUI();
    }

    [ContextMenu("mes 2")] void Prova2() => Experiencia(2);
    [ContextMenu("mes 20")] void Prova20() => Experiencia(20);
    [ContextMenu("mes 200")] void Prova200() => Experiencia(200);
    public void Experiencia(int xp)
    {
        experiencia += xp;
        if(experiencia >= ProximNivell(nivell))
        {
            nivell++;
            EnPujarNivell.Invocar(nivell);
        }

        ActualitarUI();
    }

    void ActualitarUI()
    {
        Debug.Log(Cercle);
        uiCercle.fillAmount = Cercle;
        uiNivell.text = nivell.ToString();
        uiExperencia.text = $"{experiencia} / {ProximNivell(nivell)}";
    }
}
