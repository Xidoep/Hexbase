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
    public Casa(Peça peça, Recepta[] necessitats) 
    {
        need = new Need(peça, necessitats);
        //necessitatsss = necessitats;
        //peça.processador.AfegirRecepta(necessitatsss[0]);
        //peça.processador.AfegirRecepta(receptes[0], SeguentNecessitat);
    }


    [SerializeField] public Need need;
    [SerializeField] Necessitat[] necessitats;
    [SerializeField] Recepta[] necessitatsss; //canviar nom per necessitats

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

    

    /*FALTA:
     * intentar accedir a la funcino Proveir d'aquest classe en contes de la que te de base.
     */
    [System.Serializable]
    public struct Need
    {
        //Necessito aquest classe per poder acumular producte
        //pero quests productes es poden enviar al processador de la peça.
        public Need(Peça peça, Recepta[] necessitats)
        {
            this.peça = peça;
            productes = new List<object>();
            this.necessitats = new List<Recepta>(necessitats);
            peça.processador.AfegirRecepta(necessitats[0], DonarNecessitat);
        }

        [SerializeField] Peça peça;
        [SerializeField] List<object> productes;

        [SerializeField] List<Recepta> necessitats;

        public void Proveir(Producte producte)
        {
            productes.Add(producte);
            peça.processador.IntentarProcessar(peça, productes);
            /*
             * En principi e posat un callback a la recepte perque em digui quan es compleixi.
             * Aix de pensar que passa si donu un producte a una peça que no el necessita, a vera com el tornem o directament no l'accepta.
             * El que hauriem de fer es comprovar la recepta i quan arriba un producte, comprovar els inputs a veure si cohincideixen.
             * la funcio IntentarPorcessar retorna true o false, si es false, put intentar algo...
             * pero seria molt millor confirmar que els producte cohicideixen abans d'enviarlos.
             */
        }

        void DonarNecessitat(Peça peça)
        {
            if (necessitats.Count == 0)
                return;

            peça.processador.AfegirRecepta(necessitats[0], DonarNecessitat);
            necessitats.RemoveAt(0);
        }
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
