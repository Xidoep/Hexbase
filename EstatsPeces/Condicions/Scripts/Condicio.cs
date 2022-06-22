using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condicio : ScriptableObject
{
    protected Subestat objectiu;
    public virtual bool Comprovar(Hexagon peça) => false;

    protected void Canviar(Hexagon peça) => peça.CanviarSubestat(objectiu);
}
