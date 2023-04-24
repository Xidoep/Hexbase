using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Roductes/Producte")]
public class Producte : ScriptableObject
{
    [SerializeField] Texture2D icone;

    public Texture2D Icone => icone;
}

public abstract class EstrategiaDeProduccio : ScriptableObject
{
    public abstract Producte[] Produir(Producte recurs);
}
