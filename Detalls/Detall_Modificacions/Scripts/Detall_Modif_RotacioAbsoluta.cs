using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/Modificacio/Rotacio")]
public class Detall_Modif_RotacioAbsoluta : Detall_Modificacio
{
    public enum Accio
    {
        Posicio,
        Rotacio
    }

    [SerializeField] Accio accio;
    [SerializeField] Vector3 valor;
    [SerializeField] Vector3 valorRandom;
    [SerializeField] bool relatiu;
    public override void Modificar(TilePotencial tile, GameObject detall)
    {
        switch (accio)
        {
            case Accio.Posicio:
                if (!relatiu)
                {
                    detall.transform.position =
                        tile.Peça.transform.position + 
                        new Vector3(
                        Random.Range(valor.x, valorRandom.x),
                        Random.Range(valor.y, valorRandom.y),
                        Random.Range(valor.z, valorRandom.z));
                }
                else
                {
                    detall.transform.position = 
                        tile.TileFisic.transform.position +
                        (tile.TileFisic.transform.right * Random.Range(valor.x, valorRandom.x)) +
                        (tile.TileFisic.transform.up * Random.Range(valor.y, valorRandom.y)) +
                        (tile.TileFisic.transform.forward * Random.Range(valor.z, valorRandom.z));
                }
                break;
            case Accio.Rotacio:
                if (!relatiu)
                    detall.transform.rotation = Quaternion.Euler(XS_Rotation.RandomRotation(valor, valorRandom));
                else detall.transform.localRotation = Quaternion.Euler(XS_Rotation.RandomRotation(valor, valorRandom));
                break;
        }
    }
}
