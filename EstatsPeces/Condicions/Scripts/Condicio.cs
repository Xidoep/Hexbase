using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condicio : ScriptableObject
{
    protected Subestat objectiu;
    public virtual bool Comprovar(Hexagon pe�a) => false;

    protected void Canviar(Hexagon pe�a) => pe�a.CanviarSubestat(objectiu);
}
