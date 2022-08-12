using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XS_Utils;

public class UI_Peces : MonoBehaviour
{
    const float OFFSET_CAMERA_Y = -2.5f;
    const float OFFSET_CAMERA_Z = 20f;
    const float OFFSET_CAMERA_X = 2f;
    const float OFFSET_CAMERA_Rot = -30f;

    [SerializeField] AnimacioPerCodi_Base.Transformacions transformacio;
    [SerializeField] GameObject prefab;
    [SerializeField] Transform parent;
    [SerializeField] Fase_Colocar colocar;
    [SerializeField] Estat[] peces;

    //INTERN
    Transform[] childs;

    private void OnEnable()
    {
        for (int i = 0; i < peces.Length; i++)
        {
            

            //GameObject parent = new GameObject($"Peça{i + 1}", new System.Type[] {typeof(RectTransform), typeof(CanvasRenderer) });
            //parent.transform.SetParent(this.parent);
            GameObject parent = Instantiate(prefab, this.parent);

            parent.transform.position = Vector3.zero;
            //parent.transform.rotation = Quaternion.Euler(OFFSET_CAMERA_Rot, 0, 0);
            parent.GetComponent<UI_Seleccio>().Setup(peces[i],colocar);
            RectTransform rect = parent.GetComponent<RectTransform>();

            //rect.sizeDelta = Vector2.one * 170;
            //parent.AddComponent<Image>().color = Color.clear;
            //parent.transform.localScale = Vector3.one;
            //parent.AddComponent<UI_Seleccio>().Setup(peces[i],colocar);


            GameObject peça = Instantiate(peces[i].Prefag,
                //new Vector3(OFFSET_CAMERA_X * i, OFFSET_CAMERA_Y, OFFSET_CAMERA_Z),
                //posicions[i].ToWorldPosition(),
                Vector3.zero,
                Quaternion.identity,
                //Quaternion.Euler(OFFSET_CAMERA_Rot, 0, 0),
                parent.transform
                );

            rect.anchoredPosition3D = Vector3.zero;

            //peça.transform.SetParent(parent.transform);
            peça.transform.localScale = Vector3.one * 100;
            peça.AddComponent<AnimacioPerCodi>().Play(transformacio, AnimacioPerCodi_Base.Transicio_Tipus.loop, 4);
            //AnimacioPerCodi apc = peça.AddComponent<AnimacioPerCodi>().Play(transformacio);
            //apc.Add(transformacio);
            //apc.Play();
            //peça.AddComponent<BoxCollider>().size = new Vector3(2, 0.1f, 1.7f);

            //peça.transform.position = posicions[i].ToWorldPosition();
        }

        childs = transform.GetComponentsInChildren<Transform>();
        for (int i = 0; i < childs.Length; i++)
        {
            childs[i].gameObject.layer = 5;
        }
    }
}
