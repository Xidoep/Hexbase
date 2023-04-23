using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Sumari")]
public class Sumari : ScriptableObject
{
    [SerializeField] Fase_Processar faseProcessar;
    [SerializeField] Produccio produccio;

    [SerializeField]

    void OnEnable()
    {
        faseProcessar.OnFinish += Mostrar;
    }
    void OnDisable()
    {
        faseProcessar.OnFinish -= Mostrar;
    }

    void Mostrar()
    {
        //Busca tots els pobles a Grups. i mostrar quants habitants te cada poble
        //o de moment només mostrar els habitants totals.

        //Buscar totes les necessitats no covertes dels habitants.

        //Buscar totes les extraccions i mostrar quins productes tenen productor pero no es gasten i quins productes no tenen productor.
    }
}
