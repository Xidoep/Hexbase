using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Condicio/byTiles")]
public class Condicio_Tiles : Condicio
{
    [Linia]
    [Apartat("CONDICIO PLATJA")]
    [SerializeField] List<Tile> tilesBuscats;

    [Tooltip("En comptes de complirse quan troba algun dels tiles, es complir� quan no en trobi cap.")][SerializeField] bool invertit;
    [SerializeField] bool potConvertirVeins;

    [Linia]
    [Tooltip("En cas que guigui convertir venis, el que tinguin aquest estat no seran afectats")][SerializeField] List<Estat> estatsInmunes;

    //INTERN
    List<Pe�a> m_veins;

    public override bool Comprovar(Pe�a pe�a, Proximitat proximitat, Grups grups, Estat cami)
    {
        if (pe�a.SubestatIgualA(objectiu))
            return false;

        m_veins = Veins(pe�a);

        if (!invertit)
        {
            //AH! aixo prevent que hi hagui dos ports de costat.
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
                if (tilesBuscats.Contains(pe�a.Tiles[i].PossibilitatsVirtuals.Get(0).Tile))
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
                    if(estatsInmunes.Contains(m_veins[v].Estat))
                        continue;

                    for (int i = 0; i < m_veins[v].Tiles.Length; i++)
                    {
                        if (tilesBuscats.Contains(m_veins[v].Tiles[i].PossibilitatsVirtuals.Get(0).Tile))
                        {
                            //Canviar(pe�a);
                            Canviar(m_veins[v]);
                            return true;
                        }
                    }
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
