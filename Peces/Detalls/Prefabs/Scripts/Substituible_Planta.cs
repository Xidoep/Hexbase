using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/Substituibles Planta")]
public class Substituible_Planta : MonoBehaviour
{
    [SerializeField] [Range(0, 2)] int costat;
    [SerializeField] Substituible planta;
    [SerializeField] Substituible top;

    public int Costat => costat;

    public GameObject Substituir(bool top)
    {
        if (!top)
            return planta.SubstituirRetorn(transform);
        else return this.top.SubstituirRetorn(transform);
    }

}
