using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/Modificacio/Aresta")]
public class Detall_Modif_Aresta : Detall_Modificacio
{
    [SerializeField] bool dreta;

    float altura = 0.866f;
    float costat = 0.5f;


    public override void Modificar(TilePotencial tile, GameObject detall)
    {
        switch (tile.Orientacio)
        {
            case 0:
                detall.transform.position = tile.TileFisic.transform.position + tile.TileFisic.transform.forward * altura + tile.TileFisic.transform.right * (costat * (dreta ? 1 : -1)); 
                break;
            case 1:
                if (dreta) detall.transform.position = tile.TileFisic.transform.position;
                else detall.transform.position = tile.TileFisic.transform.position + tile.TileFisic.transform.forward * altura + tile.TileFisic.transform.right * (costat * 1);
                break;
            case 2:
                if (dreta) detall.transform.position = tile.TileFisic.transform.position + tile.TileFisic.transform.forward * altura + tile.TileFisic.transform.right * (costat * -1);
                else detall.transform.position = tile.TileFisic.transform.position;
                break;
            default:
                break;
        }
        //detall.transform.rotation = Quaternion.Euler(0, tile.Orientacio * 60 + offset, 0);

    }
}
