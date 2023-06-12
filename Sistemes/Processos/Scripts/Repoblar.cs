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

    [ReadOnly] List<Peça> cases;

    //INTERN
    List<Peça> veins;
    int casesVeines = 0;


    void OnEnable()
    {
        cases = new List<Peça>();
    }

    public void AfegirLaPrimeraCasa(Peça peça) => peça.AfegirCasa(necessitats);
    public void Proces(List<Peça> peces, System.Action enFinalitzar)
    {
        Debugar.LogError("--------------REPOBLAR---------------");
        List<int> cases = new List<int>();
        for (int p = 0; p < peces.Count; p++)
        {
            if (peces[p].Subestat.Tipus != Estat.TipusEnum.Casa)
                continue;

            casesVeines = 0;
            veins = peces[p].VeinsPeça;

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


    void CanviarCases(Peça peça, int casesVeines)
    {
        if (casesVeines > peça.CasesLength)
        {
            //peça.AmagarInformacio?.Invoke(peça);
            for (int i = peça.CasesLength; i < casesVeines; i++)
            {
                peça.AfegirCasa(necessitats);
                Debug.Log("Add casa");
            }
        }
        else if (casesVeines < peça.CasesLength)
        {
            //peça.AmagarInformacio?.Invoke(peça);
            for (int i = casesVeines; i < peça.CasesLength; i++)
            {
                peça.TreureCasa();
                Debug.Log("Remove casa");
            }
        }

        if (!this.cases.Contains(peça)) this.cases.Add(peça);
    }
}


