using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Roductes/Producte")]
public class Producte : ScriptableObject, IProcessable
{
    [SerializeField] Texture2D icone;
    [SerializeField] Sprite sprite;
    public Texture2D Icone => icone;
    public Sprite Sprite => sprite;

    void OnEnable()
    {
        sprite = Sprite.Create(icone, new Rect(0, 0, icone.width, icone.height), Vector2.zero);
    }

    public void Processar(Pe�a pe�a)
    {
        Debug.Log($"PROCESSAR PRODUCTE {this.name}");
        if (pe�a.ProductesExtrets == null) pe�a.SetProductesExtrets = new ProducteExtret[0];

        List<ProducteExtret> _p = new List<ProducteExtret>(pe�a.ProductesExtrets);
        _p.Add(new ProducteExtret(this));
        pe�a.SetProductesExtrets = _p.ToArray();

        //pe�a.IntentarConnectar();
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