using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Recurs")]
public class Recurs : ScriptableObject
{
    [SerializeField] EstrategiaDeProduccio estrategia;

    public Recurs[] Produir(Pe�a productor) => estrategia.Executar(productor,this);
}

public abstract class EstrategiaDeProduccio : ScriptableObject
{
    public abstract Recurs[] Executar(Pe�a productor, Recurs recurs);
}