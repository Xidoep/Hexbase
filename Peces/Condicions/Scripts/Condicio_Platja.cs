using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Condicio/byPlatja")]
public class Condicio_Platja : Condicio
{
    [Apartat("CONDICIO PLATJA")]
    [SerializeField] List<Tile> tilesPlatja;
    [SerializeField] bool potConvertirVeins;

    [SerializeField] Estat casa;
    //INTERN
    List<Peça> veins;

    public override bool Comprovar(Peça peça, Proximitat proximitat)
    {
        if (peça.SubestatIgualA(objectiu))
            return false;

        veins = Veins(peça);
        if (potConvertirVeins) 
        {
            for (int i = 0; i < veins.Count; i++)
            {
                if (veins[i].SubestatIgualA(objectiu))
                    return false;
            }
        }
        

        for (int i = 0; i < peça.Tiles.Length; i++)
        {
            if (tilesPlatja.Contains(peça.Tiles[i].Possibilitats[0]))
            {
                Canviar(peça);
                return true;
            }
        }

        //If I reach this point, means that any tile is one of the ones i was looking for.
        if (potConvertirVeins)
        {
            for (int v = 0; v < veins.Count; v++)
            {
                if (veins[v].EstatIgualA(casa))
                    continue;

                for (int i = 0; i < veins[v].Tiles.Length; i++)
                {
                    if (tilesPlatja.Contains(veins[v].Tiles[i].Possibilitats[0]))
                    {
                        //Canviar(peça);
                        Canviar(veins[v]);
                        return true;
                    }
                }
            }
        }
       

        return false;
    }
}
