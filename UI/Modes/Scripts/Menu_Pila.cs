using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

public class Menu_Pila : MonoBehaviour
{
    [Apartat("FROM PROJECT")]
    [SerializeField] GameObject prefab;
    [Apartat("FROM HIERARCHY")]
    [SerializeField] Fase_Resoldre resoldre;
    [SerializeField] Transform parent;
    [SerializeField] PoolPeces pool;

    List<UI_Peca> creades;



    System.Action<Transform> enDesapareixre;
    System.Action<Transform> enPosicio1;
    System.Action<Transform> enPosicio2;

    public System.Action<Transform> EnDesapareixre { get => enDesapareixre; set => enDesapareixre = value; }
    public System.Action<Transform> EnPosicio1 { get => enPosicio1; set => enPosicio1 = value; }
    public System.Action<Transform> EnPosicio2 { get => enPosicio2; set => enPosicio2 = value; }



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

    public void AddPeça(EstatColocable estat)
    {
        GameObject parent = Instantiate(prefab, this.parent);
        parent.transform.position = Vector3.zero;
        parent.GetComponent<RectTransform>().sizeDelta = new Vector2(170, 20);
        parent.transform.localScale = new Vector3(1, 0.2f, 1);

        //UI_Peca uiPeca = estat.Prefab.Crear();
        //uiPeca.SetTransform(Vector3.zero, Vector3.zero, Vector3.one * 100, parent.transform);

        //GameObject peça = Instantiate(estat.Prefag, Vector3.zero, Quaternion.identity, parent.transform);
        ((RectTransform)parent.transform).anchoredPosition3D = Vector3.zero;
        //parent.GetComponent<RectTransform>()
        //RectTransform rect = parent.GetComponent<RectTransform>();
        //rect.anchoredPosition3D = Vector3.zero;

        //peça.transform.localScale = new Vector3(100, 100, 100);
        //peça.transform.localRotation = Quaternion.Euler(0, 0, 0);

        //UI_Peca uiPeca = peça.GetComponent<UI_Peca>();

        //creades.Add(uiPeca);
        creades.Add((UI_Peca)(estat.Prefab.Crear().SetTransform(Vector3.zero, Vector3.zero, Vector3.one * 100, parent.transform)));
    }

    [ContextMenu("Remove")]
    void RemovePeça()
    {
        //visualitzacions.Desapareixre(creades[0].transform);
        enDesapareixre?.Invoke(creades[0].transform);
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
            enPosicio1?.Invoke(creades[0].transform);
            //visualitzacions.PrimeraPosicio(creades[0].transform);
        }
        if(creades.Count > 1)
        {
            enPosicio1?.Invoke(creades[0].transform);
            enPosicio2?.Invoke(creades[1].transform);
            //visualitzacions.PrimeraPosicio(creades[0].transform);
            //visualitzacions.SegonaPosicio(creades[1].transform);
        }
    }


}
