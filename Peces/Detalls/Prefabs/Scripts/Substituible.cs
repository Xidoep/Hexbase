using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/Substituibles")]
public class Substituible : ScriptableObject
{
    [SerializeField] GameObject[] alternatives;

    public void Substituir(Transform transform)
    {
        alternatives[Random.Range(0, alternatives.Length)].Instantiate(transform.position, transform.rotation, transform.localScale, transform.parent);
        Destroy(transform.gameObject);
    }
}
