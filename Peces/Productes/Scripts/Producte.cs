using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Recurs")]
public class Producte : ScriptableObject
{
    [SerializeField] Texture2D icone;

    public Texture2D Icone => icone;
}

public abstract class EstrategiaDeProduccio : ScriptableObject
{
    public abstract Producte[] Produir(Producte recurs);

    public abstract int Numero { get; }
}