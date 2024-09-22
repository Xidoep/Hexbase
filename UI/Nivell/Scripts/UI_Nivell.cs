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
    [Space(10)]
    [FoldoutGroup("Audio"), SerializeField, SerializeScriptableObject] So soExperiencia;
    [FoldoutGroup("Audio"), SerializeField, SerializeScriptableObject] So soPujarNivell;
    [Space(10)]
    [FoldoutGroup("Particules"), SerializeField] GameObject novesPeces;

    public List<bool> prova;
    public List<System.Action<int>> accions;
    public int nivellMostrat;
    public int experienciaMostrada;
    public float pitch;
    public bool efectes;

    private void OnEnable()
    {
        prova = new List<bool>();
        accions = new List<System.Action<int>>();

        nivell.EnGuanyarExperiencia += PujarExperiencia;
        nivell.EnPujarNivell += PujarNivell;
        resoldre.EnTornar += Amagar;
        resoldre.EnRepetir += Amagar;
        resoldre.EnContinuar += Amagar;

        efectes = false;
        SetNivell(1);
        SetExperiencia(0);
        efectes = true;
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
        Debug.Log($"Pujar Nivell: {nivell}");
        prova.Add(true);
        accions.Add(SetNivell);
        
        //pujarNivell?.Play(pivot);
        //ANIMACIOOOO!!!
        //SetNivell(nivell);
    }
    void PujarExperiencia(int total, int guanyada)
    {
        Debug.Log($"PujarExperiencia: {guanyada}");
        pitch = 1.3f;
        for (int i = 0; i < guanyada; i++)
        {
            prova.Add(false);
            accions.Add(SetExperiencia);
            //StartCoroutine(AnimacioExperiencia(i * 0.3f, total - guanyada + i + 1));
        }

        StartCoroutine(AnimacioAccio(0, total - guanyada +1, guanyada));

    }




    void SetNivell(int experiencia)
    {
        Debug.Log($"nivell: {experiencia}");
        nivellMostrat = nivell.GetNivell;
        uiNivell.text = nivell.GetNivell.ToString();

        uiCercle.fillAmount = nivell.FactorExperienciaNivellActual(nivellMostrat, experiencia);
        uiExperencia.text = $"{experiencia} / {nivell.ExperienciaNecessariaProximNivell(nivellMostrat - 1)}";

        if (!efectes)
            return;

        pujarNivell?.Play(pivot);
        Instantiate(novesPeces, Acces_NivellPecesOrigen.nivellPecesOrigen.position, Quaternion.identity);
    }

    void SetExperiencia(int experiencia)
    {
        Debug.Log($"experiencia: {experiencia}");

        uiCercle.fillAmount = nivell.FactorExperienciaNivellActual(nivellMostrat, experiencia);
        uiExperencia.text = $"{experiencia} / {nivell.ExperienciaNecessariaProximNivell(nivellMostrat - 1)}";

        if (!efectes)
            return;

        soExperiencia.Play(0.1f, pitch);
        pitch += 0.1f;

        guanyarExperiencia.Play(pivot);
        
    }

    public void Amagar() => Destroy(this.gameObject);

    IEnumerator AnimacioAccio(float temps, int xpInicial, int xpGuanyada)
    {
        yield return new WaitForSeconds(temps);
        accions[0].Invoke(xpInicial);
        accions.RemoveAt(0);
        prova.RemoveAt(0);
        //SetExperiencia();

        if(accions.Count > 0)
        {
            StartCoroutine(AnimacioAccio(0.5f / (float)xpGuanyada, xpInicial + 1, xpGuanyada));
        }
    }
}
