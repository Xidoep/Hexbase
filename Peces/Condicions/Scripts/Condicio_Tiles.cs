using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Condicio/byTiles")]
public class Condicio_Tiles : Condicio
{
    [Linia]
    [Apartat("CONDICIO TILES")]
    [SerializeField] List<Tile> tilesBuscats;

    [Tooltip("En comptes de complirse quan troba algun dels tiles, es complir� quan no en trobi cap.")]
    [SerializeField] bool invertit;

    public override bool Comprovar(Pe�a pe�a, Grups grups, Estat cami, bool canviar, System.Action<Pe�a, bool, int> enConfirmar, System.Action<Pe�a, int> enCanviar)
    {
        if (pe�a.SubestatIgualA(objectiu))
            return false;

        if (!invertit)
        {
            for (int i = 0; i < pe�a.Tiles.Length; i++)
            {
                if (tilesBuscats.Contains(pe�a.Tiles[i].PossibilitatsVirtuals.Get(0).Tile))
                {
                    enConfirmar.Invoke(pe�a, canviar, punts);
                    if (canviar)
                        Canviar(pe�a, enCanviar);
                    return true;
                }
            }
        }
        else //INVERTIT
        {
            for (int i = 0; i < pe�a.Tiles.Length; i++)
            {
                if (!tilesBuscats.Contains(pe�a.Tiles[i].PossibilitatsVirtuals.Get(0).Tile))
                {
                    enConfirmar.Invoke(pe�a, canviar, punts);
                    if (canviar)
                        Canviar(pe�a, enCanviar);
                    return true;
                }
            }
        }
        
       

        return false;
    }

    new public void OnValidate()
    {
        base.OnValidate();
    }
}
