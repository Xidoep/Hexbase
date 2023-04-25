using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Roductes/Puntuacio")]
public class GuanyarPuntuacio : ScriptableObject, IProcessable
{
    [SerializeField] int puntuacio;

    [Apartat("Auto-configurable")]
    [SerializeField] Fase_Resoldre resoldre;
    [SerializeField] Visualitzacions visualitzacions;

    public void Processar(Peça peça)
    {
        resoldre.Nivell.GuanyarExperiencia(puntuacio);
        visualitzacions.GuanyarExperiencia(peça.transform.position, puntuacio);
    }


    private void OnValidate()
    {
        if (resoldre == null) resoldre = XS_Utils.XS_Editor.LoadAssetAtPath<Fase_Resoldre>("Assets/XidoStudio/Hexbase/Sistemes/Fases/Resoldre.asset");
        if (visualitzacions == null) visualitzacions = XS_Utils.XS_Editor.LoadAssetAtPath<Visualitzacions>("Assets/XidoStudio/Hexbase/Sistemes/Visualitzacions/Visualitzacions.asset");
    }
}
