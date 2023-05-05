using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_Pila : MonoBehaviour
{
    [Apartat("FROM PROJECT")]
    [SerializeField] GameObject prefab;
    [Apartat("FROM HIERARCHY")]
    [SerializeField] Fase_Resoldre resoldre;
    [SerializeField] Transform parent;
    [SerializeField] PoolPeces pool;
    [SerializeField] Visualitzacions visualitzacions;

    List<UI_Peca> creades;

    void OnEnable()
    {
        if (creades != null)
            return;

        pool.EnAfegir += AddPeça;
        pool.EnTreure += RemovePeça;
        resoldre.EnTornar += Amagar;
        resoldre.EnRepetir += Amagar;
        resoldre.EnContinuar += Amagar;

        //Crear les peces que hi ha al pool, ja que aquest s'haurà creat abans que el menu.
        creades = new List<UI_Peca>();
        for (int i = 0; i < pool.Quantitat; i++)
        {
            AddPeça(pool.Peça(i));
        }

        ResaltarISepararSuperior();
    }

    void OnDisable()
    {
        pool.EnAfegir -= AddPeça;
        pool.EnTreure -= RemovePeça;
        resoldre.EnTornar -= Amagar;
        resoldre.EnRepetir -= Amagar;
        resoldre.EnContinuar -= Amagar;

        pool.Reset();
    }

    public void Amagar()
    {
        Destroy(this.gameObject);
    }

    public void AddPeça(Estat estat)
    {
        GameObject parent = Instantiate(prefab, this.parent);
        parent.transform.position = Vector3.zero;
        parent.GetComponent<RectTransform>().sizeDelta = new Vector2(170, 20);
        parent.transform.localScale = new Vector3(1, 0.2f, 1);

        GameObject peça = Instantiate(estat.Prefag, Vector3.zero, Quaternion.identity, parent.transform);

        RectTransform rect = parent.GetComponent<RectTransform>();
        rect.anchoredPosition3D = Vector3.zero;

        peça.transform.localScale = new Vector3(100, 100, 100);
        peça.transform.localRotation = Quaternion.Euler(0, 0, 0);

        UI_Peca uiPeca = peça.GetComponent<UI_Peca>();

        creades.Add(uiPeca);
    }

    [ContextMenu("Remove")]
    void RemovePeça()
    {
        visualitzacions.ColocarPeça(creades[0].transform);
        StartCoroutine(RemovePeçaTemps(creades[0]));
        
        creades.RemoveAt(0);
        ResaltarISepararSuperior();
    }

    IEnumerator RemovePeçaTemps(UI_Peca peça)
    {
        yield return new WaitForSeconds(0.51f);
        Destroy(peça.transform.parent.gameObject);
    }

    void ResaltarISepararSuperior()
    {
        if (creades.Count > 0)
        {
            creades[0].Resaltar();
            creades[0].Seleccionar();
            visualitzacions.PrimeraPosicio(creades[0].transform);
        }
        if(creades.Count > 1)
        {
            visualitzacions.PrimeraPosicio(creades[0].transform);
            visualitzacions.SegonaPosicio(creades[1].transform);
        }
    }

}
