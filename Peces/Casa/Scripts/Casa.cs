using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using XS_Utils;


[System.Serializable]
public class Casa
{
    public Casa(Peça peça, Producte[] recursosNeeded, System.Action enModificarNecessitats)
    {
        this.peça = peça;
        Debugar.LogError($"CASA {peça.Coordenades}");

        //Crea necessitats per la casa
        necessitats = new Necessitat[0];
        for (int i = 0; i < recursosNeeded.Length; i++)
        {
            AfegirNecessitat(recursosNeeded[i]);
        }

        this.enModificarNecessitats = enModificarNecessitats;
    }
    public Casa(Vector2Int peça, Necessitat[] necessitats)
    {
        this.coordenadaPeça = peça;
        this.necessitats = necessitats;
    }


    [SerializeField] Peça peça;
    [SerializeField] Necessitat[] necessitats;

    System.Action enModificarNecessitats;



    //INTERN
    int index = 0;
    Vector2Int coordenadaPeça;
    bool proveit;

    public bool Proveit
    {
        get
        {
            proveit = true;
            for (int i = 0; i < necessitats.Length; i++)
            {
                if(!necessitats[i].Proveit)
                {
                    proveit = false;
                    break;
                }
            }
            return proveit;
        }
    }
    public Necessitat[] Necessitats => necessitats;
    public void AfegirNecessitat(Producte producte)
    {
        List<Necessitat> tmp = new List<Necessitat>(necessitats);
        tmp.Add(new Necessitat(producte, peça));
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

    public SavedCasa Save => peça != null ? new SavedCasa(necessitats, peça != null ? peça.Coordenades : new Vector2Int(-1,-1)) : null;
    public void LoadLastStep(Grid grid)
    {
        Debug.LogError(coordenadaPeça);

        if (coordenadaPeça != null)
            peça = (Peça)grid.Get(coordenadaPeça);
    }









    [System.Serializable]
    public class Necessitat : System.Object
    {
        public Necessitat(Producte recurs, Peça peça)
        {
            this.producte = recurs;
            this.peça = peça;
        }

        [SerializeField] Producte producte;
        [SerializeField] Peça peça;
        [SerializeField] bool proveit;
        [SerializeField] Informacio.Unitat informacio;

        public Peça Peça => peça;
        public bool Proveit => proveit;
        public Producte Producte => producte;
        public Informacio.Unitat Informacio { get => informacio; set => informacio = value; }

        public void Proveir() => proveit = true;
    }
}
