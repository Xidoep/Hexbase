using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Informacio")]
public class Informacio : ScriptableObject
{
    [Apartat("INFROMACIO")]
    [SerializeField] UI_InformacioPeça prefab;
    [SerializeField] Visualitzacions visualitzacions;

    protected List<Unitat> ui;

    protected GameObject Prefab => prefab.gameObject;
    protected int quantitat = 0;

    public virtual Unitat[] Mostrar(Peça peça, bool mostrarProveides = false) => null;
    public Unitat[] Amagar(Unitat[] elements)
    {
        if (elements == null || elements.Length == 0)
            return new Unitat[0];

        for (int i = 0; i < elements.Length; i++)
        {
            Destroy(elements[i].gameObject, 0.51f);
        }
        return new Unitat[0];
    }

    //protected Vector3 DesplaçamentLateral(int all, int i) => ((Camera.main.transform.right * (((i + 1) / 2) * 0.5f)) * (i % 2 == 0 ? 1 : -1)) + (Vector3.down * (i * i * 0.025f));
    protected Vector3 DesplaçamentLateral(int all, int i) => X(all, i);

    Vector3 X(int all, int i) => Camera.main.transform.right * -(Mathf.Max((all - 1),0) * 0.25f) + Camera.main.transform.right * (i * 0.5f);
    Vector3 Z(int all, int i) => Vector3.down * (i * i * 0.025f);
    protected float Rotacio(int i) => (((i + 1) / 2) * 12f) * (i % 2 == 0 ? -1 : 1);


    [System.Serializable]
    public struct Unitat
    {
        public Unitat(GameObject gameObject, int index)
        {
            this.gameObject = gameObject;
            this.index = index;
        }
        public GameObject gameObject;
        public int index;
    }
}
