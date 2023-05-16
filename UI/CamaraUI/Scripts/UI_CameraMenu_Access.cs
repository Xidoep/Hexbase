using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_CameraMenu_Access : MonoBehaviour
{
    static Camera cameraMenu;
    static Canvas canvas;
    private void OnEnable()
    {
        cameraMenu = gameObject.GetComponent<Camera>();
        canvas = gameObject.GetComponentInChildren<Canvas>();
    }
    public static Camera CameraMenu => cameraMenu;
    public static Canvas Canvas => canvas;
}
