using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Estrategies/NumeroFixe")]
public class Estrategia_NumeroFixe : EstrategiaDeProduccio
{
    [SerializeField] int produccio;

    //INTERN
    List<Producte> recursos;
    public override Producte[] Produir(Producte recurs)
    {
        if (recursos == null) recursos = new List<Producte>();
        else recursos.Clear();

        for (int i = 0; i < produccio; i++)
        {
            recursos.Add(recurs);
        }
        return recursos.ToArray();
    }

    public override int Numero => produccio;
}
