using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detall_Pisos : Detall
{
    Peça peça;
    TilePotencial tile;
    [SerializeField] Detall_Pis[] pisos;
    [SerializeField] int indexTile = -1;
    [SerializeField] int[] altures;



    private void OnEnable()
    {
        if (peça == null) peça = GetComponentInParent<Peça>();
        if (pisos == null || pisos.Length == 0) pisos = GetComponentsInChildren<Detall_Pis>();





        for (int i = 0; i < pisos.Length; i++)
        {
            pisos[i].orientacioFisica = peça.Tiles[i].OrientacioFisica;
        }

        altures = new int[] {1,1,1 };

        peça.enCrearDetalls += Crear;
    }


    private void OnDisable()
    {
        peça.enCrearDetalls -= Crear;
    }



    void Crear()
    {
        for (int i = 0; i < peça.Tiles.Length; i++)
        {
            if (peça.Tiles[i].TileFisic == gameObject)
            {
                indexTile = i;
                break;
            }
        }
        tile = peça.Tiles[indexTile];

        /*altures = new int[]
        {
            peça.CasesLength,
            peça.CasesLength,
            peça.CasesLength,
        }; */
        altures = new int[] { 1, 1, 1 };

        altures[0] = tile.Veins[0] != null ? tile.Veins[0].TileFisic.TryGetComponent(out Detall_Pisos pisosExt) ? pisosExt.altures[0] : 1 : 1;
        altures[1] = tile.Veins[1].TileFisic.TryGetComponent(out Detall_Pisos pisosEsq) ? pisosEsq.altures[2] : 1;
        altures[2] = tile.Veins[2].TileFisic.TryGetComponent(out Detall_Pisos pisosDre) ? pisosDre.altures[1] : 1;


        Debug.Log($"({peça.name}) {peça.CasesLength}");
        for (int i = 0; i < pisos.Length; i++)
        {
            pisos[i].Crear(altures[pisos[i].orientacioCasa], peça.Tiles[indexTile]);
        }
    }

    private void OnValidate()
    {
        if (pisos == null || pisos.Length == 0) pisos = GetComponentsInChildren<Detall_Pis>();
    }
}
