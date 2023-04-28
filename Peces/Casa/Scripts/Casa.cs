using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;


[System.Serializable]
public class Casa
{
    public Casa(Peça peça, Recepta[] receptes)
    {
        this.peça = peça;
        this.receptes = new List<Recepta>(receptes);

        AgafarNecessitats();
    }

    [SerializeField] Peça peça;
    [SerializeField] List<Recepta> receptes;
    [SerializeField] List<Producte> proveits;
    [SerializeField] List<Producte> necessitats;

    public Recepta ReceptaActual => receptes[0];

    public bool Proveir(Producte producte)
    {
        if (!necessitats.Contains(producte))
            return false;

        necessitats.Remove(producte);
        proveits.Add(producte);

        if (peça.processador.IntentarProcessar(peça, new List<object>(proveits)))
        {
            receptes.RemoveAt(0);
            AgafarNecessitats();
        }
        return true;
    }

    void AgafarNecessitats()
    {
        proveits = new List<Producte>();

        if (receptes.Count == 0)
            return;

        necessitats = new List<Producte>();
        for (int i = 0; i < receptes[0].Inputs.Length; i++)
        {
            necessitats.Add((Producte)receptes[0].Inputs[i]);
        }
        peça.processador.AfegirRecepta(receptes[0]);

        
    }


}
