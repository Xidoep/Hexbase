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

    System.Action<Pe�a, int> enPuntuar;
    public System.Action<Pe�a, int> EnPuntuar { get => enPuntuar; set => enPuntuar = value; }

    public void Processar(Pe�a pe�a)
    {
        nivell.GuanyarExperiencia(puntuacio, 2);
        enPuntuar(pe�a, puntuacio);
    }


    private void OnValidate()
    {
        if (nivell == null) nivell = XS_Utils.XS_Editor.LoadAssetAtPath<Nivell>("Assets/XidoStudio/Hexbase/Sistemes/Processos/Nivell.asset");
    }
}
