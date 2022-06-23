using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Detall : System.Object
{
    [SerializeField] GameObject detall;
    [SerializeField] Detall_Tiles tiles;

    public GameObject GameObject => detall;
    public int[] Tiles => tiles.Get();
}
