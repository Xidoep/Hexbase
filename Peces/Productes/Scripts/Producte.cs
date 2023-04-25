using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Roductes/Producte")]
public class Producte : ScriptableObject, IProcessable
{
    [SerializeField] Texture2D icone;

    public Texture2D Icone => icone;

    public void Processar(Pe�a pe�a)
    {
        Debug.Log($"PROCESSAR PRODUCTE {this.name}");
        if (pe�a.ExtreureProducte == null) pe�a.SetProductesExtrets = new ProducteExtret[0];

        List<ProducteExtret> _p = new List<ProducteExtret>(pe�a.ExtreureProducte);
        _p.Add(new ProducteExtret(this));
        pe�a.SetProductesExtrets = _p.ToArray();

        pe�a.IntentarConnectar();
    }
}

public abstract class EstrategiaDeProduccio : ScriptableObject
{
    public abstract Producte[] Produir(Producte recurs);
}





[System.Serializable]
public struct ProducteExtret
{
    public ProducteExtret(Producte producte)
    {
        this.producte = producte;
        gastat = false;
        informacio = new Informacio.Unitat();
    }
    public Producte producte;
    public bool gastat;
    public Informacio.Unitat informacio;
}