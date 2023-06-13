using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Detall_Pis : Detall
{
    public override void Setup(string[] orientacio)
    {
        if (orientacio[0] == "Ext") this.orientacio = 0;
        else if (orientacio[0] == "Esq") this.orientacio = 1;
        else if (orientacio[0] == "Dre") this.orientacio = 2;
    }

    [SerializeField] public int orientacio = -1;
    [SerializeField] public int orientacioFisica = -1;

    [ShowInInspector]
    public int OrientacioFinal
    {
        get
        {
            switch (orientacioFisica * 10 + orientacio)
            {
                case 0:
                    return 0;
                case 1:
                    return 2;
                case 2:
                    return 1;
                case 10:
                    return 2;
                case 11:
                    return 1;
                case 12:
                    return 0;
                case 20:
                    return 1;
                case 21:
                    return 0;
                case 22:
                    return 2;
                default:
                    return -1;
            }
        }
    }

    public void Crear(int pisos, TilePotencial tile)
    {
        Debug.Log($"Crear {pisos} pisos en orientacio {OrientacioFinal}, al tile {tile.TileFisic.name}");
        switch (OrientacioFinal)
        {
            case 0:
                Debug.Log($"En teoria hi ha una casa a {tile.Veins[OrientacioFinal].TileFisic.name} orientada cap a 0: {tile.Veins[OrientacioFinal].Veins[0].TileFisic.name} soc jo?");
                break;
            case 1:
                Debug.Log($"En teoria hi ha una casa a {tile.Veins[OrientacioFinal].TileFisic.name} orientada cap a 2: {tile.Veins[OrientacioFinal].Veins[2].TileFisic.name} soc jo?");
                break;
            case 2:
                Debug.Log($"En teoria hi ha una casa a {tile.Veins[OrientacioFinal].TileFisic.name} orientada cap a 1: {tile.Veins[OrientacioFinal].Veins[1].TileFisic.name} soc jo?");
                break;
        }
    }
}
