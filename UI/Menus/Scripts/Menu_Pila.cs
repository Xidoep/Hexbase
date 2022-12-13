using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_Pila : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] Transform parent;
    [SerializeField] Fase_Colocar colocar;
    [SerializeField] Estat[] disponibles;
    [SerializeField] int quantitatInicial;

    [Apartat("Animacions")]
    [SerializeField] Animacio_Scriptable primeraPosicio;
    [SerializeField] Animacio_Scriptable segonaPosicio;
    [SerializeField] Animacio_Scriptable segonaPosicioParent;
    [SerializeField] Animacio_Scriptable colocarPe�a;
    [SerializeField] Animacio_Scriptable colocarPe�aParent;

    [Apartat("EVENTS")]
    [SerializeField] Canal_Integre EnPujarNivell;

    [Apartat("Proves")]
    public Estat prova;

    List<UI_Peca> creades;

    private void OnEnable()
    {
        if (creades != null)
            return;

        creades = new List<UI_Peca>();
        for (int i = 0; i < quantitatInicial; i++)
        {
            AddPe�a(disponibles[Random.Range(0, disponibles.Length)]);
        }

        ResaltarSuperior();
        SepararSuperiors();

        colocar.OnFinish += RemovePe�a;
        EnPujarNivell.Registrar(AddPecesPerNivell);
    }

    [ContextMenu("Add")]
    void Add() => AddPe�a(prova);

    public void AddPecesPerNivell(int nivell)
    {
        for (int i = 0; i < (nivell / 2) * 10; i++)
        {
            AddPe�a(disponibles[Random.Range(0, disponibles.Length)]);
        }
    }

    public void AddPe�a(Estat estat)
    {
        GameObject parent = Instantiate(prefab, this.parent);
        parent.transform.position = Vector3.zero;
        parent.GetComponent<RectTransform>().sizeDelta = new Vector2(170, 20);
        parent.transform.localScale = new Vector3(1, 0.2f, 1);

        GameObject pe�a = Instantiate(estat.Prefag, Vector3.zero, Quaternion.identity, parent.transform);

        RectTransform rect = parent.GetComponent<RectTransform>();
        rect.anchoredPosition3D = Vector3.zero;

        pe�a.transform.localScale = new Vector3(100, 100, 100);
        pe�a.transform.localRotation = Quaternion.Euler(0, 0, 0);

        UI_Peca uiPeca = pe�a.GetComponent<UI_Peca>();

        creades.Add(uiPeca);
    }

    [ContextMenu("Remove")]
    void RemovePe�a()
    {
        colocarPe�a.Play(creades[0].transform);
        colocarPe�aParent.Play(creades[0].transform.parent.GetComponent<RectTransform>());
        StartCoroutine(RemovePe�aTemps(creades[0]));
        
        creades.RemoveAt(0);
        ResaltarSuperior();
    }

    IEnumerator RemovePe�aTemps(UI_Peca pe�a)
    {
        yield return new WaitForSeconds(0.51f);
        Destroy(pe�a.transform.parent.gameObject);
    }

    void ResaltarSuperior()
    {
        if (creades.Count > 0)
        {
            creades[0].Mostrar();
            creades[0].Seleccionar();
            primeraPosicio.Play(creades[0].transform);
        }
        if(creades.Count > 1)
        {
            segonaPosicio.Play(creades[1].transform);
            segonaPosicioParent.Play(creades[1].transform.parent.GetComponent<RectTransform>());
        }
    }

    void SepararSuperiors()
    {
        /*if(creades.Count > 0)
            creades[0].transform.position = new Vector3()
        if(creades.Count > 1)*/
    }
}
