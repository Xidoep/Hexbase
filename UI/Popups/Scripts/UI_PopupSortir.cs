using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XS_Utils;

public class UI_PopupSortir : MonoBehaviour
{

    [Apartat("FROM PROJECt")]
    [SerializeField] Fase_Menu menu;
    [SerializeField] Utils_InstantiableFromProject fadeOut;
    [SerializeField] Utils_InstantiableFromProject sortir2;

    [Apartat("FROM SCENE")]
    [SerializeField] Button button;
    
    [Apartat("Autoconfigurable")]
    [SerializeField] Guardat guardat;

    bool segonaPartida;

    private void OnEnable()
    {
        segonaPartida = (bool)guardat.Get(Fase_Menu.SEGONA_PARTIDA, false);

        if (segonaPartida)
        {
            button.onClick.AddListener(menu.Sortir);
            button.onClick.AddListener(fadeOut.Instantiate);
        }
        else
        {
            button.onClick.AddListener(sortir2.Instantiate);
        }
    }

    private void OnDisable()
    {
        if (segonaPartida)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    private void OnValidate()
    {
        if(guardat == null) guardat = XS_Editor.LoadGuardat<Guardat>();
    }
}
