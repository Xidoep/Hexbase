using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Estats/Casa")]
public class Estat_Casa : Estat
{
    [Apartat("CASA")]
    [SerializeField] Producte[] necessitats;

    public Producte[] Necessitats => necessitats;
}
