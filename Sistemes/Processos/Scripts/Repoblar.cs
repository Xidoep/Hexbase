using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;
using static Casa;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Processos/Repoblar")]
public class Repoblar : ScriptableObject
{
    [SerializeField] Subestat casa;
    [SerializeField] Detall_Tiles_Estat detall_Tiles;
    [SerializeField] Producte necessitatInicial;
    [SerializeField] Recepta[] necessitats;

    [Nota("Nom�s per debugging",NoteType.Warning)]
    [SerializeField] List<Pe�a> cases;

    //INTERN
    List<Pe�a> veins;
    int casesVeines = 0;


    void OnEnable()
    {
        cases = new List<Pe�a>();
    }

    public void Proces(List<Pe�a> peces, System.Action enFinalitzar)
    {
        Debugar.LogError("--------------REPOBLAR---------------");
        List<int> cases = new List<int>();
        for (int p = 0; p < peces.Count; p++)
        {
            if (peces[p].Subestat.Tipus != Subestat.TipusEnum.Casa)
                continue;

            casesVeines = 0;
            veins = peces[p].VeinsPe�a;

            //Suma les cases que hi ha al voltant.
            for (int v = 0; v < veins.Count; v++)
            {
                if (veins[v].Subestat.Tipus == Subestat.TipusEnum.Casa) casesVeines++;
            }

            CanviarCases(peces[p], 1 + casesVeines);
            //CanviarNecessitats(peces[p], 1 + casesVeines);
        }

        if (enFinalitzar != null) enFinalitzar.Invoke();
    }

    public void AfegirLaPrimeraCasa(Pe�a pe�a) => pe�a.AfegirCasa(necessitats);

    void CanviarCases(Pe�a pe�a, int cases)
    {
        if (cases > pe�a.CasesLength)
        {
            //pe�a.AmagarInformacio?.Invoke(pe�a);
            for (int i = pe�a.CasesLength; i < cases; i++)
            {
                pe�a.AfegirCasa(necessitats);
                Debug.Log("Add casa");
            }
        }
        else if (cases < pe�a.CasesLength)
        {
            //pe�a.AmagarInformacio?.Invoke(pe�a);
            for (int i = cases; i < pe�a.CasesLength; i++)
            {
                pe�a.TreureCasa();
                Debug.Log("Remove casa");
            }
        }

        if (!this.cases.Contains(pe�a)) this.cases.Add(pe�a);
    }
}


