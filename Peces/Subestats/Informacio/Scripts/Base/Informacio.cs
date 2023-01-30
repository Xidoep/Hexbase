using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Informacio")]
public class Informacio : ScriptableObject
{

    public virtual void Mostrar(Peça peça, bool mostrarProveides = false) { }
    public virtual void Amagar(Peça peça) { }





    protected Vector3 DesplaçamentLateral(Transform parent, int all, int i) => X(parent, all, i) + Z(all,i);

    Vector3 X(Transform parent, int all, int i) => parent.right * -(Mathf.Max((all - 1),0) * 0.125f) + parent.right * (i * 0.25f);
    Vector3 Z(int all, int i) => /*Vector3.down * (0.25f) +*/ Vector3.down * (Mathf.Min((Mathf.Min(i,(all-1)-i)),2) * 0.125f);
    protected float Rotacio(int i) => (((i + 1) / 2) * 12f) * (i % 2 == 0 ? -1 : 1);





    [System.Serializable]
    public struct Unitat
    {
        public Unitat(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }
        public GameObject gameObject;
    }
}
