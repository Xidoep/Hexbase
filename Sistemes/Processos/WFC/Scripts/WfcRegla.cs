using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WfcRegla : ScriptableObject
{
    public abstract Subestat Subestat { get; }
    public bool Comprovar(Peça peça)
    {
        if (!peça.SubestatIgualA(Subestat))
            return true;
        else return Comprovacio(peça);
    }

    protected abstract bool Comprovacio(Peça peça);
}
