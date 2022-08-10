using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Condicio/byPlatja")]
public class Condicio_Platja : Condicio
{
    [Linia]
    [Apartat("CONDICIO PLATJA")]
    [SerializeField] List<Tile> tilesPlatja;
    [SerializeField] bool potConvertirVeins;

    [SerializeField] Estat casa;
    //INTERN
    List<Pe�a> m_veins;

    public override bool Comprovar(Pe�a pe�a, Proximitat proximitat)
    {
        if (pe�a.SubestatIgualA(objectiu))
            return false;

        m_veins = Veins(pe�a);
        if (potConvertirVeins) 
        {
            for (int i = 0; i < m_veins.Count; i++)
            {
                if (m_veins[i].SubestatIgualA(objectiu))
                    return false;
            }
        }
        

        for (int i = 0; i < pe�a.Tiles.Length; i++)
        {
            if (tilesPlatja.Contains(pe�a.Tiles[i].Possibilitats[0]))
            {
                Canviar(pe�a);
                return true;
            }
        }

        //If I reach this point, means that any tile is one of the ones i was looking for.
        if (potConvertirVeins)
        {
            for (int v = 0; v < m_veins.Count; v++)
            {
                if (m_veins[v].EstatIgualA(casa))
                    continue;

                for (int i = 0; i < m_veins[v].Tiles.Length; i++)
                {
                    if (tilesPlatja.Contains(m_veins[v].Tiles[i].Possibilitats[0]))
                    {
                        //Canviar(pe�a);
                        Canviar(m_veins[v]);
                        return true;
                    }
                }
            }
        }
       

        return false;
    }
}
