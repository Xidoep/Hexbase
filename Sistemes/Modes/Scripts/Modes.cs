using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Modes")]
public class Modes : ScriptableObject
{
    [SerializeField] Mode mode;

    [Apartat("REFERNCIES")]
    [SerializeField] GameObject[] prefabs_FreeSyle;
    [SerializeField] GameObject[] prefabs_Pila;
    [SerializeField] PoolPeces pool;

    public Mode Mode => mode;
    public void Set(Mode mode) => this.mode = mode;

    public void ConfigurarModes()
    {
        GameObject menu = null;
        switch (mode)
        {
            case Mode.FreeStyle:
                for (int i = 0; i < prefabs_FreeSyle.Length; i++)
                {
                    menu = Instantiate(prefabs_FreeSyle[i], UI_CameraMenu_Access.CameraMenu.transform);
                    SetupMenuCanvasWorldCamera();
                }

                break;
            case Mode.Pila:
                pool.Iniciar();
                for (int i = 0; i < prefabs_Pila.Length; i++)
                {
                    menu = Instantiate(prefabs_Pila[i], UI_CameraMenu_Access.CameraMenu.transform);
                    SetupMenuCanvasWorldCamera();
                }

                break;
        }
        void SetupMenuCanvasWorldCamera() => menu.GetComponent<Canvas>().worldCamera = UI_CameraMenu_Access.CameraMenu;
    }

    void OnDisable()
    {
        //mode = Mode.Pila;
    }
}

public enum Mode { FreeStyle, Pila }