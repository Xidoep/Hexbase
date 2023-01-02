using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Desplaçador : MonoBehaviour
{
    public RectTransform rect;
    public bool superior;
    public float quantitat = 0.5f;
    public UI_Desplaçador constrari;
    public bool desplaçar = false;

    public void Desplaçar()
    {
        desplaçar = true;
    }
    public void Aturar()
    {
        desplaçar = false;
    }

    void Update()
    {
        if (!desplaçar)
            return;

        quantitat -= Time.deltaTime * 2f;
        CanviarGrafics();

        constrari.quantitat = 1 - quantitat;
        constrari.CanviarGrafics();
    }


    public void CanviarGrafics()
    {
        if (superior)
        {
            rect.anchorMin = new Vector2(0, Mathf.Lerp(1, .4f, quantitat));
        }
        else
        {
            rect.anchorMax = new Vector2(1, Mathf.Lerp(0, .6f, quantitat));
        }
    }
}
