using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using XS_Utils;
using Cinemachine;

public class CamaraGestio : MonoBehaviour
{

    [Apartat("MOVIMENT")]
    [SerializeField] InputActionReference panKeyboard;
    [SerializeField] InputActionReference panMouse;
    public float movementSpeed;
    public float movementTime;
    Vector3 movement;

    [Apartat("ZOOM")]
    [SerializeField] CinemachineVirtualCamera vCamera;
    CinemachineTransposer transposer;
    [SerializeField] InputActionReference zoomKeyboard;
    [SerializeField] InputActionReference zoomMouse;
    public ZoomPosition close;
    public ZoomPosition middle;
    public ZoomPosition far;
    public float zoomSpeed;
    public float zoomTime;
    float zoom;

    [Apartat("DEBUG")]
    public Vector2 input;

    // Start is called before the first frame update
    void Start()
    {
        movement = transform.position;

        transposer = vCamera.GetCinemachineComponent<CinemachineTransposer>();
        zoom = .25f;
        close.Setup();
        middle.Setup();
        far.Setup();
        transposer.m_FollowOffset = FactorToZoom();
        
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        Zoom();
    }

    void Movement()
    {
        Moviment_Keyboard();
        //Moviment_Mouse();

        transform.position = Vector3.Lerp(transform.position, movement, Time.deltaTime * movementTime * (zoom + 1));
    }



    void Moviment_Keyboard()
    {
        if (!panKeyboard.IsVector2Zero())
        {
            movement += ((transform.forward * panKeyboard.GetVector2().y) * movementSpeed) + ((transform.right * panKeyboard.GetVector2().x) * movementSpeed);
        }

    }
    void Moviment_Mouse()
    {
        Vector2 mPosition = panMouse.GetVector2();
        if (NearUpScreen(mPosition)) movement += transform.forward * movementSpeed;
        else if (NearDownScreen(mPosition)) movement -= transform.forward * movementSpeed;

        if (NearRightScreen(mPosition)) movement += transform.right * movementSpeed;
        else if (NearLeftScreen(mPosition)) movement -= transform.right * movementSpeed;
    }


    void Zoom()
    {
        Zoom_Keyboard();
        Zoom_Mouse();

        zoom = Mathf.Clamp01(zoom);
        transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, FactorToZoom(), Time.deltaTime * zoomTime);
    }

    void Zoom_Keyboard()
    {
        //input = zoomKeyboard.GetFloat();
        if (!zoomKeyboard.IsFloatZero())
        {
            zoom += zoomKeyboard.GetFloat() * zoomSpeed;
        }
        
    }
    void Zoom_Mouse()
    {
        input = zoomMouse.GetVector2();
        zoom += zoomMouse.GetVector2().y * zoomSpeed;
    }








    bool NearUpScreen(Vector2 axis) => axis.y >= Screen.height * .95f;
    bool NearDownScreen(Vector2 axis) => axis.y <= Screen.height * .05f;
    bool NearRightScreen(Vector2 axis) => axis.x >= Screen.width * .95f;
    bool NearLeftScreen(Vector2 axis) => axis.x <= Screen.width * .05f;

    Vector3 FactorToZoom()
    {
        if (zoom <= .5f)
            return Vector3.Lerp(close.Get, middle.Get, (zoom * 2));
        else
            return Vector3.Lerp(middle.Get, far.Get, ((zoom * 2) - 1));
    }


    [System.Serializable] public class ZoomPosition
    {
        [SerializeField] Vector2 offset;
        Vector3 valor;
        public void Setup() => valor = new Vector3(0, offset.x, offset.y);
        public Vector3 Get => valor;
    }
}
