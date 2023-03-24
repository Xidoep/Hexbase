using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/GameObject/Camí")]
public class Detall_GameObject_Cami : Detall_GameObject
{
    const string NO_CAMI = "000000";

    [SerializeField] Estat cami;
    //[SerializeField] List<Estat> estatsCaminables;
    //[SerializeField] Detall_Tiles_Subestats estatsCaminables;
    [SerializeField] Dependencia[] tilesProhibits;

    [SerializeField][Range(0,6)] int caminsMinims = 1;

    public string binary;
    int index;
    string codi = "";
    bool found = false;
    int rotation = 0;
    bool prohibit = false;


    public override GameObject Get(Peça peça, TilePotencial tile)
    {
       // if(peça.VeinsPeça.Count < caminsMinims) 
       //     return null;

        List<Hexagon> veins = peça.Veins;
        binary = "";
        

        for (int i = 0; i < veins.Count; i++)
        {
            if (veins[i] != null && veins[i].EsPeça)
            {
                if (peça.EstatIgualA(cami))
                {
                    if (!EstaProhibit(peça.Tiles[i]))
                    {
                        binary += ((Peça)veins[i]).Subestat.Caminable ? "1" : "0";
                    }
                    else binary += "0";

                }
                else binary += ((Peça)veins[i]).EstatIgualA(cami) ? "1" : "0";
                
            }
            else binary += "0";
        }


        if (binary == NO_CAMI && !peça.EstatIgualA(cami))
            return null;

        found = false;
        for (int d = detalls.Length - 1; d >= 0; d--)
        {
            codi = detalls[d].name.Substring(detalls[d].name.Length - 6, 6);

            for (int i = 0; i < 6; i++)
            {
                //Debug.Log($"{peça.name} Provar Cami: {(codi.Substring(i, 6 - i) + codi.Substring(0, i))} = {(codi.Substring(i, 6 - i) + codi.Substring(0, i)).Equals(binary)}");
                if ((codi.Substring(i, 6 - i) + codi.Substring(0, i)).Equals(binary))
                {
                    index = d;
                    found = true;
                    rotation = i;
                    break;
                }
            }

            if (found) break;
            //}
        }

        //detalls[index].transform.GetChild(0).rotation = Quaternion.Euler(0, -60 * rotation, 0);
        //detalls[index].transform.rotation = Quaternion.Euler(0, -60 * rotation, 0);
        return detalls[index];
    }



    bool EstaProhibit(TilePotencial tile)
    {
        prohibit = false;
        for (int p = 0; p < tilesProhibits.Length; p++)
        {
            if (tilesProhibits[p].Cohincideix(tile))
            {
                prohibit = true;
                break;
            }
        }
        return prohibit;
    }
}
