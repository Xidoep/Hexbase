using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Informacio")]
public class Informacio : ScriptableObject
{
    [Apartat("INFROMACIO")]
    [SerializeField] UI_InformacioPe�a prefab;
    [SerializeField] Visualitzacions visualitzacions;

    //protected List<Unitat> ui;

    protected GameObject Prefab => prefab.gameObject;
    protected int quantitat = 0;

    public virtual void Mostrar(Pe�a pe�a, bool mostrarProveides = false) { }
    public virtual void Amagar(Pe�a pe�a) { }









    //protected Vector3 Despla�amentLateral(int all, int i) => ((Camera.main.transform.right * (((i + 1) / 2) * 0.5f)) * (i % 2 == 0 ? 1 : -1)) + (Vector3.down * (i * i * 0.025f));
    protected Vector3 Despla�amentLateral(Transform parent, int all, int i) => X(parent, all, i) + Z(all,i);

    Vector3 X(Transform parent, int all, int i) => parent.right * -(Mathf.Max((all - 1),0) * 0.125f) + parent.right * (i * 0.25f);
    Vector3 Z(int all, int i) => Vector3.down * (0.25f) + Vector3.down * (Mathf.Min((Mathf.Min(i,(all-1)-i)),2) * 0.125f);
    protected float Rotacio(int i) => (((i + 1) / 2) * 12f) * (i % 2 == 0 ? -1 : 1);


    [System.Serializable]
    public struct Unitat
    {
        public Unitat(GameObject gameObject)
        {
            this.gameObject = gameObject;
            //this.index = index;
        }
        public GameObject gameObject;
        //public int index;
    }
}
