using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Outline : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    //[SerializeField] Canal_Void amagarEvent;
    [SerializeField] Animacio_Scriptable animacio;

    private void OnEnable()
    {
        //Registrar();
    }

    //void Amagar() => gameObject.SetActive(false);
    public void Amagar() 
    {
        animacio.Play(meshRenderer);
        //Desregistrar();
    }

    private void OnDestroy()
    {
        //Desregistrar();
    }

    //public void Registrar() => amagarEvent.Registrar(Amagar);
    //public void Desregistrar() => amagarEvent.Desregistrar(Amagar);
}
