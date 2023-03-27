using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using XS_Utils;

public class MandoColocarPeces : MonoBehaviour
{
    [SerializeField] Camera camera;
    [SerializeField] InputActionReference posicioMouse;
    [SerializeField] InputActionReference colocar;
    [SerializeField] bool cursosBloquejat;

    Ray ray;
    RaycastHit[] results = new RaycastHit[1];
    Hexagon trobat;

    Hexagon Trobat
    {
        get => trobat;
        set
        {
            if(trobat != null)
            {
                trobat.OnPointerExit();
            }
            trobat = value;
            
            if(trobat != null)
            {
                trobat.OnPointerEnter();
            }
        }
    }


    private void OnEnable()
    {
        Activar(); //For testing

        colocar.action.Enable();
        colocar.OnPerformedAdd(Clicar);
    }

    private void OnDisable()
    {
        colocar.OnPerformedRemove(Clicar);
        colocar.action.Disable();
    }

    public void Activar()
    {
        cursosBloquejat = true;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void Desactivar()
    {
        cursosBloquejat = false;
        Cursor.lockState = CursorLockMode.None;
    }


    void Clicar(InputAction.CallbackContext action) 
    {
        Debug.Log("cLICAR");
        Trobat?.OnPointerDown();
        Trobat?.OnPointerUp();
    } 

    void Update()
    {
        if (!cursosBloquejat)
            return;

        ray = camera.ScreenPointToRay(posicioMouse.action.ReadValue<Vector2>());
        //Debug.DrawRay(camera.gameObject.transform.position, ray.direction);
        if(Physics.RaycastNonAlloc(ray, results) > 0)
        {
            if(Trobat != results[0].collider.GetComponent<Hexagon>())
            {
                Trobat = results[0].collider.GetComponent<Hexagon>();
            }
        }
        else
        {
            Trobat = null;
        }
    }

    void OnValidate()
    {
        if (camera == null) camera = Camera.main;
    }
}
