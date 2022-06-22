using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat")]
public class Subestat : ScriptableObject
{
    public GameObject[] detalls;
    [SerializeField] Condicio[] condicions;
    [SerializeField] 

    public Condicio[] Condicions => condicions;
}



