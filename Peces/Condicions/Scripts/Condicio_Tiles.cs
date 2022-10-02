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

    public override bool Comprovar(Pe�a pe�a, Proximitat proximitat, Grups grups, Estat cami)
    {
        if (pe�a.SubestatIgualA(objectiu))
            return false;

        if (!invertit)
        {
            for (int i = 0; i < pe�a.Tiles.Length; i++)
            {
                if (tilesBuscats.Contains(pe�a.Tiles[i].PossibilitatsVirtuals.Get(0).Tile))
                {
                    Canviar(pe�a);
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
                    Canviar(pe�a);
                    return true;
                }
            }
        }
        
       

        return false;
    }
}
