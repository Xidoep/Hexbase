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
    //Doncs el seguent pas �s... Tenim canvi d'estat, tenim produccio...
    //falta la satisfaccio de les cases.






    public bool IntentarProcessar(Pe�a pe�a, List<object> inputs)
    {
        string _debug = $"Intentar Processar {pe�a.name} amb {inputs.Count} inputs: ";
        for (int i = 0; i < inputs.Count; i++)
        {
            _debug += $"{inputs[i]}, ";
        }
        Debug.Log(_debug);

        if (receptes == null) receptes = new List<ReceptaPreparada>();

        bool aconseguit = false;
        for (int i = 0; i < receptes.Count; i++)
        {
            if (receptes[i].IngredientsNecessaris(inputs))
            {
                Debug.Log("Match!");
                receptes[i].Processar(pe�a, EsborrarRecepta);
                aconseguit = true;
            }
        }
        Debug.Log("no match...");
        return aconseguit;
    }

    void EsborrarRecepta(ReceptaPreparada recepta) => receptes.Remove(recepta);

    public void NovesReceptes(Recepta[] receptes)
    {
        this.receptes = new List<ReceptaPreparada>();

        for (int i = 0; i < receptes.Length; i++)
        {
            this.receptes.Add(new ReceptaPreparada(receptes[i], null));
        }
    }
    public void AfegirRecepta(Recepta recepta, System.Action<Pe�a> onProcessar = null)
    {
        if(receptes == null) receptes = new List<ReceptaPreparada>();

        receptes.Add(new ReceptaPreparada(recepta, onProcessar));
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
        public ReceptaPreparada(Recepta recepta, System.Action<Pe�a> onProcessar)
        {
            this.recepta = recepta;
            this.onProcessar = onProcessar;
        }

        [SerializeScriptableObject][SerializeField] Recepta recepta;
        [SerializeField] System.Action<Pe�a> onProcessar;

        public bool IngredientsNecessaris(List<object> inputs) => recepta.TeInputsIguals(inputs);

        public void Processar(Pe�a pe�a, System.Action<ReceptaPreparada> borrarDeLaLLista)
        {
            recepta.Processar(pe�a);
            onProcessar?.Invoke(pe�a);
            borrarDeLaLLista.Invoke(this);
        }
    }

}