using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Processador : System.Object
{
    [SerializeField] List<Recepta> receptes;

    bool aconseguit;
    public bool IntentarProcessar(Peça peça, List<object> inputs, bool aLaPrimeraReceptaComplertaAturat = false)
    {
        string _debug = $"Intentar Processar {peça.name} amb {inputs.Count} inputs: ";
        for (int i = 0; i < inputs.Count; i++)
        {
            _debug += $"{inputs[i]}, ";
        }
        Debug.Log(_debug);

        if (receptes == null) receptes = new List<Recepta>();

        aconseguit = false;
        for (int i = 0; i < receptes.Count; i++)
        {
            if(receptes[i].ConnexioPropia != Peça.ConnexioEnum.NoImporta)
            {
                if (!peça.EstatConnexio.HasFlag(receptes[i].ConnexioPropia))
                    continue;
            }

            if (receptes[i].TeInputsIguals(inputs))
            {
                Debug.Log($"Match! ({receptes[i].name})");

                /*
                 * PROBLEMA!!! Destrueix les receptes si es un canvi d'estat.
                 * Osigui, que canvia les receptes perque canvia l'estat
                 * i després borra la recepta...
                 * Aixo va bé per la produccio que no canvia d'estat i per tant no modifica les receptes
                 */
                //receptes.Remove(receptes[i]);

                Recepta _confirmada = receptes[i];

                if (receptes.Count > 0)
                {
                    Debug.Log($"i = {i}");
                    if (i == Mathf.Clamp(i, 0, receptes.Count - 1))
                    {
                        Debug.Log("IN RANGE!");
                        receptes.RemoveAt(i);
                        i--;
                        i = Mathf.Clamp(i, 0, receptes.Count - 1);
                    }
                }

                _confirmada.Processar(peça);
                //receptes[i].Processar(peça);

                aconseguit = true;
                if (aLaPrimeraReceptaComplertaAturat)
                    return aconseguit;
                //aconseguit = true;
                //if (aLaPrimeraReceptaComplertaAturat)
                //    return aconseguit;
            }
        }
        /*for (int i = 0; i < length; i++)
        {

        }*/


        Debug.Log("no match...");
        return aconseguit;
    }







    //public void NovaRecepta(Recepta recepta) => receptes = new List<Recepta>() { recepta };
    public void NovaRecepta(Recepta[] receptes) 
    {
        Debug.Log($"RECEPTES - Nou amb {receptes.Length} receptes");
        this.receptes = new List<Recepta>(receptes);
    } 
    public void AfegirRecepta(Recepta recepta) => receptes.Add(recepta);
    //public void BorrarRecepta(int index) => receptes.RemoveAt(index);
    public void BorrarRecepta(Recepta recepta) 
    {
        Debug.Log($"RECEPTES - Borrar recepta {recepta.name}");
        if (!receptes.Contains(recepta))
            return;

        receptes.Remove(recepta);
    }

}