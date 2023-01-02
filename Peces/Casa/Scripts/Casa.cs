using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using XS_Utils;


[System.Serializable]
public class Casa
{
    public Casa(Pe�a pe�a, Producte[] recursosNeeded, System.Action enModificarNecessitats)
    {
        this.pe�a = pe�a;
        Debugar.LogError($"CASA {pe�a.Coordenades}");

        //Crea necessitats per la casa
        necessitats = new Necessitat[0];
        for (int i = 0; i < recursosNeeded.Length; i++)
        {
            AfegirNecessitat(recursosNeeded[i]);
        }

        this.enModificarNecessitats = enModificarNecessitats;
    }
    public Casa(Vector2Int pe�a, Necessitat[] necessitats)
    {
        this.coordenadaPe�a = pe�a;
        this.necessitats = necessitats;
    }


    [SerializeField] Pe�a pe�a;
    [SerializeField] Necessitat[] necessitats;

    System.Action enModificarNecessitats;



    //INTERN
    int index = 0;
    Vector2Int coordenadaPe�a;
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
        tmp.Add(new Necessitat(producte, pe�a));
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

    public SavedCasa Save => pe�a != null ? new SavedCasa(necessitats, pe�a != null ? pe�a.Coordenades : new Vector2Int(-1,-1)) : null;
    public void LoadLastStep(Grid grid)
    {
        Debug.LogError(coordenadaPe�a);

        if (coordenadaPe�a != null)
            pe�a = (Pe�a)grid.Get(coordenadaPe�a);
    }









    [System.Serializable]
    public class Necessitat : System.Object
    {
        public Necessitat(Producte recurs, Pe�a pe�a)
        {
            this.producte = recurs;
            this.pe�a = pe�a;
        }

        [SerializeField] Producte producte;
        [SerializeField] Pe�a pe�a;
        [SerializeField] bool proveit;
        [SerializeField] Informacio.Unitat informacio;

        public Pe�a Pe�a => pe�a;
        public bool Proveit => proveit;
        public Producte Producte => producte;
        public Informacio.Unitat Informacio { get => informacio; set => informacio = value; }

        public void Proveir() => proveit = true;
    }
}
