using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using XS_Utils;
using Cinemachine;

[System.Serializable]

public class CamaraZoom
{
    readonly float factorMultiplicadorTamanyMapa = 0.9f;
    readonly WaitForSeconds waitForSeconds = new(0.05f);

    [SerializeField] CinemachineVirtualCamera vCamera;
    [Space(10)]
    [SerializeField] InputActionReference keyboard;
    //[SerializeField] InputActionReference mouse;
    [Space(10)]
    [SerializeField] Vector3 aprop;
    [SerializeField] Vector3 lluny = new Vector3(0, 2, -5);
    [SerializeField] Vector3 targetLluny = new Vector3(0, 2, -5);
    [Space(10)]
    [SerializeField] float speed;
    [SerializeField] float time;

    CinemachineTransposer transposer;

    float zoom;

    public Vector2 Dimensions 
    {
        set 
        { 
            targetLluny = new Vector3(
                0, 
                Mathf.Max(value.x, value.y) * factorMultiplicadorTamanyMapa + 15, 
                -Mathf.Max(value.x, value.y) * factorMultiplicadorTamanyMapa - 15);

            XS_Coroutine.StartCoroutine(LerpLlum());
        } 
    }



    public void Enable()
    {
        keyboard.action.Enable();
    }
    public void Start()
    {
        transposer = vCamera.GetCinemachineComponent<CinemachineTransposer>();
        zoom = 0.25f;

        transposer.m_FollowOffset = FactorToZoom();
    }
    public void Update()
    {
        Zoom_Keyboard();

        //zoom = Mathf.Clamp01(zoom);
        transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, FactorToZoom(), Time.deltaTime * time);
    }
    public void Disable()
    {
        keyboard.action.Disable();
    }

    IEnumerator LerpLlum()
    {
        while (Vector3.Magnitude(targetLluny - lluny) > 0.1f)
        {
            lluny = Vector3.Lerp(lluny, targetLluny, 0.1f * Time.deltaTime);
            yield return waitForSeconds;
        }
        yield return null;
    }

    void Zoom_Keyboard()
    {
        if (!keyboard.IsZero_Float())
        {
            zoom += keyboard.GetFloat() * speed;
            zoom = Mathf.Clamp01(zoom);
        }
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
