using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/GameObject/Camí")]
public class Detall_GameObject_Cami : Detall_GameObject
{
    const string NO_CAMI = "000000";

    [SerializeField] Estat cami;
    //[SerializeField] List<Estat> estatsCaminables;
    [SerializeField] Detall_Tiles_Estats estatsCaminables;
    [SerializeField] Dependencia[] tilesProhibits;

    int[] bits;
    public string binary;
    int index;
    string codi = "";
    bool found = false;
    int rotation = 0;
    bool prohibit = false;


    public override GameObject Get(Peça peça, TilePotencial tile)
    {

        List<Hexagon> veins = peça.Veins;
        bits = new int[6];
        binary = "";
        

        for (int i = 0; i < veins.Count; i++)
        {
            if (veins[i] != null && veins[i].EsPeça)
            {
                if (peça.EstatIgualA(cami))
                {
                    if (!EstaProhibit(peça.Tiles[i]))
                    {
                        binary += estatsCaminables.Estats.Contains(((Peça)veins[i]).Estat) ? "1" : "0";
                    }
                    else binary += "0";
                    /*if (estatsCaminables.Estats.Contains(((Peça)veins[i]).Estat))
                    {
                        prohibit = true;
                        for (int p = 0; p < tilesProhibits.Length; p++)
                        {
                            if (tilesProhibits[p].Cohincideix(tile))
                            {
                                prohibit = false;
                                break;
                            }
                        }
                        binary += prohibit ? "1" : "0";
                    }
                    else binary += "0";*/

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
            /*Debug.Log($"Provar Cami: {detalls[d].ToString().Substring(0, 6)} = {detalls[d].ToString().Substring(0, 6).Equals(binary)}");
            if (detalls[d].ToString().Substring(0, 6).Equals(binary)) 
            { 
                index = d;
                break;
            } 
            else
            {*/
            codi = detalls[d].ToString().Substring(3, 6);

            for (int i = 0; i < 6; i++)
            {
                Debug.Log($"Provar Cami: {(codi.Substring(i, 6 - i) + codi.Substring(0, i))} = {(codi.Substring(i, 6 - i) + codi.Substring(0, i)).Equals(binary)}");
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

        detalls[index].transform.GetChild(0).rotation = Quaternion.Euler(0, -60 * rotation, 0);
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
