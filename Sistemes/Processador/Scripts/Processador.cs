using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Processador : System.Object
{
    [SerializeField] List<Recepta> receptes;

    bool aconseguit;
    Recepta confirmada;

    public bool IntentarProcessar(Peça peça, List<object> inputs, bool aLaPrimeraReceptaComplertaAturat = false)
    {
        if (receptes == null) receptes = new List<Recepta>();

        aconseguit = false;
        for (int i = 0; i < receptes.Count; i++)
        {
            if (receptes[i].TeInputsIguals(peça, inputs))
            {
                Debug.Log($"Match! ({receptes[i].name})");
                
                confirmada = receptes[i];

                i = BorrarRecepta(i);

                confirmada.Processar(peça);

                aconseguit = true;
                if (aLaPrimeraReceptaComplertaAturat)
                    return aconseguit;
            }
        }

        Debug.Log("no match...");
        return aconseguit;
    }

    

    int BorrarRecepta(int i)
    {
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

        return i;
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








    void Debugar(Peça peça, List<object> inputs)
    {
        string _debug = $"Intentar Processar {peça.name} amb {inputs.Count} inputs: ";
        for (int i = 0; i < inputs.Count; i++)
        {
            _debug += $"{inputs[i]}, ";
        }
        Debug.Log(_debug);
    }
}