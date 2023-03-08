using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Modes")]
public class Modes : ScriptableObject
{
    [SerializeField] Mode mode;
    [SerializeField] SaveHex save;

    [Apartat("REFERNCIES")]
    [SerializeField] GameObject prefabs_FreeSyle;
    [SerializeField] GameObject prefabs_Pila;
    [SerializeField] PoolPeces pool;

    GameObject menu;

    public Mode Mode => mode;
    public void Set(Mode mode) 
    {
        this.mode = mode;
        save.SetMode(mode);
    }

    public void ConfigurarModes()
    {
        if(menu != null)
        {
            Destroy(menu);
        }
        switch (mode)
        {
            case Mode.FreeStyle:
                menu = Instantiate(prefabs_FreeSyle, UI_CameraMenu_Access.CameraMenu.transform);
                SetupMenuCanvasWorldCamera();

                break;
            case Mode.Pila:
                pool.Iniciar();
                menu = Instantiate(prefabs_Pila, UI_CameraMenu_Access.CameraMenu.transform);
                SetupMenuCanvasWorldCamera();

                break;
        }
        void SetupMenuCanvasWorldCamera() => menu.GetComponent<Canvas>().worldCamera = UI_CameraMenu_Access.CameraMenu;
    }

    public void Destruir() => menu.GetComponent<AnimacioPerCodi_GameObject_Referencia>().Destroy();

    public void Normal() => mode = Mode.Pila;
    public void FreeStyle() => mode = Mode.FreeStyle;
    void OnDisable()
    {
        menu = null;
        //mode = Mode.Pila;
    }

    

}

public enum Mode { FreeStyle, Pila }