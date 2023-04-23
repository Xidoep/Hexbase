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
            if (!peces[p].SubestatIgualA(casa))
                continue;

            casesVeines = 0;
            veins = peces[p].VeinsPeça;

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
    
    /*void CanviarNecessitats(Peça peça, int necessitats)
    {
        //AIXO HA DE FUNCIONAR COMPLETAMENT DIFERENT.
        //1.- Ha de mantenir que hi ha tant habitants com cases al voltant + ella mateixa.
        //2.- Quan una casa "apareix". Se l'hi afageixen totes les necessitats que pot tenir.
        //3.- Et demana la primera mentre estigui coverta. Aixo no se si s'ha de gestionar aqui



        //Si falten necessitats, n'agafeix.
        if (necessitats > peça.Casa.Necessitats.Length)
        {
            peça.AmagarInformacio?.Invoke(peça);
            for (int i = peça.Casa.Necessitats.Length; i < necessitats; i++)
            {
                peça.Casa.AfegirNecessitat(necessitatInicial);
            }
        }
        //Si sobren necessitats, en treu.
        else if(necessitats < peça.Casa.Necessitats.Length)
        {
            peça.AmagarInformacio?.Invoke(peça);
            for (int i = necessitats; i < peça.Casa.Necessitats.Length; i++)
            {
                peça.Casa.TreureNecessitat();
            }
        }

        if (!this.cases.Contains(peça)) this.cases.Add(peça);
    }
    */
    
    public void AfegirLaPrimeraCasa(Peça peça) => peça.AfegirCasa(necessitatInicial);

    void CanviarCases(Peça peça, int cases)
    {
        if (cases > peça.CasesLength)
        {
            peça.AmagarInformacio?.Invoke(peça);
            for (int i = peça.CasesLength; i < cases; i++)
            {
                peça.AfegirCasa(necessitatInicial);
                //peça.Casa.AfegirNecessitat(necessitatInicial);
            }
        }
        //Si sobren necessitats, en treu.
        else if (cases < peça.CasesLength)
        {
            peça.AmagarInformacio?.Invoke(peça);
            for (int i = cases; i < peça.CasesLength; i++)
            {
                peça.TreureCasa();
                //peça.Casa.TreureNecessitat();
            }
        }

        if (!this.cases.Contains(peça)) this.cases.Add(peça);
    }
}


