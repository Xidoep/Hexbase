using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Roductes/Producte")]
public class Producte : ScriptableObject, IProcessable
{
    [SerializeField] Texture2D icone;

    public Texture2D Icone => icone;

    public void Processar(Peça peça)
    {
        Debug.Log($"PROCESSAR PRODUCTE {this.name}");
        if (peça.ExtreureProducte == null) peça.SetProductesExtrets = new ProducteExtret[0];

        List<ProducteExtret> _p = new List<ProducteExtret>(peça.ExtreureProducte);
        _p.Add(new ProducteExtret(this));
        peça.SetProductesExtrets = _p.ToArray();

        peça.IntentarConnectar();
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