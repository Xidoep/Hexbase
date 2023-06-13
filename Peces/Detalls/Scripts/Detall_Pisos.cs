using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detall_Pisos : Detall
{
    [SerializeField] Peça peça;
    [SerializeField] Detall_Pis[] pisos;
    [SerializeField] int indexTile = -1;

    private void OnEnable()
    {
        if (peça == null) peça = GetComponentInParent<Peça>();
        if (pisos == null || pisos.Length == 0) pisos = GetComponentsInChildren<Detall_Pis>();
        for (int i = 0; i < peça.Tiles.Length; i++)
        {
            if(peça.Tiles[i].TileFisic == gameObject)
            {
                indexTile = i;
                break;
            }
        }
        for (int i = 0; i < pisos.Length; i++)
        {
            pisos[i].orientacioFisica = peça.Tiles[i].OrientacioFisica;
        }
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

        for (int i = 0; i < pisos.Length; i++)
        {
            pisos[i].Crear(peça.CasesLength, peça.Tiles[indexTile]);
        }
    }

    private void OnValidate()
    {
        if (pisos == null || pisos.Length == 0) pisos = GetComponentsInChildren<Detall_Pis>();
    }
}
