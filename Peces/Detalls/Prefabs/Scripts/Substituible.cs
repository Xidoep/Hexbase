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
        if (transform == null)
            return;

        //XS_Instantiate
        //Instantiate(alternatives[Random.Range(0, alternatives.Length)], transform.position, transform.rotation, transform.localScale, transform.parent);
        alternatives[Random.Range(0, alternatives.Length)].Instantiate(transform.position, transform.rotation, transform.localScale, transform.parent).layer = transform.gameObject.layer;

        Destroy(transform.gameObject);
        //Destroy(this);
    }
    public GameObject SubstituirRetorn(Transform transform)
    {
        GameObject nou = alternatives[Random.Range(0, alternatives.Length)].Instantiate(transform.position, transform.rotation, transform.localScale, transform.parent);
        nou.layer = transform.gameObject.layer;

        Destroy(transform.gameObject);

        return nou;
    }
}
