using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Estrategies/NumeroFixe")]
public class Estrategia_NumeroFixe : EstrategiaDeProduccio
{
    [SerializeField] int produccio;

    //INTERN
    List<Recurs> recursos;
    public override Recurs[] Executar(Peça productor, Recurs recurs)
    {
        if (recursos == null) recursos = new List<Recurs>();
        else recursos.Clear();

        for (int i = 0; i < produccio; i++)
        {
            recursos.Add(recurs);
        }
        return recursos.ToArray();
    }
}
