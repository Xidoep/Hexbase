using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

public class ProvaGPU : MonoBehaviour
{
    public List<XS_InstantiateGPU.Grafic> grafics;
    public GameObject prefab;
    public Quaternion quaternion;

    private void Start()
    {
        GameObject tmp = Instantiate(prefab, transform.position, transform.rotation);
        quaternion = transform.rotation;
        tmp.AddGrafics(grafics);
        //GameObject tmp = XS_InstantiateGPU.Instantiate(prefab);
        //tmp.transform.position = transform.position;
        //tmp.transform.rotation = transform.rotation;
    }

    private void Update()
    {
        XS_InstantiateGPU.RenderUpdate(grafics);
    }
}
