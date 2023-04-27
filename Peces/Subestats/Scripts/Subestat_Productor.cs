using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Productor")]
public class Subestat_Productor : Subestat, IProcessable
{
    public override Subestat Setup(Peça peça)
    {
        //produccio.AddProductor(peça);

        return base.Setup(peça);
    }

    new public void Processar(Peça peça)
    {
        Debug.Log($"PROCESSAR SUBESTAT PRODUCTOR {this.name}");
        peça.IntentarConnectar();
        base.Processar(peça);
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
