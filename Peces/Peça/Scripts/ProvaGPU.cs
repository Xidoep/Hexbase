using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

public class ProvaGPU : MonoBehaviour
{
    public GameObject prefab;
    [Space(10)]
    public ComputeBuffer graphicsBuffer;
    public List<XS_InstantiateGPU.Grafic> grafics;

    GameObject tmp2;

    private void Start()
    {
        GameObject tmp = Instantiate(prefab, transform.position + Vector3.right * -1, transform.rotation);
        tmp.AddGrafics();

        XS_Coroutine.StartCoroutine_Ending(5, () =>
         {
             tmp2 = Instantiate(prefab, transform.position, transform.rotation);
             tmp2.AddGrafics();
         });

        XS_Coroutine.StartCoroutine_Ending(10, () =>
        {
            GameObject tmp3 = Instantiate(prefab, transform.position + Vector3.right * 1, transform.rotation);
            tmp3.AddGrafics();
        });
        XS_Coroutine.StartCoroutine_Ending(15, () =>
        {
            tmp2.RemoveGrafic();
        });
    }

    private void Update()
    {
        XS_InstantiateGPU.Render();
    }
}
