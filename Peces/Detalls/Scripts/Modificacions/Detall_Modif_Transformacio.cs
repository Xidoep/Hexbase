using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/Modificacio/Transformacio")]
public class Detall_Modif_Transformacio : Detall_Modificacio
{
    public enum Accio
    {
        Posicio,
        Rotacio,
        Escala
    }

    [SerializeField] Accio accio;
    [Linia]
    [SerializeField] Vector3 valor;
    [SerializeField] Vector3 valorRandom;
    [Nota("Si no vols randomitzacio, deixa els dos valor iguals")]
    [Linia]
    [Header("OPCIONS")]
    [SerializeField] bool aditiu;
    [Nota("No hi ha opcio relativa per la rotacio.",NoteType.Warning)]
    [SerializeField] bool relatiu;



    public override void Modificar(TilePotencial tile, GameObject detall)
    {
        switch (accio)
        {
            case Accio.Posicio:
                if (!relatiu)
                {
                    if (!aditiu)
                        detall.transform.position = tile.Peça.transform.position + PosicioAbsoluta();
                    else detall.transform.position += PosicioAbsoluta();
                }
                else
                {
                    if (!aditiu)
                        detall.transform.localPosition = tile.TileFisic.transform.localPosition + PosicioRelativa(tile);
                    else detall.transform.localPosition += PosicioRelativa(tile);
                }
                break;
            case Accio.Rotacio:
                if (!relatiu)
                {
                    if (!aditiu)
                        detall.transform.rotation = Rotacio();
                    else detall.transform.rotation = RotacioAditiva(detall);
                }
                else 
                {
                    if (!aditiu)
                        detall.transform.localRotation = Rotacio();
                    else detall.transform.rotation = RotacioAditiva(detall);
                } 
                break;
            case Accio.Escala:
                detall.transform.localScale = Escala();
                break;
        }
    }



    Vector3 PosicioAbsoluta() => new Vector3(
        Random.Range(valor.x, valorRandom.x), 
        Random.Range(valor.y, valorRandom.y), 
        Random.Range(valor.z, valorRandom.z));

    Vector3 PosicioRelativa(TilePotencial tile) => 
        (tile.TileFisic.transform.right * Random.Range(valor.x, valorRandom.x)) +
        (tile.TileFisic.transform.up * Random.Range(valor.y, valorRandom.y)) +
        (tile.TileFisic.transform.forward * Random.Range(valor.z, valorRandom.z));

    Quaternion Rotacio() => Quaternion.Euler(XS_Rotation.RandomRotation(valor, valorRandom));
    Quaternion RotacioAditiva(GameObject detall) => Quaternion.Euler(detall.transform.localEulerAngles + XS_Rotation.RandomRotation(valor, valorRandom));

    Vector3 Escala() => new Vector3(
        Random.Range(valor.x, valorRandom.x), 
        Random.Range(valor.y, valorRandom.y), 
        Random.Range(valor.z, valorRandom.z));
}
