using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotacioAleatoria : MonoBehaviour
{
    [SerializeField] Vector3 minim;
    [SerializeField] Vector3 maxim;

    private void OnEnable()
    {
        transform.localRotation = Quaternion.Euler(
            Random.Range(minim.x, maxim.x), 
            Random.Range(minim.y, maxim.y), 
            Random.Range(minim.z, maxim.z)
            );
    }
}
