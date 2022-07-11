using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Habitant : System.Object
{
    public Habitant(Pe�a casa)
    {
        this.casa = casa;
        ocupat = false;
    }

    public void Ocupar(Pe�a feina)
    {
        this.feina = feina;
        ocupat = true;
    }

    public Pe�a casa;
    public bool ocupat = false;
    public Pe�a feina;
}
