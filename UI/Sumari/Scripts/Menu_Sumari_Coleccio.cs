using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Menu_Sumari_Coleccio : MonoBehaviour
{
    [SerializeField] HorizontalLayoutGroup parent;
    [SerializeField] XS_Button parentBoto;
    [SerializeField] XS_Button boto;
    //[SerializeField] List<XS_Button> llista;
    //[SerializeField] List<Producte> productes;
    [SerializeField] List<UI_Producte> productes;
    [SerializeField] GameObject numeroParent;
    [SerializeField] TMP_Text numero;
    [Space(20)]
    [SerializeField] GameObject prefab_informacio;
    [SerializeField] bool esProducte;
    [SerializeField] UI_Producte prefab_producte;

    //INTERN
    AnimacioPerCodi_GameObject_Referencia informacio;
    bool mostrat;
    Coroutine coroutine;
    float factor;
    float actual;
    bool enable;

    float Spacing(int tamany) => Mathf.Max(-(Mathf.LerpUnclamped(10, 40, tamany / 15f)), -65);

    private void OnEnable()
    {
        enable = false;
        //parent.GetComponent<XS_Button>().OnEnter = Desresaltar;
        boto.enabled = productes.Count > 0;
    }


    public void Crear(List<Sumari.Informacio> nous)
    {
        if(productes == null)
            productes = new List<UI_Producte>();

        
        for(int b = productes.Count - 1; b >= 0; b--)
        {
            if (nous.Count == 0)
                break;

            for (int n = nous.Count - 1; n >= 0; n--)
            {
                if(productes[b].Iguals(nous[n]))
                //if(productes[b].Producte == nous[n].producte)
                {
                    productes[b].Keepit = true;
                    productes[b].SetPe�a = nous[n].pe�a;
                    nous.RemoveAt(n);
                }
            }
        }

        //tots els marcats amb keepIt, s�n els que s'han de mantenir.
        for (int i = productes.Count - 1; i >= 0; i--)
        {
            if (productes[i].Keepit)
            {
                productes[i].Keepit = false;
                continue;
            }
            productes[i].Borrar();
            productes.RemoveAt(i);
        }


        
        //Els nous que queden s�n els que s'han de sumar.
        for (int i = 0; i < nous.Count; i++)
        {
            UI_Producte _producte = Instantiate(prefab_producte, parent.transform).Setup(nous[i].pe�a, nous[i].producte, nous[i].index, Resaltar, Desresaltar, parentBoto.onClick.Invoke);
            productes.Add(_producte);
            if (!mostrat)
                _producte.gameObject.SetActive(false);
        }

        if (!mostrat)
        {
            numeroParent.SetActive(productes.Count > 0);
            numero.text = productes.Count.ToString("#0");
        }

        if(enable)
            boto.enabled = productes.Count > 0;

        parent.spacing = Spacing(productes.Count);

        enable = true;
    }

    IEnumerator Comprimir(bool comprimir)
    {
        factor = 0;
        actual = parent.spacing;

        if (comprimir)
        {
            while (parent.spacing < 0)
            {
                factor += Time.deltaTime * 0.75f;
                parent.spacing = Mathf.LerpUnclamped(actual, 0, factor);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (parent.spacing > Spacing(productes.Count))
            {
                factor += Time.deltaTime;
                parent.spacing = Mathf.LerpUnclamped(actual, Spacing(productes.Count), factor);
                yield return new WaitForEndOfFrame();
            }
            parent.spacing = Spacing(productes.Count);
        }
    }

    public void Mostrar(bool mostrar)
    {
        mostrat = mostrar;
        
        if (coroutine != null) 
            StopCoroutine(coroutine);

        if (mostrat)
        {
            for (int i = 0; i < productes.Count; i++)
            {
                productes[i].Mostrar();
            }

            coroutine = StartCoroutine(Comprimir(false));
        }
        else
        {
            for (int i = 0; i < productes.Count; i++)
            {
                productes[i].Amagar();
            }
            numeroParent.SetActive(productes.Count > 0);
            numero.text = productes.Count.ToString("#0");

            coroutine = StartCoroutine(Comprimir(true));
        }
    }


    void Resaltar(Pe�a pe�a, Producte producte) => pe�a.ResaltarInformacio();
    void Desresaltar(Pe�a pe�a) => pe�a.DesresaltarInformacio();

}
