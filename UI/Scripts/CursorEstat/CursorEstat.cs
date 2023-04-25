using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/CursorEstat")]
public class CursorEstat : ScriptableObject
{
    [SerializeField] Prediccio prediccio;
    [SerializeField] Fase_Colocar faseColocar;
    [SerializeField] Fase_Menu faseMenu;
    [SerializeField] UI_Menu uiMenu;
    [SerializeField] Camera camera;
    [SerializeField] InputActionReference mousePosition; 

    [SerializeField] GameObject cursor;
    Coroutine coroutine;

    void OnEnable()
    {
        /*
        prediccio.OnStartPrediccio += AmagarCursor;
        prediccio.OnEndPrediccio += MostrarCursor;
        faseColocar.OnStart += MostrarCursor;
    */
    }

    private void OnDisable()
    {
        prediccio.OnStartPrediccio -= AmagarCursor;
        prediccio.OnEndPrediccio -= MostrarCursor;
        faseColocar.OnStart -= MostrarCursor;
    }



    void MostrarCursor()
    {
        if(cursor != null)
        {
            cursor.SetActive(true);
        }
        else
        {
            cursor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        }
        coroutine = XS_Coroutine.StartCoroutine(SeguirRatoli());
    }

    void AmagarCursor()
    {
        if(coroutine != null)
        {
            XS_Coroutine.StopCoroutine(coroutine);
            coroutine = null;
        }
        if (cursor == null)
            return;

        cursor.SetActive(false);
    }
    
    IEnumerator SeguirRatoli()
    {
        Ray ray;

        float maxDistance = 100;
        float currentDistance = 0;
        bool rayhit = false;

        while (cursor != null && cursor.activeSelf)
        {
            maxDistance = 100;
            currentDistance = 0;
            rayhit = false;



            ray = Camera.main.ScreenPointToRay(mousePosition.GetVector2());
            if (Physics.Raycast(ray, out RaycastHit hit, 100))
            {
                cursor.transform.position = hit.point;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
