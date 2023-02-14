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

    [SerializeField] Necessitat[] necessitats;

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
