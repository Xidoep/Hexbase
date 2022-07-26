using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat")]
public class Subestat : ScriptableObject
{
    [SerializeField] int punts;

    [Linia]
    [Nota("Els detalls intanciats sobre la peça")]
    [SerializeField] Detall[] detalls;

    [Linia]
    [Nota("Condicions per canviar d'estat")]
    [SerializeField] Condicio[] condicions;

    [Linia]
    [Header("RECURSOS")]
    [SerializeField] Produccio produccio;
    [SerializeField] Recurs recurs;




    public Condicio[] Condicions => condicions;
    public Detall[] Detalls => detalls;
    public Recurs[] Recursos(Peça productor) => recurs.Produir(productor);
    public int Punts => punts;
    public void Productor(Peça peça)
    {
        if (recurs != null) produccio.AddProductor(peça);
    }
    //public virtual Subestat Get(Peça peça) => this;





    private void OnValidate()
    {
        if (recurs == null)
            return;

        produccio = (Produccio)XS_Editor.LoadAssetAtPath<Produccio>("Assets/XidoStudio/Hexbase/Sistemes/Processos/Produccio.asset");
    }
}