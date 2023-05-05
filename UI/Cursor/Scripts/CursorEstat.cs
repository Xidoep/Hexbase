using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/CursorEstat")]
public class CursorEstat : MonoBehaviour
{
    static bool mostrar = true;
    static Vector3 snap;

    [SerializeField] Fase_Colocar faseColocar;

    [SerializeField] InputActionReference mousePosition; 

    [SerializeField] GameObject cursor;

    Estat estat;
    Ray ray;
    float distanciaDelTerra;
    Vector3 final;
    int stepsCount;




    /*FALTA:
     * -Que s'snapy sobre la ranura.
     * ?Que no es mostri quan per sobre d'altres peces??? s'hauria de mostrar la info. Es pot fer aixo si estas en mode Infinit només
     * detall-Posar una mica d'animacio de tild, que es tombi cap una vanda i una altre amb el delta del mouse.
     * detall-Que es quedi clavada allà on has clicat (sobre una ranura) i que es borri quan apareix la nova. Amb una animacio per fer veure que està tot preparat.
     * 
     */
    public static void Mostrar(bool _mostrar) => mostrar = _mostrar;
    public static void Snap(Vector3 _snap) => snap = _snap;
    public static void NoSnap() => snap = Vector3.down;

    void OnEnable()
    {
        mostrar = true;
        snap = Vector3.down;
        estat = null;
        faseColocar.OnStart += MostrarCursor;
        faseColocar.OnFinish += AmagarCursor;
        faseColocar.OnCanviarSeleccionada += CanviarCursor;
    }

    private void OnDisable()
    {
        faseColocar.OnStart -= MostrarCursor;
        faseColocar.OnFinish -= AmagarCursor;
        faseColocar.OnCanviarSeleccionada -= CanviarCursor;
    }

    private void LateUpdate()
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

    void CanviarCursor(Estat estat)
    {
        Debug.Log("Canviar");
        if (this.estat != estat)
        {
            this.estat = estat;

            if (cursor != null)
                Destroy(cursor);

            cursor = Instantiate(this.estat.Prefag);
        }
    }
    void MostrarCursor()
    {
        Debug.Log("Mostrar");

        if (cursor == null)
            return;

        cursor.SetActive(true);
    }

    void AmagarCursor()
    {
        //estat = null;

        if (cursor == null)
            return;

        cursor.SetActive(false);
    }

}
