using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XS_Utils;

public class UI_FinalPartida : MonoBehaviour
{
    [SerializeField] Fase_Resoldre resoldre;
    [SerializeField] Fase menu;
    [SerializeField] Fase iniciar;
    [SerializeField] CapturarPantalla capturarPantalla;

    [SerializeField] Button tornar;
    [SerializeField] Button repetir;
    [SerializeField] Button continuar;

    [SerializeField] Utils_InstantiableFromProject prefab_uiPreguntarGuardar;

    float TempsCanviarFase(bool guardar) => guardar ? 3 : 1;

    private void OnEnable()
    {
        //tornar.onClick.AddListener(Tornar);
    }

    //Tornar:
        //Si- Guardar i anar a fas menu
        //No- anar a fase menu
    //Repetri
        //Si- Guardar i anar a fase inici
        //No- anar a fase inici

    void Tornar() => prefab_uiPreguntarGuardar.InstantiateReturn().GetComponent<Utils_EsdevenimentDelegatBool>().Registrar(PopupTornar);
    void Repetir() => prefab_uiPreguntarGuardar.InstantiateReturn().GetComponent<Utils_EsdevenimentDelegatBool>().Registrar(PopupRepetir);

    void PopupTornar(bool guardar)
    {
        if(guardar) 
            capturarPantalla.Capturar();

        StartCoroutine(CanviarMenu(TempsCanviarFase(guardar), menu));
    }
    void PopupRepetir(bool guardar)
    {
        if (guardar)
            capturarPantalla.Capturar();

        StartCoroutine(CanviarMenu(TempsCanviarFase(guardar), iniciar));
    }

    IEnumerator CanviarMenu(float temps, Fase fase)
    {
        yield return new WaitForSeconds(temps);
        resoldre.Nivell.Reset();
        fase.Iniciar();
    }

}
