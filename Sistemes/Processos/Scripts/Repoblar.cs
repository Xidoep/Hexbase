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

    [Nota("Només per debugging",NoteType.Warning)]
    [SerializeField] List<Peça> cases;

    //INTERN
    List<Peça> veins;
    int casesVeines = 0;


    void OnEnable()
    {
        cases = new List<Peça>();
    }

    public void Proces(List<Peça> peces, System.Action enFinalitzar)
    {
        Debugar.LogError("--------------REPOBLAR---------------");
        List<int> cases = new List<int>();
        for (int p = 0; p < peces.Count; p++)
        {
            if (peces[p].Subestat.Tipus != Subestat.TipusEnum.Casa)
                continue;

            casesVeines = 0;
            veins = peces[p].VeinsPeça;

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

    public void AfegirLaPrimeraCasa(Peça peça) => peça.AfegirCasa(necessitats);

    void CanviarCases(Peça peça, int cases)
    {
        if (cases > peça.CasesLength)
        {
            //peça.AmagarInformacio?.Invoke(peça);
            for (int i = peça.CasesLength; i < cases; i++)
            {
                peça.AfegirCasa(necessitats);
                Debug.Log("Add casa");
            }
        }
        else if (cases < peça.CasesLength)
        {
            //peça.AmagarInformacio?.Invoke(peça);
            for (int i = cases; i < peça.CasesLength; i++)
            {
                peça.TreureCasa();
                Debug.Log("Remove casa");
            }
        }

        if (!this.cases.Contains(peça)) this.cases.Add(peça);
    }
}


