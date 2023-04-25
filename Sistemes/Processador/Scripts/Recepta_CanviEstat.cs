using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Receptes/Canvi d'Estat")]
public class Recepta_CanviEstat : Recepta
{
    [SerializeField] Pe�a.EstatConnexioEnum connexio;

    List<object> estatsVeins;

    public bool TeInputsIguals(List<object> veins)
    {
        if (veins.Count == 0 || veins[0] is not Pe�a)
            return false;

        estatsVeins = new List<object>();
        for (int i = 0; i < veins.Count; i++)
        {
            if(connexio > 0)
            {
                if(connexio == Pe�a.EstatConnexioEnum.NoImporta)
                {
                    estatsVeins.Add(((Pe�a)veins[i]).Subestat);
                }
                else
                {
                    if (((Pe�a)veins[i]).EstatConnexio == connexio) estatsVeins.Add(((Pe�a)veins[i]).Subestat);
                }
            }
        }

        return base.TeInputsIguals(estatsVeins);
    }
}
