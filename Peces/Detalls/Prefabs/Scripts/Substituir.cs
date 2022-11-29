using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CanEditMultipleObjects]
public class Substituir : MonoBehaviour
{
    [SerializeField] GameObject[] alternatives;
    private void OnEnable()
    {
        if (alternatives.Length == 0)
            return;

        Instantiate(alternatives[Random.Range(0, alternatives.Length)], transform.position, transform.rotation, transform.parent);
        Destroy(gameObject);
    }
}
