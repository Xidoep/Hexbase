using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Productor")]
public class Subestat_Productor : Subestat
{
    public override Subestat Setup(Peça peça)
    {
        produccio.AddProductor(peça);

        return base.Setup(peça);
    }



    [Apartat("RECURSOS")]
    [SerializeField] Produccio produccio;


    new void OnValidate()
    {
        produccio = (Produccio)XS_Editor.LoadAssetAtPath<Produccio>("Assets/XidoStudio/Hexbase/Sistemes/Processos/Produccio.asset");
        base.OnValidate();
    }
}
