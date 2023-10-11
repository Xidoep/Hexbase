using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;


[System.Serializable]
public class Casa
{
    public Casa(Pe�a pe�a, Recepta[] receptes)
    {
        this.pe�a = pe�a;
        this.receptes = new List<Recepta>(receptes);

        AgafarNecessitats();
    }

    [SerializeField] Pe�a pe�a;
    [SerializeField] List<Recepta> receptes;
    [SerializeField] List<Producte> proveits;
    [SerializeField] List<Producte> necessitats;

    public Recepta ReceptaActual => receptes[0];
    public List<Producte> Necessitats => necessitats;

    public Processador.Proces Proveir(Producte producte)
    {
        if (!necessitats.Contains(producte))
            return new Processador.Proces(false);

        necessitats.Remove(producte);
        proveits.Add(producte);

        Processador.Proces proces = pe�a.processador.IntentarProcessar(pe�a, new List<object>(proveits));
        if (proces.confirmat)
        //if (pe�a.processador.IntentarProcessar(pe�a, new List<object>(proveits)))
        {
            receptes.RemoveAt(0);
            AgafarNecessitats();
        }
        return proces;
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
        pe�a.processador.AfegirRecepta(receptes[0]);

        
    }


}
