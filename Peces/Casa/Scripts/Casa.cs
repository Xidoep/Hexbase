using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;


[System.Serializable]
public class Casa
{
    public Casa(Producte[] recursosNeeded, System.Action enModificarNecessitats)
    {
        necessitats = new Necessitat[0];
        for (int i = 0; i < recursosNeeded.Length; i++)
        {
            AfegirNecessitat(recursosNeeded[i]);
        }
        this.enModificarNecessitats = enModificarNecessitats;
    }
    public Casa(Necessitat[] necessitats)
    {
        this.necessitats = necessitats;
    }
    public Casa(Peça peça, Recepta[] totesLesNecessitats) 
    {
        receptes = new List<Recepta>(totesLesNecessitats);
        //peça.processador.AfegirRecepta(receptes[0], SeguentNecessitat);
    }




    [SerializeField] Necessitat[] necessitats;
    [SerializeField] List<Recepta> receptes; //canviar nom per necessitats

    System.Action enModificarNecessitats;


    public Necessitat[] Necessitats => necessitats;
    public void AfegirNecessitat(Producte producte)
    {
        List<Necessitat> tmp = new List<Necessitat>(necessitats);
        tmp.Add(new Necessitat(producte));
        necessitats = tmp.ToArray();

        enModificarNecessitats?.Invoke();
    }
    public void TreureNecessitat()
    {
        List<Necessitat> tmp = new List<Necessitat>(necessitats);
        tmp.RemoveAt(tmp.Count - 1);
        necessitats = tmp.ToArray();

        enModificarNecessitats?.Invoke();
    }

    public void Proveir()
    {
        necessitats[0].Proveir();
        //Falta que aquesta necessitat ara desaparegui i mostri la seguent.
    }

    






    [System.Serializable]
    public class Necessitat : System.Object
    {
        public Necessitat(Producte producte, bool proveit = false)
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
