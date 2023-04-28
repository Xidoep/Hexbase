using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedCasa
{
    /*public SavedCasa(Casa.Necessitat_old[] necessitats)
    {
        savedNecessitats = new SavedNecessitat(necessitats);
    }*/

    [SerializeField] SavedNecessitat savedNecessitats;

    /*public Casa Load(System.Func<string, Producte> producteNomToPrefab)
    {
        return new Casa(savedNecessitats.Load(producteNomToPrefab));
    }*/
}
