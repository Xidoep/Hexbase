using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detall_Tiles : ScriptableObject
{
    public virtual int[] Get(Peça peça) => new int[] { 0 };
}
