using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/GameObject/One")]
public class Detall_GameObject : ScriptableObject
{
    [SerializeField] protected GameObject[] detalls;
    public virtual GameObject Get(Pe�a pe�a) => detalls[0];
}
