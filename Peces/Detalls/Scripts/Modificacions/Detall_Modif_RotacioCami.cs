using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/Modificacio/Roatacio Cami")]
public class Detall_Modif_RotacioCami : Detall_Modificacio
{
    const string NO_CAMI = "000000";

    [SerializeField] Estat cami;

    public string binary;
    string codi = "";
    bool prohibit = false;
    int rotation = 0;

    public override void Modificar(TilePotencial tile, GameObject detall)
    {
        List<Hexagon> veins = tile.Peça.Veins;
        binary = "";

        for (int i = 0; i < veins.Count; i++)
        {
            if (veins[i] != null && veins[i].EsPeça)
            {
                if (tile.Peça.EstatIgualA(cami))
                {
                    binary += ((Peça)veins[i]).Subestat.Caminable ? "1" : "0";
                }
                else binary += ((Peça)veins[i]).EstatIgualA(cami) ? "1" : "0";

            }
            else binary += "0";
        }

        codi = detall.ToString().Substring(3, 6);

        for (int i = 0; i < 6; i++)
        {
            if ((codi.Substring(i, 6 - i) + codi.Substring(0, i)).Equals(binary))
            {
                rotation = i;
                break;
            }
        }

        detall.transform.rotation = Quaternion.Euler(0, -60 * rotation, 0);
    }

}
