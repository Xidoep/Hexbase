using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedNecessitat
{
    public SavedNecessitat(Casa.Necessitat_old[] necessitats)
    {
        List<Necessitat> n = new List<Necessitat>();
        for (int i = 0; i < necessitats.Length; i++)
        {
            n.Add(new Necessitat() { producte = necessitats[i].Producte.name, proveit = necessitats[i].Proveit });
        }
        this.necessitats = n.ToArray();
    }

    [System.Serializable] struct Necessitat
    {
        public string producte;
        public bool proveit;
    }
    [SerializeField] Necessitat[] necessitats;

    public Casa.Necessitat_old[] Load(System.Func<string, Producte> producteNomToPrefab)
    {
        Casa.Necessitat_old[] cn = new Casa.Necessitat_old[necessitats.Length];
        for (int i = 0; i < necessitats.Length; i++)
        {
            cn[i] = new Casa.Necessitat_old(producteNomToPrefab.Invoke(necessitats[i].producte));
        }
        return cn;
    }
}
