using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/Casa")]
public class ConstruccioCases : DetallScriptable
{
    [SerializeField] int cantonsConnectats;
    [SerializeField] Subestat casa;
    public override GameObject GameObject(Pe�a pe�a, TilePotencial tile)
    {
        SetAlturaSiCal(tile, pe�a);

        for (int i = 0; i < tile.Veins.Length; i++)
        {
            if (tile.Veins[i] == null)
                continue;

            if(tile.Pe�a.SubestatIgualA(casa))
                SetAlturaSiCal(tile.Veins[i], tile.Pe�a);
        }



        return base.GameObject(pe�a, tile);
    }

    void SetAlturaSiCal(TilePotencial tile, Pe�a pe�a)
    {
        if (tile.TeAltura)
            return;

        tile.Altura = Random.Range(0, pe�a.Casa.Necessitats.Length);
    }
}
