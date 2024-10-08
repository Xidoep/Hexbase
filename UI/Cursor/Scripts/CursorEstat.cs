using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/CursorEstat")]
public class CursorEstat : ScriptableObject
{
    static CursorEstat Instance;

    bool mostrar = true;
    Vector3 snap;

    [SerializeField] Fase_Colocar colocar;

    [SerializeField] InputActionReference mousePosition;
    [SerializeField] Visualitzacions visualitzacions;
    [SerializeField] AnimacioPerCodi anim_Mostrar;

    [SerializeField, ReadOnly] GameObject cursor;

    EstatColocable estat;
    Ray ray;
    float distanciaDelTerra;
    Vector3 final;
    int stepsCount;
    WaitForSeconds waitForSeconds;
    AudioClip so;

    public static void Mostrar(bool _mostrar) 
    {
        Instance.mostrar = _mostrar;
    }
    public static void Snap(Vector3 _snap) 
    {
        Instance.snap = _snap;
        Instance.visualitzacions.Snap.Play();
    } 
    public static void NoSnap() => Instance.snap = Vector3.down;


    void OnEnable()
    {
        Instance = this;

        mostrar = true;
        snap = Vector3.down;
        estat = null;

        colocar.OnStart += MostrarCursorReset;
        colocar.OnFinish += AmagarCursor;
        colocar.OnCanviarSeleccionada += CanviarCursor;

        waitForSeconds = new WaitForSeconds(1);
    }

    void OnDisable()
    {
        colocar.OnStart -= MostrarCursorReset;
        colocar.OnFinish -= AmagarCursor;
        colocar.OnCanviarSeleccionada -= CanviarCursor;
    }

    public void Actualitzar()
    {

        if (!cursor)
            return;

        if(!mostrar && cursor.activeSelf)
            AmagarCursor();
        else if(mostrar && !cursor.activeSelf)
            MostrarCursor();

        if (!cursor.activeSelf)
            return;

        if(snap != Vector3.down)
        {
            cursor.transform.position = snap;
            return;
        }

        ray = Camera.main.ScreenPointToRay(mousePosition.GetVector2());
        distanciaDelTerra = Camera.main.transform.position.y;
        stepsCount = 0;

        while ((ray.origin + ray.direction * distanciaDelTerra).y > 0.1f && stepsCount < 10)
        {
            distanciaDelTerra += (ray.origin + ray.direction * distanciaDelTerra).y * 1.5f;
            stepsCount++;
        }

        final = (ray.origin + ray.direction * distanciaDelTerra);
        cursor.transform.position = new Vector3(
            final.x,
            0.2f,
            final.z);

    }

    void CanviarCursor(EstatColocable estat)
    {
        Debug.Log("Canviar");
        if (this.estat != estat)
        {
            this.estat = estat;

            if (cursor != null)
                Destroy(cursor);

            cursor = this.estat.Prefab.Crear(false).gameObject;
            //cursor = Instantiate(this.estat.Prefag);
        }
    }
    void MostrarCursor()
    {
        Debug.Log("Mostrar");

        if (cursor == null)
            return;

        if (cursor.activeSelf)
            return;

        cursor.SetActive(true);
        anim_Mostrar.Play(cursor.transform);
    }
    void MostrarCursorReset() => XS_Coroutine.StartCoroutine(MostrarCursorReset_Corrutine());

    IEnumerator MostrarCursorReset_Corrutine()
    {
        yield return waitForSeconds;
        MostrarCursor();
        snap = Vector3.down;
    }

    void AmagarCursor()
    {
        snap = Vector3.down;

        if (cursor != null)
            cursor.SetActive(false);
    }

}
