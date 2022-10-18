using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Outline : MonoBehaviour
{
    [SerializeField] Canal_Void amagarEvent;

    private void OnEnable()
    {
        amagarEvent.Registrar(Amagar);
    }

    void Amagar() => gameObject.SetActive(false);
}
