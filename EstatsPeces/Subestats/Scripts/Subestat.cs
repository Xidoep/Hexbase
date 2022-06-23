using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat")]
public class Subestat : ScriptableObject
{
    //public GameObject[] detalls;
    public Detall[] detalls;
    [SerializeField] Condicio[] condicions;

    public Condicio[] Condicions => condicions;

}



