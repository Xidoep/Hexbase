using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;
     

[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Menu")]
public class Fase_Menu : Fase
{
    enum Mode { FreeSyle, pila }

    Grid grid;

    [SerializeField] Grups grups;
    [SerializeField] Fase colocar;
    [SerializeField] SaveHex save;
    [SerializeField] CapturarPantalla capturarPantalla;
    [Linia]
    [SerializeField] Mode mode;
    [SerializeField] GameObject prefab_FreeSyle;
    [SerializeField] GameObject prefab_Pila;


    GameObject menu;

    public override void Actualitzar()
    {
        if (grid == null) grid = FindObjectOfType<Grid>();

        grid.CrearGrid();

        OnFinish += FreeSyle;

        if (!save.TePeces)
        {
            IniciarNet();
        }
        else
        {
            save.Load(grups, colocar);
        }
    }

    public void IniciarNet()
    {
        grid.Resetejar();
        grid.CrearBoto(grid.Centre);
    }


    public override void Finalitzar()
    {
        OnFinish_Invocar();
        OnFinish -= FreeSyle;
    }

    void FreeSyle() 
    {
        switch (mode)
        {
            case Mode.FreeSyle:
                menu = Instantiate(prefab_FreeSyle, UI_CameraMenu_Access.CameraMenu.transform);
                break;
            case Mode.pila:
                menu = Instantiate(prefab_Pila, UI_CameraMenu_Access.CameraMenu.transform);
                break;
        }
        menu.GetComponent<Canvas>().worldCamera = UI_CameraMenu_Access.CameraMenu;
        
    }
    public void Sortir()
    {
        if (!save.TePeces)
            return;

        if (!save.TeCaptures)
        {
            capturarPantalla.Capturar();
            XS_Coroutine.StartCoroutine(SortirTemps(3));
        }
        else
        {
            XS_Coroutine.StartCoroutine(SortirTemps(1));
        }
    }

    IEnumerator SortirTemps(float temps)
    {
        yield return new WaitForSeconds(temps);
        Debugar.Log("SORTIR");
        Application.Quit();
    }
}
