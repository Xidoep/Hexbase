using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Acces_NivellPecesOrigen : MonoBehaviour
{
    public static Transform nivellPecesOrigen;

    private void OnEnable()
    {
        nivellPecesOrigen = transform;
    }
}
