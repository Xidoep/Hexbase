using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;
using Sirenix.OdinInspector;
//using static Casa;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Processos/Repoblar")]
public class Repoblar : ScriptableObject
{
    [SerializeField] Recepta[] necessitats;

    [ReadOnly] List<Pe�a> cases;

    //INTERN
    List<Pe�a> veins;
    int casesVeines = 0;


    void OnEnable()
    {
        cases = new List<Pe�a>();
    }

    public void AfegirLaPrimeraCasa(Pe�a pe�a) => pe�a.AfegirCasa(necessitats);
    public void Proces(List<Pe�a> peces, System.Action enFinalitzar)
    {
        Debugar.LogError("--------------REPOBLAR---------------");
        List<int> cases = new List<int>();
        for (int p = 0; p < peces.Count; p++)
        {
            if (peces[p].Subestat.Tipus != Estat.TipusEnum.Casa)
                continue;

            casesVeines = 0;
            veins = peces[p].VeinsPe�a;

            //Suma les cases que hi ha al voltant.
            for (int v = 0; v < veins.Count; v++)
            {
                if (veins[v].Subestat.Tipus == Estat.TipusEnum.Casa) casesVeines++;
            }

            CanviarCases(peces[p], 1 + casesVeines);
            //CanviarNecessitats(peces[p], 1 + casesVeines);
        }

        if (enFinalitzar != null) enFinalitzar.Invoke();
    }


    void CanviarCases(Pe�a pe�a, int casesVeines)
    {
        if (casesVeines > pe�a.CasesLength)
        {
            //pe�a.AmagarInformacio?.Invoke(pe�a);
            for (int i = pe�a.CasesLength; i < casesVeines; i++)
            {
                pe�a.AfegirCasa(necessitats);
                Debug.Log("Add casa");
            }
        }
        else if (casesVeines < pe�a.CasesLength)
        {
            //pe�a.AmagarInformacio?.Invoke(pe�a);
            for (int i = casesVeines; i < pe�a.CasesLength; i++)
            {
                pe�a.TreureCasa();
                Debug.Log("Remove casa");
            }
        }

        if (!this.cases.Contains(pe�a)) this.cases.Add(pe�a);
    }
}


