using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_CameraMenu_Access : MonoBehaviour
{
    static Camera cameraMenu;
    private void OnEnable()
    {
        cameraMenu = gameObject.GetComponent<Camera>();
    }
    public static Camera CameraMenu => cameraMenu;
}
