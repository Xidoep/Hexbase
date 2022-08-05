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





    [System.Serializable]
    public class Movement
    {
        [SerializeField] InputActionReference keyboard;
        [SerializeField] InputActionReference mouse;
        [Space(10)]
        [SerializeField] float speed;
        [SerializeField] float time;
        Vector3 movement;



        public void Start(Transform transform)
        {
            movement = transform.position;
        }
        public void Update(Transform transform, float zoom)
        {
            Moviment_Keyboard(transform);
            //Moviment_Mouse(transform);

            transform.position = Vector3.Lerp(transform.position, movement, Time.deltaTime * time * ((zoom + 1) * 5));
        }



        void Moviment_Keyboard(Transform transform)
        {
            if (!keyboard.IsVector2Zero())
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
        [SerializeField] float speed;
        [SerializeField] float time;
        Quaternion rotacio;



        public void Start(Transform transform)
        {
            rotacio = transform.rotation;
        }
        public void Update(Transform transform)
        {
            Rotacio_Keyboard();

            transform.rotation = Quaternion.Lerp(transform.rotation, rotacio, Time.deltaTime * time);
        }



        void Rotacio_Keyboard()
        {
            if (!keyboard.IsFloatZero())
            {
                rotacio *= Quaternion.Euler(Vector3.up * (speed * keyboard.GetFloat()));
            }
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



        void Zoom_Keyboard()
        {
            if (!keyboard.IsFloatZero())
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
