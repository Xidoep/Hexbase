using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Habitant : System.Object
{
    public Habitant(Peça casa)
    {
        this.casa = casa;
        ocupat = false;
    }

    public void Ocupar(Peça feina)
    {
        this.feina = feina;
        ocupat = true;
    }

    public Peça casa;
    public bool ocupat = false;
    public Peça feina;
}
