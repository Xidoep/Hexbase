using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Estats/Montanya")]
public class EstatPeça_Montanya : EstatPeça
{
    [Header("MONTANYA")]
    [Header("Inicials")]
    [SerializeField] Tile punta;
    [SerializeField] Tile serra;
    [SerializeField] Tile cim;
    //[SerializeField] Tile serra;
    //[SerializeField] EstatPeça aigues;
    [Header("VeinsNulls")]
    [SerializeField] Connexio[] nules;
    public override void TilesInicials(TilePotencial[] tiles)
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            if (VeiNull(tiles[i]))
            {
                Punta(tiles[i]);
                continue;
            }


            if (VeiMontanya(tiles[i]))
            {
                if (AnteriorMontanyos(tiles, i))
                {
                    if(PosteriorMontanyos(tiles, i))
                    {
                        //Montanya davant i derrera
                        Cim(tiles[i]);
                    }
                    else
                    {
                        //Motanya derrera pero no davant
                        Final(tiles[i]);
                    }
                }
                else
                {
                    if (PosteriorMontanyos(tiles, i))
                    {
                        Inici(tiles[i]);
                    }
                    else
                    {
                        Punta(tiles[i]);
                    }
                }
            }
            else
            {
                Punta(tiles[i]);
            }
        }
    }
    public override Connexio[] Null(TilePotencial tile)
    {
        return nules;
    }



    bool VeiMontanya(TilePotencial tile) => tile.Veins[0].Estat == this;
    bool AnteriorMontanyos(TilePotencial[] tiles, int i) => !VeiNull(tiles[IndexAnterior(i)]) && VeiMontanya(tiles[IndexAnterior(i)]);
    bool PosteriorMontanyos(TilePotencial[] tiles, int i) => !VeiNull(tiles[IndexPosterior(i)]) && VeiMontanya(tiles[IndexPosterior(i)]);
    int IndexAnterior(int i) => (i == 0 ? 5 : i - 1);
    int IndexPosterior(int i) => (i + 1).ClampVeins();



    void Punta(TilePotencial tile) => tile.Escollir(punta, 0);
    void Inici(TilePotencial tile) => tile.Escollir(serra, 2);
    void Cim(TilePotencial tile) => tile.Escollir(cim, 0);
    void Final(TilePotencial tile) => tile.Escollir(serra, 1);
}
