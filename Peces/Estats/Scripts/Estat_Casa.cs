using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Estats/Casa")]
public class Estat_Casa : Estat
{
    [SerializeField] Recurs[] necessitats;
    //[SerializeField] Condicio_GuanyarRecurs condicioGuanyarRecurs;


    public override bool EsCasa => true;

    public Recurs[] Necessitats => necessitats;
    //public Condicio_GuanyarRecurs CondicioGuanyarRecurs => condicioGuanyarRecurs;
}
