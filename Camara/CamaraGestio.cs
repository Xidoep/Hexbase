using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using XS_Utils;
using Cinemachine;

public class CamaraGestio : MonoBehaviour
{
    [SerializeField] Movement movement;
    [Linia]
    [SerializeField] Zoom zoom;
    [Linia]
    [SerializeField] Rotation rotation;


    private void OnEnable()
    {
        movement.Enable();
        zoom.Enable();
        rotation.Enable();
    }
    void Start()
    {
        movement.Start(transform);
        zoom.Start();
        rotation.Start(transform);
    }
    void Update()
    {
        movement.Update(transform, zoom.Value);
        rotation.Update(transform);
        zoom.Update();
    }
    private void OnDisable()
    {
        movement.Disable();
        zoom.Disable();
        rotation.Disable();
    }




    [System.Serializable]
    public class Movement
    {
        [SerializeField] InputActionReference keyboard;
        [SerializeField] InputActionReference mouse;
        [Space(10)]
        [SerializeField] float speed;
        [SerializeField] float time;
        Vector3 movement;


        public void Enable()
        {
            keyboard.action.Enable();
            mouse.action.Enable();
        }
        public void Start(Transform transform)
        {
            movement = transform.position;
        }
        public void Update(Transform transform, float zoom)
        {
            Moviment_Keyboard(transform);
            if(!Application.isEditor) 
                Moviment_Mouse(transform);

            transform.position = Vector3.Lerp(transform.position, movement, Time.deltaTime * time * (((zoom * 3) + 1) * 5));
        }
        public void Disable()
        {
            keyboard.action.Disable();
            mouse.action.Disable();
        }


        void Moviment_Keyboard(Transform transform)
        {
            if (!keyboard.IsZero_Vector2())
            {
                movement += ((transform.forward * keyboard.GetVector2().y) * speed) + ((transform.right * keyboard.GetVector2().x) * speed);
            }
        }
        void Moviment_Mouse(Transform transform)
        {
            Vector2 mPosition = mouse.GetVector2();
            if (NearUpScreen(mPosition)) movement += transform.forward * speed;
            else if (NearDownScreen(mPosition)) movement -= transform.forward * speed;

            if (NearRightScreen(mPosition)) movement += transform.right * speed;
            else if (NearLeftScreen(mPosition)) movement -= transform.right * speed;
        }



        bool NearUpScreen(Vector2 axis) => axis.y >= Screen.height * .95f;
        bool NearDownScreen(Vector2 axis) => axis.y <= Screen.height * .05f;
        bool NearRightScreen(Vector2 axis) => axis.x >= Screen.width * .95f;
        bool NearLeftScreen(Vector2 axis) => axis.x <= Screen.width * .05f;
    }





    [System.Serializable]
    public class Rotation
    {
        [SerializeField] InputActionReference keyboard;
        [SerializeField] InputActionReference mouseActivation;
        [SerializeField] InputActionReference mouse;
        [SerializeField] float speed;
        [SerializeField] float time;
        Quaternion rotacio;
        bool active;
        //float startMousePos;
        //Quaternion startRot;

        public void Enable()
        {
            keyboard.action.Enable();
            mouseActivation.action.Enable();
            mouseActivation.OnPerformedAdd(Activate);
            mouse.action.Enable();

        }
        public void Start(Transform transform)
        {
            rotacio = transform.rotation;
        }
        public void Update(Transform transform)
        {
            Rotacio_Keyboard();
            Rotacio_Mouse();

            transform.rotation = Quaternion.Lerp(transform.rotation, rotacio, Time.deltaTime * time);
        }
        public void Disable()
        {
            keyboard.action.Disable();
            mouseActivation.action.Disable();
            mouseActivation.OnPerformedRemove(Activate);
            mouse.action.Disable();
        }

        public void Activate(InputAction.CallbackContext context) 
        {
            active = context.GetBool();
            Fase_Colocar.Bloquejar(active);
        } 
        void Rotacio_Keyboard()
        {
            if (keyboard.IsZero_Float())
                return;

            rotacio *= Quaternion.Euler(Vector3.up * (speed * keyboard.GetFloat()));
        }

        void Rotacio_Mouse()
        {
            if (!active)
                return;

            rotacio *= Quaternion.Euler(Vector3.up * (speed * mouse.GetVector2().x));
            //rotacio *= Quaternion.Euler(Vector3.up * (speed * (startMousePos - mouse.GetVector2().y)));
            //rotacio = startRot * Quaternion.Euler(Vector3.up * (speed * (startMousePos - mouse.GetVector2().x)));
            //startMousePos = mouse.GetVector2().y;
        }
    }





    [System.Serializable]
    public class Zoom
    {
        [SerializeField] CinemachineVirtualCamera vCamera;
        [Space(10)]
        [SerializeField] InputActionReference keyboard;
        [SerializeField] InputActionReference mouse;
        [Space(10)]
        [SerializeField] Vector2 close;
        [SerializeField] Vector2 middle;
        [SerializeField] Vector2 far;
        Vector3 fClose;
        Vector3 fMiddle;
        Vector3 fFar;
        [Space(10)]
        [SerializeField] float speed;
        [SerializeField] float time;

        CinemachineTransposer transposer;
        float zoom;
        public float Value => zoom;


        public void Enable()
        {
            keyboard.action.Enable();
            mouse.action.Enable();
        }
        public void Start()
        {
            transposer = vCamera.GetCinemachineComponent<CinemachineTransposer>();
            zoom = .25f;

            fClose = new Vector3(0, close.x, close.y);
            fMiddle = new Vector3(0, middle.x, middle.y);
            fFar = new Vector3(0, far.x, far.y);

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
            if (zoom <= .5f)
                return Vector3.Lerp(fClose, fMiddle, (zoom * 2));
            else
                return Vector3.Lerp(fMiddle, fFar, ((zoom * 2) - 1));
        }
        [System.Serializable] public class ZoomPosition
        {
            [SerializeField] Vector2 offset;
            Vector3 valor;
            public void Setup() => valor = new Vector3(0, offset.x, offset.y);
            public Vector3 Get => valor;
        }
    }
}
