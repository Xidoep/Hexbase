using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Condicio/byTiles")]
public class Condicio_Tiles : Condicio
{
    [Linia]
    [Apartat("CONDICIO TILES")]
    [SerializeField] List<Tile> tilesBuscats;

    [Tooltip("En comptes de complirse quan troba algun dels tiles, es complirà quan no en trobi cap.")]
    [SerializeField] bool invertit;

    public override bool Comprovar(Peça peça, Proximitat proximitat, Grups grups, Estat cami)
    {
        if (peça.SubestatIgualA(objectiu))
            return false;

        if (!invertit)
        {
            for (int i = 0; i < peça.Tiles.Length; i++)
            {
                if (tilesBuscats.Contains(peça.Tiles[i].PossibilitatsVirtuals.Get(0).Tile))
                {
                    Canviar(peça);
                    return true;
                }
            }
        }
        else //INVERTIT
        {
            for (int i = 0; i < peça.Tiles.Length; i++)
            {
                if (!tilesBuscats.Contains(peça.Tiles[i].PossibilitatsVirtuals.Get(0).Tile))
                {
                    Canviar(peça);
                    return true;
                }
            }
        }
        
       

        return false;
    }
}
