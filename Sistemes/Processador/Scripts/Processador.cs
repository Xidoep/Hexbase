using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Codice.Client.Common.Servers.RecentlyUsedServers;

public class Processador : MonoBehaviour
{
    [SerializeField] List<ReceptaPreparada> receptes;

    //Ara mes o menys he agregat les receptes a tot arreu on fan falta, crec que només falta a l'extractor...

    //falta quadrar la informacio
    //perque la idea era: [input1][input2][...] => [output]
    //en el cas de les condcions [subestatX][subestatY] => [subestatZ]
    //com faig perque reconegui [producteX] => [X punts] ???
    //o en el cas de la produccio [subestatProductor] => [producte1][producte2][...]
    //al final tots els inputs i outputs seran ScriptableObjects que es tractaran com objectes i despres com la classe que son.
    //aixi que necessiteria una funcio generica que es digues GetIcone que em retornes la icone per mostrar a la info.

    //apunt: aixo està bé perque així pots posar com "potenciadors", que fan que un producte generi mes produccio.

    //per ultim, el que caldia fer seria...
    //Comprovar i activar les receptes i introduir el sistema dins la fase de processar... o bueno, ja descobriré on introduir-ho.
    //bona nit!





    public void IntentarProcessar(ScriptableObject[] inputs)
    {
        if (receptes == null) receptes = new List<ReceptaPreparada>();

        for (int i = 0; i < receptes.Count; i++) 
        {
            if (receptes[i].IngredientsNecessaris(inputs))
            {
                receptes[i].Processar();
                receptes.RemoveAt(i);
            }
        }
    }

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
        public void Processar() 
        {
            recepta.Processar(enProcessar);
            processada = true;
        }
    }

}

[CreateAssetMenu(menuName = "Xido Studio/Hex/Receptes/Recepta")]
[System.Serializable]
public class Recepta : ScriptableObject
{
    [SerializeField] ScriptableObject[] inputs;
    [SerializeField] ScriptableObject output;



    public bool TeInputsIguals(ScriptableObject[] inputs) => inputs == this.inputs;
    public void Processar(System.Action<object> enProcessar) => enProcessar?.Invoke(output);
}
