using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detall_Arbre : Detall
{
    public override void Setup(string[] args)
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);
        transform.localScale =
            Vector3.right * Random.Range(.8f, 1.2f) +
            Vector3.up * Random.Range(.8f, 1.2f) +
            Vector3.forward * Random.Range(.8f, 1.2f);
    }
}
