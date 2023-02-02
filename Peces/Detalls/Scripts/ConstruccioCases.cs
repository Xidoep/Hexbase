using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/Casa")]
public class ConstruccioCases : DetallScriptable
{
    [SerializeField] int cantonsConnectats;
    [SerializeField] Subestat casa;
    public override GameObject GameObject(Peça peça, TilePotencial tile)
    {
        SetAlturaSiCal(tile, peça);

        for (int i = 0; i < tile.Veins.Length; i++)
        {
            if (tile.Veins[i] == null)
                continue;

            if(tile.Peça.SubestatIgualA(casa))
                SetAlturaSiCal(tile.Veins[i], tile.Peça);
        }



        return base.GameObject(peça, tile);
    }

    void SetAlturaSiCal(TilePotencial tile, Peça peça)
    {
        if (tile.TeAltura)
            return;

        tile.Altura = Random.Range(0, peça.Casa.Necessitats.Length);
    }
}
