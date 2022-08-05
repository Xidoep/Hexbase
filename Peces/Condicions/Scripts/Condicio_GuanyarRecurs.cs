using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Condicio/byRecurs")]
public class Condicio_GuanyarRecurs : Condicio
{
    public override bool Comprovar(Peça peça, Proximitat proximitat)
    {
        //Mirar les cases i mirar si una de les cases ha guanyat algun recurs...
        //...

        //No pot anar per subestat. Hi ha més d'una casa a casa estat.
        return false;
    }
}
