using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using XS_Utils;
using Cinemachine;


public class CamaraGestio : MonoBehaviour
{
    [SerializeField] Transform objectiu;



    [Space(10)]
    [SerializeField] Transform nord;
    [SerializeField] Transform sud;
    [SerializeField] Transform est;
    [SerializeField] Transform oest;



    [Linia]
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
        movement.Start(objectiu);
        zoom.Start();
        rotation.Start(objectiu);
    }
    void Update()
    {
        movement.Update(objectiu);
        rotation.Update(objectiu);
        zoom.Update();
    }
    private void OnDisable()
    {
        movement.Disable();
        zoom.Disable();
        rotation.Disable();
    }

    public void ResetDimensions(Transform transform)
    {
        nord = transform;
        sud = transform;
        est = transform;
        oest = transform;
    }
    public void SetDimensions(Hexagon nord, Hexagon sud, Hexagon est, Hexagon oest)
    {
        if (nord == null)
            return;

        this.nord = nord.transform;
        this.sud = sud.transform;
        this.est = est.transform;
        this.oest = oest.transform;

        movement.Centrar(this.nord, this.sud, this.est, this.oest);
        movement.Limits = new Vector4(this.est.position.x, this.nord.position.z, this.oest.position.x, this.sud.position.z);
        zoom.Dimensions = new Vector2(this.oest.position.x - this.oest.position.x, this.nord.position.z - this.sud.position.z);
    }

    private void OnDrawGizmos()
    {
        if (nord == null)
            return;
        Gizmos.color = Color.red - (Color.black * .5f);

        Gizmos.DrawCube(new Vector3((est.position.x + oest.position.x) * 0.5f, 0, (nord.position.z + sud.position.z) * 0.5f),
            new Vector3(oest.transform.position.x - est.transform.position.x, 1, sud.transform.position.z - nord.transform.position.z)
            );

    }



    [System.Serializable]
    public class Movement
    {
        [SerializeField] InputActionReference keyboard;
        [SerializeField] InputActionReference mouse;
        [SerializeField] CinemachineTargetGroup targetGroup;
        [SerializeField] Transform centre;
        [Space(10)]
        [SerializeField] float speed;
        [SerializeField] float time;
        [Space(10)]
        [SerializeField] Vector4 limits;
        Vector3 movement;

        public Vector4 Limits { set => limits = value; }


        public void Enable()
        {
            keyboard.action.Enable();
            mouse.action.Enable();
        }
        public void Start(Transform transform)
        {
            movement = transform.localPosition;
        }
        public void Update(Transform transform, float zoom)
        {
            Moviment_Keyboard(transform);
            if(!Application.isEditor) 
                Moviment_Mouse(transform);

            movement = Vector3.Min(movement, new Vector3(limits.x, 0, limits.y));
            movement = Vector3.Max(movement, new Vector3(limits.z, 0, limits.w));

            transform.position = Vector3.Lerp(transform.position, movement, Time.deltaTime * time * (((zoom * 3) + 1) * 5));
        }
        public void Update(Transform transform)
        {
            Moviment_Keyboard(transform);
            //Moviment_Mouse(transform);

            movement = Vector3.Min(movement, new Vector3(limits.x - centre.position.x, 0, limits.y - centre.position.z));
            movement = Vector3.Max(movement, new Vector3(limits.z - centre.position.x, 0, limits.w - centre.position.z));


            if(Mathf.Abs(Vector3.Magnitude(targetGroup.transform.position - centre.position)) > 0.1f)
            {
                centre.position = Vector3.Lerp(centre.position, targetGroup.transform.position, 1f * Time.deltaTime);
            }
            transform.localPosition = Vector3.Lerp(transform.localPosition, movement, Time.deltaTime * time);
        }
        public void Disable()
        {
            keyboard.action.Disable();
            mouse.action.Disable();
        }

        public void Centrar(Transform nord, Transform sud, Transform est, Transform oest)
        {
            targetGroup.transform.position = new Vector3((est.position.x + oest.position.x) * 0.5f, 0, (nord.position.z + sud.position.z) * 0.5f);
            centre.transform.position = new Vector3((est.position.x + oest.position.x) * 0.5f, 0, (nord.position.z + sud.position.z) * 0.5f);
            /*targetGroup.m_Targets[0].target = nord;
            targetGroup.m_Targets[1].target = sud;
            targetGroup.m_Targets[2].target = est;
            targetGroup.m_Targets[3].target = oest;*/
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
        [SerializeField] Vector3 aprop;
        [SerializeField] Vector3 lluny = new Vector3(0, 2, -5);
        [Space(10)]
        [SerializeField] float speed;
        [SerializeField] float time;

        CinemachineTransposer transposer;
        
        float zoom;
        public float Value => zoom;
        public Vector2 Dimensions { set => lluny = new Vector3(0, Mathf.Max(value.x, value.y) * 1.4f +15, -Mathf.Max(value.x, value.y) * 1.4f -15); }


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
        [System.Serializable] public class ZoomPosition
        {
            [SerializeField] Vector2 offset;
            Vector3 valor;
            public void Setup() => valor = new Vector3(0, offset.x, offset.y);
            public Vector3 Get => valor;
        }
    }
}
