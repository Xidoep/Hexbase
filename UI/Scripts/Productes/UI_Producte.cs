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
    [SerializeField] Peça peça;

    public GameObject Setup(Peça peça, int index)
    {
        this.peça = peça;

        //meshRenderer.material.SetTexture(ICONE, peça.ProductesExtrets[index].producte.Icone);
        //meshRenderer.material.SetFloat(GASTADA, peça.ProductesExtrets[index].gastat ? 1 : 0);
        //meshRenderer.material.SetFloat(PTENCIAL, peça.Connectat ? 0 : 1);
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
