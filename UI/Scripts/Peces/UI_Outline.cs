using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Outline : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] AnimacioPerCodi animacio;


    public void Amagar() 
    {
        if (!meshRenderer.gameObject.activeSelf)
            return;

        animacio.Play(meshRenderer);
    }
    public void Mostrar()
    {
        gameObject.SetActive(true);
    }

}
