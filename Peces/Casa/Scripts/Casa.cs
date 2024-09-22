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
    public Casa(Peça peça, string[] necessitats, string[] receptes)
    {
        this.peça = peça;
        this.necessitats = new List<Producte>();
        for (int i = 0; i < necessitats.Length; i++)
        {
            this.necessitats.Add(Referencies.Instance.GetProducte(necessitats[i]));
        }

        this.receptes = new List<Recepta>();
        for (int i = 0; i < receptes.Length; i++)
        {
            this.receptes.Add(Referencies.Instance.GetRecepta(receptes[i]));
        }
    }

    [SerializeField] Peça peça;
    [SerializeField] List<Recepta> receptes;
    [SerializeField] List<Producte> proveits;
    [SerializeField] List<Producte> necessitats;

    public List<Recepta> Receptes => receptes;
    public Recepta ReceptaActual => receptes[0];
    public List<Producte> Necessitats => necessitats;

    public Processador.Proces Proveir(Producte producte)
    {
        if (!necessitats.Contains(producte))
            return new Processador.Proces(false);

        necessitats.Remove(producte);
        proveits.Add(producte);

        Processador.Proces proces = peça.processador.IntentarProcessar(peça, new List<object>(proveits));
        if (proces.confirmat)
        //if (peça.processador.IntentarProcessar(peça, new List<object>(proveits)))
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
        peça.processador.AfegirRecepta(receptes[0]);

        
    }


}
