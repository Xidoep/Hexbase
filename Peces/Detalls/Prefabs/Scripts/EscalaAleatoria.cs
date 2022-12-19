using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscalaAleatoria : MonoBehaviour
{
    [SerializeField] Vector3 minim;
    [SerializeField] Vector3 maxim;
    [Space(10)]
    [Nota("Això ferà que el minim/maxim es sumi a l'escala actual")]
    [SerializeField] bool relatiu;

    private void OnEnable()
    {
        if (relatiu)
        {
            transform.localScale = new Vector3(
            transform.localScale.x + Random.Range(minim.x, maxim.x),
            transform.localScale.y + Random.Range(minim.y, maxim.y),
            transform.localScale.z + Random.Range(minim.z, maxim.z)
            );
        }
        else
        {
            transform.localScale = new Vector3(
           Random.Range(minim.x, maxim.x),
           Random.Range(minim.y, maxim.y),
           Random.Range(minim.z, maxim.z)
           );
        }
        
    }
}
