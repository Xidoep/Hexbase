using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Recurs")]
public class Recurs : ScriptableObject
{
    [SerializeField] EstrategiaDeProduccio estrategia;

    public Recurs[] Produir(Peça productor) => estrategia.Executar(productor,this);
}

public abstract class EstrategiaDeProduccio : ScriptableObject
{
    public abstract Recurs[] Executar(Peça productor, Recurs recurs);
}