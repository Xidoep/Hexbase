using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Connexions/Viables")]
public class Connexio_Viables : Connexio
{
    [SerializeField] List<Connexio> viables;

    //public override bool EncaixaAmb(Connexio connexio) => viables.Contains(connexio);
}
