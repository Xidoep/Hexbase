using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detall_Pisos : Detall
{
    Pe�a pe�a;
    TilePotencial tile;
    [SerializeField] Detall_Pis[] pisos;
    [SerializeField] int indexTile = -1;
    [SerializeField] int[] altures;



    private void OnEnable()
    {
        if (pe�a == null) pe�a = GetComponentInParent<Pe�a>();
        if (pisos == null || pisos.Length == 0) pisos = GetComponentsInChildren<Detall_Pis>();





        for (int i = 0; i < pisos.Length; i++)
        {
            pisos[i].orientacioFisica = pe�a.Tiles[i].OrientacioFisica;
        }

        altures = new int[] {1,1,1 };

        pe�a.enCrearDetalls += Crear;
    }


    private void OnDisable()
    {
        pe�a.enCrearDetalls -= Crear;
    }



    void Crear()
    {
        for (int i = 0; i < pe�a.Tiles.Length; i++)
        {
            if (pe�a.Tiles[i].TileFisic == gameObject)
            {
                indexTile = i;
                break;
            }
        }
        tile = pe�a.Tiles[indexTile];

        /*altures = new int[]
        {
            pe�a.CasesLength,
            pe�a.CasesLength,
            pe�a.CasesLength,
        }; */
        altures = new int[] { 1, 1, 1 };

        altures[0] = tile.Veins[0] != null ? tile.Veins[0].TileFisic.TryGetComponent(out Detall_Pisos pisosExt) ? pisosExt.altures[0] : 1 : 1;
        altures[1] = tile.Veins[1].TileFisic.TryGetComponent(out Detall_Pisos pisosEsq) ? pisosEsq.altures[2] : 1;
        altures[2] = tile.Veins[2].TileFisic.TryGetComponent(out Detall_Pisos pisosDre) ? pisosDre.altures[1] : 1;


        Debug.Log($"({pe�a.name}) {pe�a.CasesLength}");
        for (int i = 0; i < pisos.Length; i++)
        {
            pisos[i].Crear(altures[pisos[i].orientacioCasa], pe�a.Tiles[indexTile]);
        }
    }

    private void OnValidate()
    {
        if (pisos == null || pisos.Length == 0) pisos = GetComponentsInChildren<Detall_Pis>();
    }
}
