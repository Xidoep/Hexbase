using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Roductes/Producte")]
public class Producte : ScriptableObject, IProcessable
{
    public void Setup(Texture2D icone)
    {
        this.icone = icone;
        GenerarGastada();
    }

    [PropertyOrder(-2)][PreviewField][OnValueChanged("GenerarGastada")]
    [SerializeField] Texture2D icone;

    [ReadOnly] [SerializeField] Sprite sprite;
    [ReadOnly] [SerializeField] Texture2D gastada;
    [PropertyOrder(-1)] [ReadOnly] [PreviewField] [SerializeField] Sprite spriteGastada;
    public Texture2D Icone => icone;
    public Sprite Sprite 
    {
        get
        {
            if(sprite == null)
            {
                GenerarSprite();
            }
            return sprite;
        }
    } 
    public Sprite Gastada 
    {
        get 
        {
            if(spriteGastada == null)
            {
                GenerarGastada();
            }
            return spriteGastada;
        }
    } 



    void GenerarSprite()
    {
        sprite = Sprite.Create(icone, new Rect(0, 0, icone.width, icone.height), Vector2.zero);
    }

    [ContextMenu("Gastada")]
    void GenerarGastada()
    {
        gastada = new Texture2D(icone.width, icone.height, TextureFormat.ARGB32, false);
        for (int x = 0; x < icone.width; x++)
        {
            for (int y = 0; y < icone.height; y++)
            {
                gastada.SetPixel(x, y, 
                    Color.Lerp(
                        new Color(.75f, 0, 0, icone.GetPixel(x, y).a), 
                        new Color(0, 0, 0, 0), 
                        icone.GetPixel(x, y).r + icone.GetPixel(x, y).g + icone.GetPixel(x, y).b));
            }
        }
        gastada.Apply();

        spriteGastada = Sprite.Create(gastada, new Rect(0, 0, gastada.width, gastada.height), Vector2.zero);
    }

    public void Processar(Peça peça)
    {
        Debug.Log($"PROCESSAR PRODUCTE {this.name}");
        if (peça.ProductesExtrets == null) peça.SetProductesExtrets = new ProducteExtret[0];

        List<ProducteExtret> _p = new List<ProducteExtret>(peça.ProductesExtrets);
        _p.Add(new ProducteExtret(this));
        peça.SetProductesExtrets = _p.ToArray();
        //peça.IntentarConnectar();
    }

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
    public ProducteExtret(Producte producte, bool gastat)
    {
        this.producte = producte;
        this.gastat = gastat;
        informacio = new Informacio.Unitat();
    }
    public Producte producte;
    public bool gastat;
    public Informacio.Unitat informacio;
}