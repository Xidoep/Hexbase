using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/CursorEstat")]
public class CursorEstat : MonoBehaviour
{
    [SerializeField] Fase_Colocar faseColocar;

    [SerializeField] InputActionReference mousePosition; 

    [SerializeField] GameObject cursor;
    Coroutine coroutine;

    Ray ray;
    float distanciaDelTerra;
    Vector3 final;

    void OnEnable()
    {
        
        //prediccio.OnStartPrediccio += AmagarCursor;
        //prediccio.OnEndPrediccio += MostrarCursor;
        faseColocar.OnStart += MostrarCursor;
        faseColocar.OnFinish += AmagarCursor;
    }

    private void OnDisable()
    {
        //prediccio.OnStartPrediccio -= AmagarCursor;
        //prediccio.OnEndPrediccio -= MostrarCursor;
        faseColocar.OnStart -= MostrarCursor;
        faseColocar.OnFinish -= AmagarCursor;
    }

    private void LateUpdate()
    {
        if (!cursor.activeSelf)
            return;

        ray = Camera.main.ScreenPointToRay(mousePosition.GetVector2());
        distanciaDelTerra = Camera.main.transform.position.y;
        Vector3 origenDelRaig = ray.origin + ray.direction * distanciaDelTerra;
        int step = 0;

        while ((ray.origin + ray.direction * distanciaDelTerra).y > 0.1f && step < 20)
        {
            distanciaDelTerra += (ray.origin + ray.direction * distanciaDelTerra).y * 1.5f;
            step++;
        }
        //Debug.Log($"ray to {(ray.origin + ray.direction * distanciaDelTerra)}");
        Debug.Log(step);
        final = (ray.origin + ray.direction * distanciaDelTerra);
        cursor.transform.position = new Vector3(
            final.x,
            0.2f,
            final.z);

    }

    void MostrarCursor()
    {
        Debug.Log("Mostrar");
        if(cursor != null)
        {
            cursor.SetActive(true);
        }
        else
        {
            cursor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cursor.transform.localScale = new Vector3(1, 0.1f, 1);
            cursor.GetComponent<Collider>().enabled = false;
        }
        //coroutine = XS_Coroutine.StartCoroutine(SeguirRatoli());
    }

    void AmagarCursor()
    {
        if(coroutine != null)
        {
            //XS_Coroutine.StopCoroutine(coroutine);
            //coroutine = null;
        }
        if (cursor == null)
            return;

        cursor.SetActive(false);
    }


    IEnumerator SeguirRatoli()
    {
        Ray ray;

        float distanciaDelTerra = Camera.main.transform.position.y;

        Debug.Log("ray?");

        while (cursor != null && cursor.activeSelf)
        {
            ray = Camera.main.ScreenPointToRay(mousePosition.GetVector2());
            distanciaDelTerra = Camera.main.transform.position.y;
            Vector3 origenDelRaig = ray.origin + ray.direction * distanciaDelTerra;
            int step = 3;

            while(step > 0)
            {
                distanciaDelTerra += (ray.origin + ray.direction * distanciaDelTerra).y;
                /*
                 * que coi faig aqui?
                 * en toeria, creo un raig,
                 * Si aquest raig + la distancia desde el terra de la camara, no es menor de zero.
                 * Sumo la distancia desde el terra del final d'aquest raig, a la distancia desde el terra.
                 * comença fent un origin de la posicio de la camara
                 * Despres en cada iteracio agafa l'origin i el substitueix per el final de raig.
                 * Al final la posicio
                 * */
                step--;
            }
            Debug.Log($"ray to {(ray.origin + ray.direction * distanciaDelTerra)}");


            cursor.transform.position = (ray.origin + ray.direction * distanciaDelTerra);

            yield return new WaitForEndOfFrame();
        }
    }
}
