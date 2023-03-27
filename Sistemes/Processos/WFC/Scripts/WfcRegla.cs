using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WfcRegla : ScriptableObject
{
    public abstract Subestat Subestat { get; }
    public bool Comprovar(Pe�a pe�a)
    {
        if (!pe�a.SubestatIgualA(Subestat))
            return true;
        else return Comprovacio(pe�a);
    }

    protected abstract bool Comprovacio(Pe�a pe�a);
}
