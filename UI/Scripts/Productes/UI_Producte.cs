using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Producte : MonoBehaviour
{
    const string ICONE = "_Icone";
    //const string COVERTA = "_Coverta";
    const string GASTADA = "_Gastada";
    //const string PTENCIAL = "_Potencial";
    const string START_TIME = "_StartTime";

    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Image image;

    [Apartat("Debug")]
    [SerializeField] Pe�a pe�a;

    public GameObject Setup(Pe�a pe�a, int index)
    {
        this.pe�a = pe�a;

        //meshRenderer.material.SetTexture(ICONE, pe�a.ProductesExtrets[index].producte.Icone);
        //meshRenderer.material.SetFloat(GASTADA, pe�a.ProductesExtrets[index].gastat ? 1 : 0);
        //meshRenderer.material.SetFloat(PTENCIAL, pe�a.Connectat ? 0 : 1);
        //meshRenderer.material.SetFloat(START_TIME, Time.time + 1000);

        return gameObject;
    }
    public GameObject Setup(Producte producte, bool gastat)
    {
        if (meshRenderer)
        {
            meshRenderer.material.SetTexture(ICONE, producte.Icone);
            meshRenderer.material.SetFloat(GASTADA, gastat ? 1 : 0);
        }
        if (image)
        {
            image.sprite = producte.Sprite;
        }

        return gameObject;
    }


    public void Destruir(float time)
    {
        //meshRenderer.material.SetFloat(START_TIME, Time.time + time);
        Destroy(gameObject);
    }

    //Debug
    //ProducteExtret producte;
    //Subestat subestat;
}
