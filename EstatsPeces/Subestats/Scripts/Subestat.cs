using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat")]
public class Subestat : ScriptableObject
{
    [SerializeField] Condicio[] condicions;

    public Condicio[] Condicions => condicions;
}



public class Condicio : ScriptableObject
{
    public Subestat objectiu;
    public virtual bool Comprovar(Hexagon peça) => false;
}