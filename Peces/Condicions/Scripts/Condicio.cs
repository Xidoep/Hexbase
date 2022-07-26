using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// La base de les condicions.
/// Cada un d'aquesta te 
/// </summary>
public abstract class Condicio : ScriptableObject
{
    [Tooltip("L'subestat al que canviarà quan es compleixi la condiciò.")][SerializeField] protected Subestat objectiu;


    /// <summary>
    /// Funcio virtual sobreescrite per la resta de condicions, que comprova si es compleix la condicio.
    /// Es cridada desde Proximitat.
    /// 
    /// IMPORTANT: Les funcions subscrites han de cridar Canviar quan el resultat sigui positiu.
    /// </summary>
    public abstract bool Comprovar(Peça peça);

    /// <summary>
    /// Canviar es la funcio comuna que es crida a al confirmar la condicio, a la funcio Comprovar.
    /// </summary>
    protected void Canviar(Peça peça) => peça.CanviarSubestat(objectiu);
}
