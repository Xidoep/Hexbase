using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class Menu_Pila : MonoBehaviour
{
    [Apartat("FROM PROJECT")]
    [SerializeField] GameObject prefab;
    [Apartat("FROM HIERARCHY")]
    [SerializeField] Fase_Resoldre resoldre;
    [SerializeField] Transform parent;
    [SerializeField] PoolPeces pool;
    [Space(10)]
    [SerializeField] TMP_Text numero;
    [SerializeField] Image bombolla;

    [FoldoutGroup("Animacions"), SerializeField, SerializeScriptableObject] AnimacioPerCodi animacio_afegirPeces;
    [FoldoutGroup("Animacions"), SerializeField, SerializeScriptableObject] AnimacioPerCodi animacio_poquesPeces;

    List<UI_Peca> creades;

    bool PoquesPeces => pool.Quantitat <= 5;

    /*
    System.Action<Transform> enDesapareixre;
    System.Action<Transform> enPosicio1;
    System.Action<Transform> enPosicio2;

    public System.Action<Transform> EnDesapareixre { get => enDesapareixre; set => enDesapareixre = value; }
    public System.Action<Transform> EnPosicio1 { get => enPosicio1; set => enPosicio1 = value; }
    public System.Action<Transform> EnPosicio2 { get => enPosicio2; set => enPosicio2 = value; }
    */


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

        numero.text = pool.Quantitat.ToString();
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

        ((RectTransform)parent.transform).anchoredPosition3D = Vector3.zero;

        creades.Add((UI_Peca)(estat.Prefab.Crear().SetTransform(Vector3.zero, Vector3.zero, Vector3.one * 100, parent.transform)));
        ResaltarNumero();

        ActualitzarNumero();
    }

    void ResaltarNumero() => animacio_afegirPeces.Play(bombolla.transform);

    [ContextMenu("Remove")]
    void RemovePeça()
    {
        StartCoroutine(RemovePeçaTemps(creades[0]));
        
        creades.RemoveAt(0);
        ResaltarISepararSuperior();

        ActualitzarNumero();
    }

    IEnumerator RemovePeçaTemps(UI_Peca peça)
    {
        peça.Desapareixre();
        yield return new WaitForSeconds(0.51f);
        Destroy(peça.transform.parent.gameObject);
    }

    void ResaltarISepararSuperior()
    {
        if (creades.Count > 0)
        {
            creades[0].Resaltar();
            creades[0].Seleccionar();
            creades[0].Posicio1();
        }
        if(creades.Count > 1)
        {
            creades[1].Posicio2();
        }
    }

    void ActualitzarNumero()
    {
        numero.text = pool.Quantitat.ToString();

        bombolla.color = PoquesPeces ? (Color.red + Color.cyan * 0.25f) : Color.white;

        if (PoquesPeces)
            animacio_poquesPeces.Play(bombolla.transform);
    }

}
