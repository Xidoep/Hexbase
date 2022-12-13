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
    [SerializeField] Animacio_Scriptable colocarPeça;
    [SerializeField] Animacio_Scriptable colocarPeçaParent;

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
            AddPeça(disponibles[Random.Range(0, disponibles.Length)]);
        }

        ResaltarSuperior();
        SepararSuperiors();

        colocar.OnFinish += RemovePeça;
        EnPujarNivell.Registrar(AddPecesPerNivell);
    }

    [ContextMenu("Add")]
    void Add() => AddPeça(prova);

    public void AddPecesPerNivell(int nivell)
    {
        for (int i = 0; i < (nivell / 2) * 10; i++)
        {
            AddPeça(disponibles[Random.Range(0, disponibles.Length)]);
        }
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
        colocarPeça.Play(creades[0].transform);
        colocarPeçaParent.Play(creades[0].transform.parent.GetComponent<RectTransform>());
        StartCoroutine(RemovePeçaTemps(creades[0]));
        
        creades.RemoveAt(0);
        ResaltarSuperior();
    }

    IEnumerator RemovePeçaTemps(UI_Peca peça)
    {
        yield return new WaitForSeconds(0.51f);
        Destroy(peça.transform.parent.gameObject);
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
