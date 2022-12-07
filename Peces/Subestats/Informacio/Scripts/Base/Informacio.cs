using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Informacio")]
public class Informacio : ScriptableObject
{
    [Apartat("INFROMACIO")]
    [SerializeField] UI_InformacioPeça prefab;
    [SerializeField] Animacio_Scriptable animacioAmagar;

    protected GameObject Prefab => prefab.gameObject;


    public virtual GameObject[] Mostrar(Peça peça) => null;
    public GameObject[] Amagar(GameObject[] elements)
    {
        if (elements.Length == 0)
            return new GameObject[0];

        for (int i = 0; i < elements.Length; i++)
        {
            animacioAmagar.Play(elements[i]);
            Destroy(elements[i], 0.51f);
        }
        return new GameObject[0];
    }

    protected Vector3 DesplaçamentLateral(int i) => ((Camera.main.transform.right * (((i + 1) / 2) * 0.5f)) * (i % 2 == 0 ? 1 : -1)) + (Vector3.down * (i * i * 0.025f));
    protected float Rotacio(int i) => (((i + 1) / 2) * 12f) * (i % 2 == 0 ? -1 : 1);

}
