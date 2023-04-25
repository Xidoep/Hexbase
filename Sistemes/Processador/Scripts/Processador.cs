using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Codice.Client.Common.Servers.RecentlyUsedServers;

public class Processador : MonoBehaviour
{
    [SerializeField] List<ReceptaPreparada> receptes;


    //Molt be! hem posat la part mes complicada.
    //El seguent pass es que la produccio agafi els productes de... a vera que ho porovo HA! funciona!!!
    //nomes falta treure les referencies a ocupat i merdes aixi.
    //Doncs el seguent pas és... Tenim canvi d'estat, tenim produccio...
    //falta la satisfaccio de les cases.
    //





    public bool IntentarProcessar(Peça peça, List<object> inputs)
    {
        string _debug = $"Intentar Processar {peça.name} amb {inputs.Count} inputs: ";
        for (int i = 0; i < inputs.Count; i++)
        {
            _debug += $"{inputs[i]}, ";
        }
        Debug.Log(_debug);
        //intenta processar totes les receptes que te??? aixo no pot ser,
        //huria de ferne una i si aquesta es compleix para. Sino, pot portar problemes.
        //ja que algunes canvies l'estat i borren les receptes existents.
        if (receptes == null) receptes = new List<ReceptaPreparada>();

        for (int i = 0; i < receptes.Count; i++)
        {
            if (receptes[i].IngredientsNecessaris(inputs))
            {
                Debug.Log("Match!");
                receptes[i].Processar(peça, EsborrarRecepta);
                return true;
            }
        }
        Debug.Log("no match...");
        return false;
    }

    void EsborrarRecepta(ReceptaPreparada recepta) => receptes.Remove(recepta);

    public void NovesReceptes(Recepta[] receptes)
    {
        this.receptes = new List<ReceptaPreparada>();

        for (int i = 0; i < receptes.Length; i++)
        {
            this.receptes.Add(new ReceptaPreparada(receptes[i]));
        }
    }
    public void AfegirRecepta(Recepta recepta)
    {
        if(receptes == null) receptes = new List<ReceptaPreparada>();

        receptes.Add(new ReceptaPreparada(recepta));
    }

    public void BorrarReceptes() 
    {
        if (receptes == null)
            return;

        receptes.Clear();
    } 



    [System.Serializable]
    public struct ReceptaPreparada
    {
        public ReceptaPreparada(Recepta recepta)
        {
            this.recepta = recepta;
        }

        [SerializeScriptableObject][SerializeField] Recepta recepta;

        public bool IngredientsNecessaris(List<object> inputs) => recepta.TeInputsIguals(inputs);

        public void Processar(Peça peça, System.Action<ReceptaPreparada> borrarDeLaLLista)
        {
            recepta.Processar(peça);
            borrarDeLaLLista.Invoke(this);
        }
    }

}