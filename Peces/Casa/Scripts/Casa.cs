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

    public bool Proveir(Producte producte)
    {
        if (!necessitats.Contains(producte))
            return false;

        necessitats.Remove(producte);
        proveits.Add(producte);

        if (peça.processador.IntentarProcessar(peça, new List<object>(proveits)))
        {
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

        receptes.RemoveAt(0);
    }


























    Necessitat_old[] necessitats_old;
    public Necessitat_old[] Necessitats => necessitats_old;




    [System.Serializable]
    public class Necessitat_old : System.Object
    {
        public Necessitat_old(Producte producte, bool proveit = false)
        {
            this.producte = producte;
            this.proveit = proveit;
        }

        [SerializeField] Producte producte;
        [SerializeField] bool proveit;
        [SerializeField] Informacio.Unitat informacio;

        public bool Proveit => proveit;
        public Producte Producte => producte;
        public Informacio.Unitat Informacio { get => informacio; set => informacio = value; }

        public void Proveir() => proveit = true;
    }
}
