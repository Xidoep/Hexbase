using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Estats/Riu")]
public class EstatPeça_Riu : EstatPeça
{
    [Header("RIU")]
    [Header("Inicials")]
    [SerializeField] Tile riu;
    [SerializeField] Tile riuTrencat;
    [SerializeField] Tile terra;
    [Header("VeinsNulls")]
    [SerializeField] List<EstatPeça> aigues;
    [SerializeField] Connexio cRiu;
    [SerializeField] Connexio cCostaA;
    [SerializeField] Connexio cCostaD;
    [SerializeField] Connexio cTerra;


    int veinsAcuatics;
    List<TilePotencial> tilesReordenats;
    int ultimaAigua;
    bool riuEntrat;
    TilePotencial tilePerReordenar;
    public override void TilesInicials(TilePotencial[] tiles)
    {
        VeinsAcuatics(tiles);

        if (veinsAcuatics == 0)
        {
            NoAigua(tiles);
        }      
        else if(veinsAcuatics == 1)
        {
            Reordenar(tiles);
            UnaAigua(tilesReordenats);
        }
        else
        {
            Reordenar(tiles);
            MultiplesAigues(tilesReordenats);
        }

    }
    public override Connexio[] Null(TilePotencial tile)
    {
        VeinsAcuatics(tile.Peça.Tiles);
        return new Connexio[] { ((veinsAcuatics <= 1) ? cRiu : cTerra), cCostaA, cCostaD };
    }



    void NoAigua(TilePotencial[] tiles)
    {
        if (Random.Range(0, 2) == 0)
        {
            Riu(tiles[0]);
            Sortida(tiles[1]);
            Terra(tiles[2]);
            Terra(tiles[3]);
            Entrada(tiles[4]);
            Riu(tiles[5]);
        }
        else
        {
            Terra(tiles[0]);
            Entrada(tiles[1]);
            Riu(tiles[2]);
            Riu(tiles[3]);
            Sortida(tiles[4]);
            Terra(tiles[5]);
        }
    }
    void UnaAigua(List<TilePotencial> tiles)
    {
        if (Random.Range(0, 2) == 0)
        {
            Sortida(tiles[0]);
            Terra(tiles[1]);
            Terra(tiles[2]);
            Entrada(tiles[3]);
            Riu(tiles[4]);
            Riu(tiles[5]);
        }
        else
        {
            Entrada(tiles[0]);
            Riu(tiles[1]);
            Riu(tiles[2]);
            Sortida(tiles[3]);
            Terra(tiles[4]);
            Terra(tiles[5]);
        }
    }
    void MultiplesAigues(List<TilePotencial> tiles)
    {
        ultimaAigua = 5;
        for (int i = 5; i > 0; i--)
        {
            if (!VeiNull(tiles[i]) && aigues.Contains(tiles[i].Veins[0].Estat))
            {
                ultimaAigua = i;
                break;
            }
        }

        riuEntrat = false;
        for (int i = 0; i < tiles.Count; i++)
        {
            if (!riuEntrat)
            {
                Entrada(tiles[i]);
                riuEntrat = true;
            }
            else
            {
                if (i < ultimaAigua)
                {
                    Riu(tiles[i]);
                    /*if (VeiNull(tiles[i])) Riu(tiles[i]);
                    else if (VeiAcuatic(tiles[i]) && Random.Range(0,2) == 0)
                        Afluent(tiles[i]);
                    else
                        Riu(tiles[i]);*/
                }
                else if (i == ultimaAigua)
                {
                    Sortida(tiles[i]);
                }
                else
                    Terra(tiles[i]);
            }
        }
    }


    bool VeiAcuatic(TilePotencial tile) => aigues.Contains(tile.Veins[0].Estat);
    void Reordenar(TilePotencial[] tiles)
    {
        tilesReordenats = new List<TilePotencial>(tiles);
        for (int i = 0; i < tilesReordenats.Count; i++)
        {
            if (!VeiNull(tilesReordenats[0]) && VeiAcuatic(tilesReordenats[0]))
            {
                Debug.Log($"Començem per = {tilesReordenats[0].ID}");
                break;
            }
            else
            {
                tilePerReordenar = tilesReordenats[0];
                tilesReordenats.RemoveAt(0);
                tilesReordenats.Add(tilePerReordenar);
            }
        }
    }
    void VeinsAcuatics(TilePotencial[] tiles)
    {
        veinsAcuatics = 0;
        for (int i = 0; i < tiles.Length; i++)
        {
            if (VeiNull(tiles[i]))
                continue;

            if (aigues.Contains(tiles[i].Veins[0].Estat)) veinsAcuatics++;
        }
        Debug.Log($"veins aquatics = {veinsAcuatics}");
    }



    void Entrada(TilePotencial tile) => tile.Escollir(riu, 2);
    void Riu(TilePotencial tile) => tile.Escollir(riu, 1);
    void Sortida(TilePotencial tile) => tile.Escollir(riu, 0);
    void Terra(TilePotencial tile) => tile.Escollir(terra, 0);
    void Afluent(TilePotencial tile) => tile.Escollir(riuTrencat, 0);
}
