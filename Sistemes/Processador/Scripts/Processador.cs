using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Codice.Client.Common.Servers.RecentlyUsedServers;

public class Processador : MonoBehaviour
{
    [SerializeField] List<ReceptaPreparada> receptes;


    //falta quadrar la informacio
    //perque la idea era: [input1][input2][...] => [output]
    //en el cas de les condcions [subestatX][subestatY] => [subestatZ]
    //com faig perque reconegui [producteX] => [X punts] ???
    //o en el cas de la produccio [subestatProductor] => [producte1][producte2][...]
    //al final tots els inputs i outputs seran ScriptableObjects que es tractaran com objectes i despres com la classe que son.
    //aixi que necessiteria una funcio generica que es digues GetIcone que em retornes la icone per mostrar a la info.

    //apunt: aixo est� b� perque aix� pots posar com "potenciadors", que fan que un producte generi mes produccio.

    //per ultim, el que caldia fer seria...
    //Comprovar i activar les receptes i introduir el sistema dins la fase de processar... o bueno, ja descobrir� on introduir-ho.
    //bona nit!





    public void IntentarProcessar(ScriptableObject[] inputs)
    {
        if (receptes == null) receptes = new List<ReceptaPreparada>();

        for (int i = 0; i < receptes.Count; i++) 
        {
            if (receptes[i].IngredientsNecessaris(inputs))
            {
                receptes[i].Processar(EsborrarRecepta);
                receptes.RemoveAt(i);
            }
        }
    }
    public bool IntentarProcessar(List<object> inputs)
    {
        string _debug = $"Intentar Processar amb {inputs.Count} inputs: ";
        for (int i = 0; i < inputs.Count; i++)
        {
            _debug += $"{inputs[i]}, ";
        }
        _debug += $"\n Hi ha {receptes.Count} receptes";
        Debug.Log(_debug);
        //intenta processar totes les receptes que te??? aixo no pot ser,
        //huria de ferne una i si aquesta es compleix para. Sino, pot portar problemes.
        //ja que algunes canvies l'estat i borren les receptes existents.
        if (receptes == null) receptes = new List<ReceptaPreparada>();

        for (int i = 0; i < receptes.Count; i++)
        {
            if (receptes[i].IngredientsNecessaris(inputs))
            {
                receptes[i].Processar(EsborrarRecepta);
                Debug.Log("Match!");
                return true;
            }
        }
        Debug.Log("no match...");
        return false;
    }

    void EsborrarRecepta(ReceptaPreparada recepta) => receptes.Remove(recepta);

    public void NovesReceptes(Recepta[] receptes, System.Action<object> enProcessar)
    {
        this.receptes = new List<ReceptaPreparada>();

        for (int i = 0; i < receptes.Length; i++)
        {
            this.receptes.Add(new ReceptaPreparada(receptes[i], enProcessar));
        }
    }
    public void AfegirRecepta(Recepta recepta, System.Action<object> enProcessar)
    {
        if(receptes == null) receptes = new List<ReceptaPreparada>();

        receptes.Add(new ReceptaPreparada(recepta, enProcessar));
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
        public ReceptaPreparada(Recepta recepta, System.Action<object> enProcessar)
        {
            this.recepta = recepta;
            processada = false;
            this.enProcessar = enProcessar;
        }

        [SerializeField] Recepta recepta;
        [SerializeField] bool processada;
        System.Action<object> enProcessar;
        


        public bool IngredientsNecessaris(ScriptableObject[] inputs) => recepta.TeInputsIguals(inputs);
        public bool IngredientsNecessaris(List<object> inputs) => recepta.TeInputsIguals(inputs);
        public void Processar(System.Action<ReceptaPreparada> borrarDeLaLLista) 
        {
            recepta.Processar(enProcessar);
            borrarDeLaLLista.Invoke(this);
            processada = true;
        }
    }

}