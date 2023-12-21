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
    [SerializeField] GameObject prefabs_Nivell;
    [SerializeField] GameObject prefabs_Sumari;
    [SerializeField] PoolPeces pool;

    GameObject menuInstanciat;
    GameObject nivellInstanciat;
    GameObject sumariInstanciat;

    public Mode Mode => mode;
    public void Set(Mode mode) 
    {
        //Si buscques on està configurat al apretar el boto Play, és al mateix boto.
        this.mode = mode;
        save.SetMode(mode);
    }

    public void ConfigurarModes()
    {
        if (menuInstanciat != null) Destroy(menuInstanciat);
        if (nivellInstanciat != null) Destroy(nivellInstanciat);
        if (sumariInstanciat != null) Destroy(sumariInstanciat);



        switch (mode)
        {
            case Mode.FreeStyle:
                menuInstanciat = Instantiate(prefabs_FreeSyle, UI_CameraMenu_Access.CameraMenu.transform);
                
                SetupMenuCanvasWorldCamera();

                break;
            case Mode.Pila:
                pool.Iniciar();
                menuInstanciat = Instantiate(prefabs_Pila, UI_CameraMenu_Access.CameraMenu.transform);
                SetupMenuCanvasWorldCamera();

                break;
        }
        sumariInstanciat = Instantiate(prefabs_Sumari, UI_CameraMenu_Access.CameraMenu.transform);
        nivellInstanciat = Instantiate(prefabs_Nivell, UI_CameraMenu_Access.CameraMenu.transform);

        void SetupMenuCanvasWorldCamera() => menuInstanciat.GetComponent<Canvas>().worldCamera = UI_CameraMenu_Access.CameraMenu;
    }

    public void Destruir() => menuInstanciat.GetComponent<AnimacioPerCodi_GameObject_Referencia>().Destroy();

    public void Normal() => mode = Mode.Pila;
    public void FreeStyle() => mode = Mode.FreeStyle;
    void OnDisable()
    {
        menuInstanciat = null;
        //mode = Mode.Pila;
    }

    

}

public enum Mode { FreeStyle, Pila }