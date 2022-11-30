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
        MonoBehaviour.Instantiate(alternatives[Random.Range(0, alternatives.Length)], transform.position, transform.rotation, transform.parent);
        Destroy(transform.gameObject);
    }
}
