using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Processador : System.Object
{
    [SerializeField] List<Recepta> receptes;


    public bool IntentarProcessar(Peça peça, List<object> inputs, bool aLaPrimeraReceptaComplertaAturat = false)
    {
        string _debug = $"Intentar Processar {peça.name} amb {inputs.Count} inputs: ";
        for (int i = 0; i < inputs.Count; i++)
        {
            _debug += $"{inputs[i]}, ";
        }
        Debug.Log(_debug);

        if (receptes == null) receptes = new List<Recepta>();

        bool aconseguit = false;
        for (int i = 0; i < receptes.Count; i++)
        {
            if(receptes[i].ConnexioPropia != Peça.ConnexioEnum.NoImporta)
            {
                if (!peça.EstatConnexio.HasFlag(receptes[i].ConnexioPropia))
                    continue;
            }

            if (receptes[i].TeInputsIguals(inputs))
            {
                Debug.Log("Match!");
                receptes[i].Processar(peça);
                if(receptes.Count > 0)
                {
                    receptes.RemoveAt(i);
                    i--;
                }
                aconseguit = true;

                if (aLaPrimeraReceptaComplertaAturat)
                    return aconseguit;
            }
        }
        Debug.Log("no match...");
        return aconseguit;
    }







    public void NovaRecepta(Recepta recepta) => receptes = new List<Recepta>() { recepta };
    public void NovaRecepta(Recepta[] receptes) => this.receptes = new List<Recepta>(receptes);
    public void AfegirRecepta(Recepta recepta) => receptes.Add(recepta);
    public void BorrarRecepta(int index) => receptes.RemoveAt(index);
    public void BorrarRecepta(Recepta recepta) 
    {
        if (!receptes.Contains(recepta))
            return;

        receptes.Remove(recepta);
    }

}