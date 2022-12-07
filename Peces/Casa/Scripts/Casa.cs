using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using XS_Utils;


[System.Serializable]
public class Casa
{
    public Casa(Peça peça, Producte[] recursosNeeded)
    {
        this.peça = peça;
        Debugar.LogError($"CASA {peça.Coordenades}");

        //Crea necessitats per la casa
        List<Necessitat> _tmpNeeds = new List<Necessitat>();
        for (int r = 0; r < recursosNeeded.Length; r++)
        {
            _tmpNeeds.Add(new Necessitat(recursosNeeded[r], peça));
        }
        this.necessitats = _tmpNeeds.ToArray();
    }
    public Casa(Vector2Int peça, Necessitat[] necessitats)
    {
        this.coordenadaPeça = peça;
        this.necessitats = necessitats;
    }


    [SerializeField] Peça peça;
    [SerializeField] Necessitat[] necessitats;


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


        public Peça Peça => peça;
        public bool Proveit => proveit;
        public Producte Producte => producte;

        public void Proveir() 
        {
            proveit = true;
            Debugar.LogError("+1 PUNT!");
        } 
    }
}
