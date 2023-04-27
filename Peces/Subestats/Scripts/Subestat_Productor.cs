using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Productor")]
public class Subestat_Productor : Subestat, IProcessable
{
    public override Subestat Setup(Pe�a pe�a)
    {
        //produccio.AddProductor(pe�a);

        return base.Setup(pe�a);
    }

    new public void Processar(Pe�a pe�a)
    {
        Debug.Log($"PROCESSAR SUBESTAT PRODUCTOR {this.name}");
        pe�a.IntentarConnectar();
        base.Processar(pe�a);
    }

    /*
    [Apartat("RECURSOS")]
    [SerializeField] Produccio produccio;


    private void OnValidate()
    {
        produccio = (Produccio)XS_Editor.LoadAssetAtPath<Produccio>("Assets/XidoStudio/Hexbase/Sistemes/Processos/Produccio.asset");
    }
    */
}
