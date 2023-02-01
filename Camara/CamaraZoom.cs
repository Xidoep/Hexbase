using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using XS_Utils;
using Cinemachine;

[System.Serializable]

public class CamaraZoom
{
    [SerializeField] CinemachineVirtualCamera vCamera;
    [Space(10)]
    [SerializeField] InputActionReference keyboard;
    [SerializeField] InputActionReference mouse;
    [Space(10)]
    [SerializeField] Vector3 aprop;
    [SerializeField] Vector3 lluny = new Vector3(0, 2, -5);
    [Space(10)]
    [SerializeField] float speed;
    [SerializeField] float time;

    CinemachineTransposer transposer;

    float zoom;
    public float Value => zoom;
    public Vector2 Dimensions { set => lluny = new Vector3(0, Mathf.Max(value.x, value.y) * 1.4f + 15, -Mathf.Max(value.x, value.y) * 1.4f - 15); }


    public void Enable()
    {
        keyboard.action.Enable();
        mouse.action.Enable();
    }
    public void Start()
    {
        transposer = vCamera.GetCinemachineComponent<CinemachineTransposer>();
        zoom = 1f;

        transposer.m_FollowOffset = FactorToZoom();
    }
    public void Update()
    {
        Zoom_Keyboard();
        Zoom_Mouse();

        zoom = Mathf.Clamp01(zoom);
        transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, FactorToZoom(), Time.deltaTime * time);
    }
    public void Disable()
    {
        keyboard.action.Disable();
        mouse.action.Disable();
    }


    void Zoom_Keyboard()
    {
        if (!keyboard.IsZero_Float())
        {
            zoom += keyboard.GetFloat() * speed;
        }
    }
    void Zoom_Mouse()
    {
        zoom -= mouse.GetVector2().y * speed * 2;
    }



    Vector3 FactorToZoom()
    {
        return Vector3.Lerp(aprop, lluny, zoom);
    }
    [System.Serializable]
    public class ZoomPosition
    {
        [SerializeField] Vector2 offset;
        Vector3 valor;
        public void Setup() => valor = new Vector3(0, offset.x, offset.y);
        public Vector3 Get => valor;
    }
}
