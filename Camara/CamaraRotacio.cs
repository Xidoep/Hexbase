using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using XS_Utils;


[System.Serializable]
public class CamaraRotacio
{
    [SerializeField] Fase_Menu faseMenu;
    [SerializeField] InputActionReference keyboard;
    //[SerializeField] InputActionReference mouse;
    [SerializeField] float speed;
    [SerializeField] float time;
    Quaternion rotacio;
    //bool active;
    //float startMousePos;
    //Quaternion startRot;

    public void Enable()
    {
        keyboard.action.Enable();
        faseMenu.OnStart += ResetRotacio;
        //mouseActivation.action.Enable();
        //mouseActivation.OnPerformedAdd(Activate);
        //mouse.action.Enable();

    }
    public void Start(Transform transform)
    {
        rotacio = transform.rotation;
    }
    public void Update(Transform transform)
    {
        Rotacio_Keyboard();
        //Rotacio_Mouse();

        transform.rotation = Quaternion.Lerp(transform.rotation, rotacio, Time.deltaTime * time);
    }
    public void Disable()
    {
        keyboard.action.Disable();
        faseMenu.OnStart -= ResetRotacio;
        //mouseActivation.action.Disable();
        //mouseActivation.OnPerformedRemove(Activate);
        //mouse.action.Disable();
    }

    public void Activate(InputAction.CallbackContext context)
    {
        //active = context.GetBool();
        //Fase_Colocar.Bloquejar(active);
    }

    public void ResetRotacio()
    {
        rotacio = Quaternion.Euler(Vector3.zero);
    }
    void Rotacio_Keyboard()
    {
        if (keyboard.IsZero_Float())
            return;

        rotacio *= Quaternion.Euler(Vector3.up * (speed * keyboard.GetFloat()));
    }

    void Rotacio_Mouse()
    {
        //if (!active)
        //    return;

        //rotacio *= Quaternion.Euler(Vector3.up * (speed * mouse.GetVector2().x));
        //rotacio *= Quaternion.Euler(Vector3.up * (speed * (startMousePos - mouse.GetVector2().y)));
        //rotacio = startRot * Quaternion.Euler(Vector3.up * (speed * (startMousePos - mouse.GetVector2().x)));
        //startMousePos = mouse.GetVector2().y;
    }
}
