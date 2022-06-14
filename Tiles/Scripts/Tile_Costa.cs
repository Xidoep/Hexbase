using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pels tiles que tenen costats no simetrics.
/// En aquest cas, 2. costats iguals no simetics.
/// </summary>
[CreateAssetMenu(menuName = "Xido Studio/Hex/Tiles/Penyasegat")]
public class Tile_Costa : Tile
{
    [Space(10)]
    [Header("TILE COSTA")]
    [SerializeField] Connexio connexioAssimetrica;
    [SerializeField] [Tooltip("El tile amb la direccionalitat invertida a la resta. El tile que representa la 'punta de costa'. Osigui, el tile invertit a la resta.")] 
    Tile invertit;
    [SerializeField] [Tooltip("En cas que sigui la punta")] 
    bool exterior;


    public override bool Comprovar(TilePotencial tile, int orientacioFisica, Connexio exterior, Connexio esquerra, Connexio dreta)
    {
        return true;
        if(exterior == connexioAssimetrica)
        {
            if (!Encaixa_Exterior(orientacioFisica, tile.Veins[0]))
                return false;
        }
        if(esquerra == connexioAssimetrica)
        {
            if (!Encaixa_Esquerra(orientacioFisica, tile.Veins[1]))
                return false;
        }
        if (dreta == connexioAssimetrica)
        {
            if (!Encaixa_Dreta(orientacioFisica, tile.Veins[2]))
                return false;
        }
        return true;
    }






    bool Encaixa_Exterior(int laMevaOrientacioFisica, TilePotencial vei)
    {
        if (Null_NoResolt(vei))
            return true;

        if (SomIguals(vei))
        {
            if (laMevaOrientacioFisica != vei.OrientacioFisica)
                return true;
            else return false;
        }
        else
        {
            if (laMevaOrientacioFisica == vei.OrientacioFisica)
                return true;
            else return false;
        }
    }
    bool Encaixa_Esquerra(int laMevaOrientacioFisica, TilePotencial vei)
    {
        if (Null_NoResolt(vei))
            return true;

        if (SomIguals(vei))
        {
            if (laMevaOrientacioFisica == vei.OrientacioFisica)
                return true;
            else return false;
        }
        else
        {
            switch (laMevaOrientacioFisica)
            {
                case 0:
                    if (vei.OrientacioFisica == 2)
                        return true;
                    else return false;
                case 1:
                    if (vei.OrientacioFisica == 0)
                        return true;
                    else return false;
                default: //2, la connexio a l'esquerra es diferent.
                    return false;
            }
        }
    }
    bool Encaixa_Dreta(int laMevaOrientacioFisica, TilePotencial vei)
    {
        if (Null_NoResolt(vei))
            return true;

        if (SomIguals(vei))
        {
            Debug.Log("Som iguals");
            switch (laMevaOrientacioFisica)
            {
                case 0:
                    if (vei.OrientacioFisica == 0)
                        return true;
                    else return false;
                case 2:
                    if (vei.OrientacioFisica == 1)
                        return true;
                    else return false;
                default:
                    return false;
            }
        }
        else
        {
            Debug.Log("No som iguals");
            if (laMevaOrientacioFisica != vei.OrientacioFisica)
                return true;
            else return false;
        }
    }



    bool Null_NoResolt(TilePotencial vei)
    {
        if (vei == null)
            return true;

        if (!vei.Resolt)
            return true;

        else return false;
    }

    bool SomIguals(TilePotencial vei) => exterior ? vei.Possibilitats[0] == invertit : vei.Possibilitats[0] != invertit;

    private void OnValidate()
    {
        if (invertit == null)
            return;

        exterior = invertit == this;
    }
}
