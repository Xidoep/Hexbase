using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detall_Pisos : Detall
{
    [SerializeField] Pe�a pe�a;
    [SerializeField] Detall_Pis[] pisos;
    [SerializeField] int indexTile = -1;

    private void OnEnable()
    {
        if (pe�a == null) pe�a = GetComponentInParent<Pe�a>();
        if (pisos == null || pisos.Length == 0) pisos = GetComponentsInChildren<Detall_Pis>();
        for (int i = 0; i < pe�a.Tiles.Length; i++)
        {
            if(pe�a.Tiles[i].TileFisic == gameObject)
            {
                indexTile = i;
                break;
            }
        }
        for (int i = 0; i < pisos.Length; i++)
        {
            pisos[i].orientacioFisica = pe�a.Tiles[i].OrientacioFisica;
        }
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

        for (int i = 0; i < pisos.Length; i++)
        {
            pisos[i].Crear(pe�a.CasesLength, pe�a.Tiles[indexTile]);
        }
    }

    private void OnValidate()
    {
        if (pisos == null || pisos.Length == 0) pisos = GetComponentsInChildren<Detall_Pis>();
    }
}
