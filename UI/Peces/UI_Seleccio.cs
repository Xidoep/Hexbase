using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XS_Utils;

public class UI_Seleccio : MonoBehaviour
{
    protected const string SELECCIONAT_ID = "_Seleccionat";

    [SerializeField] MeshRenderer outline;

    public void Seleccionar()
    {
        outline.material.SetFloat(SELECCIONAT_ID, 1);
    }

    public void Deseleccionar()
    {
        outline.material.SetFloat(SELECCIONAT_ID, 0);
    }

}
