using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class NoiseWorldPosBased : MonoBehaviour
{
    [SerializeField] Transform transform;



    void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        transform.localScale = (Vector3.one * Mathf.RoundToInt(Mathf.PerlinNoise(transform.position.x * 0.3f, transform.position.z * 0.3f)) * 0.5f);
    }
}
