using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Casa
{
    public Casa(Pe�a pe�a, Producte[] recursosNeeded)
    {
        this.pe�a = pe�a;

        //Crea necessitats per la casa
        List<Necessitat> _tmpNeeds = new List<Necessitat>();
        for (int r = 0; r < recursosNeeded.Length; r++)
        {
            index = -1;
            for (int n = 0; n < _tmpNeeds.Count; n++)
            {
                if (recursosNeeded[r] == _tmpNeeds[n].Producte)
                {
                    index = n;
                    break;
                }
            }

            if (index == -1)
            {
                _tmpNeeds.Add(new Necessitat(recursosNeeded[r],pe�a));
            }
            else
            {
                _tmpNeeds[index].AddNecessitat();
            }
        }
        this.necessitats = _tmpNeeds.ToArray();
    }
    public Casa(Vector2Int pe�a, Necessitat[] necessitats)
    {
        this.coordenadaPe�a = pe�a;
        this.necessitats = necessitats;
    }


    [SerializeField] Pe�a pe�a;
    [SerializeField] Necessitat[] necessitats;


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
                if(!necessitats[i].proveit)
                {
                    proveit = false;
                    break;
                }
            }
            return proveit;
        }
        
    }
    public Necessitat[] Necessitats => necessitats;

    public Pe�a Pe�a => pe�a;
    public SavedCasa Save => pe�a != null ? new SavedCasa(necessitats, pe�a != null ? pe�a.Coordenades : new Vector2Int(-1,-1)) : null;
    public void LoadLastStep(Grid grid)
    {
        Debug.LogError(coordenadaPe�a);

        if (coordenadaPe�a != null)
            pe�a = (Pe�a)grid.Get(coordenadaPe�a);

        //Ocupar(producte);
    }

    public bool ProveitPerMi(Pe�a productor)
    {
        bool proveit = false;
        for (int i = 0; i < necessitats.Length; i++)
        {
            if (necessitats[i].TeProveidor)
            {
                if (necessitats[i].Proveidor == productor.Coordenades) 
                {
                    proveit = true;
                    break;
                } 
            }
        }
        return proveit;
    }

    /// <summary>
    /// Aporta un recurs a les necessitats de la casa.
    /// </summary>
    /// <returns>Retorna si el recurs s'ha pogut entregar o no, si es que no, s'ha d'entregar a una altre casa.</returns>
    public bool Proveir(Producte recurs, Pe�a productor, out bool haEstatProveit)
    {
        bool proveit = false;
        bool visualitzar = false;
        for (int i = 0; i < necessitats.Length; i++)
        {
            if (necessitats[i].TeProveidor)
            {
                if (necessitats[i].Proveidor == productor.Coordenades) 
                {
                    visualitzar = true;
                    proveit = false;
                }
                continue;
            }

            if (!necessitats[i].Producte.Equals(recurs))
                continue;

            if (necessitats[i].Proveir(productor.Coordenades))
            {
                visualitzar = true;
                proveit = true;
            }

        }
        haEstatProveit = proveit;
        return visualitzar;
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
        public Necessitat(Producte recurs, Pe�a pe�a)
        {
            this.producte = recurs;
            this.pe�a = pe�a;
            //proveits = new bool[1];
        }

        [SerializeField] Producte producte;
        [SerializeField] Pe�a pe�a;
        [SerializeField] Vector2Int proveidor = -Vector2Int.one;
        //[SerializeField] bool[] proveits;
        [SerializeField] public bool proveit;
        [SerializeField] bool comprovat = false;

        public bool TeProveidor => proveidor != -Vector2Int.one;
        public Vector2Int Proveidor => proveidor;
        public bool Comprovat { get => comprovat; set => comprovat = value; }
        public Pe�a Pe�a => pe�a;

        //INTERN
        bool complet = false;

        public Producte Producte => producte;
        /*public bool Complet
        {
            get
            {
                complet = true;
                for (int i = 0; i < proveits.Length; i++)
                {
                    if (proveits[i] == false)
                    {
                        complet = false;
                        break;
                    }
                }
                return complet;
            }
        }*/


        /// <summary>
        /// Utilitzat nom�s a la iniciaci� de la casa.
        /// </summary>
        public void AddNecessitat() 
        {
            /*List<bool> tmp = new List<bool>(proveits);
            tmp.Add(false);
            proveits = tmp.ToArray();*/
        } 

        /// <summary>
        /// Dona un recurs per cubrir la necessitat.
        /// </summary>
        /// <returns>Retorna si la necessitat est� complerta, i per tant s'ha de pujar un nivell.</returns>
        public bool Proveir(Vector2Int proveidor)
        {
            this.proveidor = proveidor;
            return true;
            /*//Donar recurs
            for (int i = 0; i < proveits.Length; i++)
            {
                if(proveits[i] == false)
                {
                    proveits[i] = true;
                    break;
                }
            }

            //Si solventat no toninua
            if (solventat)
                return false;

            //Comprovar complet
            if (TeProveidor)
            {
                solventat = true;
                return true;
            }
            else return false;*/

        }

        public void Clean()
        {
            comprovat = false;
            /*for (int i = 0; i < proveits.Length; i++)
            {
                proveits[i] = false;
            }*/
        }
    }
}
