using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UI_Configuracio : MonoBehaviour
{
    [SerializeField] Utils_InstantiableFromProject popupReset;
    [SerializeField] UnityEvent enReset;

    public void PopupReset() 
    {
        popupReset.InstantiateReturn().GetComponent<Utils_EsdevenimentDelegat>().Registrar(Resetejar);
    }

    void Resetejar() 
    {
        enReset?.Invoke();
    } 
}
