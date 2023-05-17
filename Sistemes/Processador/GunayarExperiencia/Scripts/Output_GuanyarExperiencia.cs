using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Productes/Puntuacio")]
public class Output_GuanyarExperiencia : ScriptableObject, IProcessable
{
    [SerializeScriptableObject][SerializeField] Nivell nivell;
    [SerializeField] int puntuacio;

    //[Apartat("Auto-configurable")]
    //[SerializeField] Fase_Resoldre resoldre;

    System.Action<Peça, int> enPuntuar;
    public System.Action<Peça, int> EnPuntuar { get => enPuntuar; set => enPuntuar = value; }

    public void Processar(Peça peça)
    {
        nivell.GuanyarExperiencia(puntuacio, 2);
        enPuntuar(peça, puntuacio);
    }


    private void OnValidate()
    {
        if (nivell == null) nivell = XS_Utils.XS_Editor.LoadAssetAtPath<Nivell>("Assets/XidoStudio/Hexbase/Sistemes/Processos/Nivell.asset");
    }
}
