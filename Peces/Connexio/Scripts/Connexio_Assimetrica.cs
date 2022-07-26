using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Connexions/Assimetrica")]
public class Connexio_Assimetrica : Connexio
{
    [SerializeField] Connexio viable;
    public override Connexio Viable => viable;
}
