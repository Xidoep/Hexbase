using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AccionsCondicionalsFase : MonoBehaviour
{
    [SerializeField] FasesControlador fasesControlador;
    [SerializeField] Fase[] cohincidencies;
    [Space(20)]
    [SerializeField] UnityEvent enCohincidir;
    [Apartat("Opcions")]
    [SerializeField] bool cohincidir = true;

    bool cohincidida = false;

    private void OnEnable()
    {
        cohincidida = false;
        for (int i = 0; i < cohincidencies.Length; i++)
        {
            if (fasesControlador.EstaEnFase(cohincidencies[i]))
            {
                cohincidida = true;
                break;
            }
        }

        if(cohincidida == cohincidir)
        {
            enCohincidir.Invoke();
        }
    }

    private void OnValidate()
    {
        fasesControlador = XS_Utils.XS_Editor.LoadAssetAtPath<FasesControlador>("Assets/XidoStudio/Hexbase/Sistemes/Fases/_Controlador.asset");
    }
}
