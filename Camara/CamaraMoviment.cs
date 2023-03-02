using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using XS_Utils;
using Cinemachine;

[System.Serializable]
public class CamaraMoviment
{

    [SerializeField] InputActionReference keyboard;
    [SerializeField] InputActionReference mouse;
    [SerializeField] CinemachineTargetGroup targetGroup;
    [SerializeField] Transform centre;
    [Space(10)]
    [Tooltip("Es la velocitat a la que es mou quan acostes el cursoso a les bores de la pantalla.")]
    [SerializeField] float speed;

    [Tooltip("Es el temps del Lerp que fa pel moviment. Ho fa amb Lerp perque sigui suau.")]
    [SerializeField] float time;
    [Space(10)]
    [SerializeField] Vector4 limits;

    float margeMoviment = 1.5f;
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
        if (!Application.isEditor)
            Moviment_Mouse(transform);

        movement = Vector3.Min(movement, new Vector3(limits.x - 1, 0, limits.y - 1));
        movement = Vector3.Max(movement, new Vector3(limits.z + 1, 0, limits.w + 1));

        transform.position = Vector3.Lerp(transform.position, movement, Time.deltaTime * time * (((zoom * 3) + 1) * 5));
    }
    public void Update(Transform transform)
    {
        Moviment_Keyboard(transform);
        //Moviment_Mouse(transform);

        movement = Vector3.Min(movement, new Vector3(limits.x - centre.position.x + margeMoviment, 0, limits.y - centre.position.z + margeMoviment));
        movement = Vector3.Max(movement, new Vector3(limits.z - centre.position.x - margeMoviment, 0, limits.w - centre.position.z - margeMoviment));


        if (Mathf.Abs(Vector3.Magnitude(targetGroup.transform.position - centre.position)) > 0.1f)
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

