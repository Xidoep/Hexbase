using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Casa
{
    /*public Casa(Pe�a pe�a, int habitants)
    {
        if (this.habitants == null) this.habitants = new List<Habitant>();

        for (int i = 0; i < habitants; i++)
        {
            this.habitants.Add(new Habitant(pe�a));
        }
    }*/
    public Casa(Pe�a pe�a, Producte[] recursosNeeded)
    {
        this.pe�a = pe�a;

        List<Necessitat> _tmpNeeds = new List<Necessitat>();
        for (int r = 0; r < recursosNeeded.Length; r++)
        {
            index = -1;
            for (int n = 0; n < _tmpNeeds.Count; n++)
            {
                if (recursosNeeded[r] == _tmpNeeds[n].Recurs)
                {
                    index = n;
                    break;
                }
            }

            if (index == -1)
            {
                _tmpNeeds.Add(new Necessitat(recursosNeeded[r]));
            }
            else
            {
                _tmpNeeds[index].AddNecessitat();
            }
        }
        this.necessitats = _tmpNeeds.ToArray();

        nivell = 1;
    }
    public Casa(Vector2Int pe�a, Vector2Int feina, int nivell, Necessitat[] necessitats)
    {
        this.coordenadaPe�a = pe�a;
        this.coordenadaProducte = feina;
        this.nivell = nivell;
        this.necessitats = necessitats;
    }


    [SerializeField] Pe�a pe�a;
    [SerializeField] Pe�a producte;
    [SerializeField] int nivell;
    //[SerializeField] Condicio_GuanyarRecurs condicio;
    [SerializeField] Necessitat[] necessitats;


    //INTERN
    int index = 0;





    /*[Linia]
    [Header("RECURSOS")]
    [SerializeField] Recurs[] recursos;*/
    //INTERN
    //int index = -1;
    Vector2Int coordenadaPe�a;
    Vector2Int coordenadaProducte;

    public bool Disponible => producte == null;
    //public int Grup => pe�a.Grup;
    public SavedCasa Save => pe�a != null ? new SavedCasa(necessitats, nivell, pe�a != null ? pe�a.Coordenades : new Vector2Int(-1,-1), producte != null ? producte.Coordenades : new Vector2Int(-1, -1)) : null;
    public void LoadLastStep(Grid grid)
    {
        Debug.LogError(coordenadaPe�a);
        Debug.LogError(coordenadaProducte);

        if (coordenadaPe�a != null)
            pe�a = (Pe�a)grid.Get(coordenadaPe�a);

        if (coordenadaProducte != -Vector2Int.one)
            producte = (Pe�a)grid.Get(coordenadaProducte);

        //Ocupar(producte);
    }



    /*public void Ocupar(Pe�a feina)
    {
        this.producte = feina;
        feina.AddTreballador(this);
    }*/
    /*public void Desocupar()
    {
        producte.RemoveTreballador();
    }*/



    /// <summary>
    /// Aporta un recurs a les necessitats de la casa.
    /// </summary>
    /// <returns>Retorna si el recurs s'ha pogut entregar o no, si es que no, s'ha d'entregar a una altre casa.</returns>
    public bool Proveir(Producte recurs, System.Action onPujarNivell)
    {
        bool proveit = false;
        for (int i = 0; i < necessitats.Length; i++)
        {
            if (!necessitats[i].Recurs.Equals(recurs))
                continue;

            if(!necessitats[i].Complet)
            {
                proveit = true;
                if (necessitats[i].Proveir())
                {
                    PujarNivell();
                    onPujarNivell.Invoke();
                    //**********************************
                    //Pos un callback
                    //**********************************
                }
                break;
            }
        }
        return proveit;
    }

    void PujarNivell()
    {
        nivell++;
        Debug.LogError($"DONAR 1 PUNT!");
    }

    public void Clean()
    {
        for (int i = 0; i < necessitats.Length; i++)
        {
            necessitats[i].Clean();
        }
    }





    [System.Serializable]
    public class Necessitat : System.Object
    {
        public Necessitat(Producte recurs)
        {
            this.recurs = recurs;
            //proveits = new List<Recurs>();
            //AddNecessitat();
            proveits = new Producte[1];
        }

        [SerializeField] Producte recurs;
        [SerializeField] Producte[] proveits;
        [SerializeField] bool solventat = false;


        //INTERN
        bool complet = false;

        public Producte Recurs => recurs;
        public bool Complet
        {
            get
            {
                complet = true;
                for (int i = 0; i < proveits.Length; i++)
                {
                    if (proveits[i] == null)
                    {
                        complet = false;
                        break;
                    }
                }
                return complet;
            }
        }

        /// <summary>
        /// Utilitzat nom�s a la iniciaci� de la casa.
        /// </summary>
        public void AddNecessitat() 
        {
            List<Producte> tmp = new List<Producte>(proveits);
            tmp.Add(null);
            proveits = tmp.ToArray();
        } 

        /// <summary>
        /// Dona un recurs per cubrir la necessitat.
        /// </summary>
        /// <returns>Retorna si la necessitat est� complerta, i per tant s'ha de pujar un nivell.</returns>
        public bool Proveir()
        {
            //Donar recurs
            for (int i = 0; i < proveits.Length; i++)
            {
                if(proveits[i] == null)
                {
                    proveits[i] = recurs;
                    break;
                }
            }

            //Si solventat no toninua
            if (solventat)
                return false;

            //Comprovar complet
            if (Complet)
            {
                //Pujar nivell
                solventat = true;
                return true;
            }
            else return false;

        }

        public void Clean()
        {
            for (int i = 0; i < proveits.Length; i++)
            {
                proveits[i] = null;
            }
        }
    }
}
