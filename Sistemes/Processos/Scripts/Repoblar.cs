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
            if (!peces[p].SubestatIgualA(casa))
                continue;

            casesVeines = 0;
            veins = peces[p].VeinsPe�a;

            //Suma les cases que hi ha al voltant.
            for (int v = 0; v < veins.Count; v++)
            {
                if (veins[v].SubestatIgualA(casa)) casesVeines++;
            }

            CanviarCases(peces[p], 1 + casesVeines);
            //CanviarNecessitats(peces[p], 1 + casesVeines);
        }

        if (enFinalitzar != null) enFinalitzar.Invoke();
    }
    
    /*void CanviarNecessitats(Pe�a pe�a, int necessitats)
    {
        //AIXO HA DE FUNCIONAR COMPLETAMENT DIFERENT.
        //1.- Ha de mantenir que hi ha tant habitants com cases al voltant + ella mateixa.
        //2.- Quan una casa "apareix". Se l'hi afageixen totes les necessitats que pot tenir.
        //3.- Et demana la primera mentre estigui coverta. Aixo no se si s'ha de gestionar aqui



        //Si falten necessitats, n'agafeix.
        if (necessitats > pe�a.Casa.Necessitats.Length)
        {
            pe�a.AmagarInformacio?.Invoke(pe�a);
            for (int i = pe�a.Casa.Necessitats.Length; i < necessitats; i++)
            {
                pe�a.Casa.AfegirNecessitat(necessitatInicial);
            }
        }
        //Si sobren necessitats, en treu.
        else if(necessitats < pe�a.Casa.Necessitats.Length)
        {
            pe�a.AmagarInformacio?.Invoke(pe�a);
            for (int i = necessitats; i < pe�a.Casa.Necessitats.Length; i++)
            {
                pe�a.Casa.TreureNecessitat();
            }
        }

        if (!this.cases.Contains(pe�a)) this.cases.Add(pe�a);
    }
    */
    
    public void AfegirLaPrimeraCasa(Pe�a pe�a) => pe�a.AfegirCasa(necessitatInicial);

    void CanviarCases(Pe�a pe�a, int cases)
    {
        if (cases > pe�a.CasesLength)
        {
            pe�a.AmagarInformacio?.Invoke(pe�a);
            for (int i = pe�a.CasesLength; i < cases; i++)
            {
                pe�a.AfegirCasa(necessitatInicial);
                //pe�a.Casa.AfegirNecessitat(necessitatInicial);
            }
        }
        //Si sobren necessitats, en treu.
        else if (cases < pe�a.CasesLength)
        {
            pe�a.AmagarInformacio?.Invoke(pe�a);
            for (int i = cases; i < pe�a.CasesLength; i++)
            {
                pe�a.TreureCasa();
                //pe�a.Casa.TreureNecessitat();
            }
        }

        if (!this.cases.Contains(pe�a)) this.cases.Add(pe�a);
    }
}


