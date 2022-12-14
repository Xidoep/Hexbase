using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;
     
/// <summary>
/// Fase inicial del joc. On esculles el mode de joc, la partida i les opcions.
/// </summary>
[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Menu")]
public class Fase_Menu : Fase
{
   

    [Apartat("GUARDAT")]
    [SerializeField] SaveHex save;
    [SerializeField] Grups grups;
    [SerializeField] CapturarPantalla capturarPantalla;

    [Apartat("MENUS")]
    [SerializeField] Mode mode;
    [SerializeField] GameObject prefab_FreeSyle;
    [SerializeField] GameObject prefab_Pila;

    [Apartat("PECES")]
    [SerializeField] PoolPeces pool;

    [Apartat("SEGÜENT FASE")]
    [SerializeField] Fase colocar;

    Grid grid;
    GameObject menu;

    public override void Inicialitzar()
    {
        if (grid == null) grid = FindObjectOfType<Grid>();

        grid.CrearGrid();

        OnFinish += SetupModes;

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


    /*public override void Finalitzar()
    {
        OnFinish_Invocar();
        OnFinish -= MostrarMenu;
    }*/

    void SetupModes() 
    {
        switch (mode)
        {
            case Mode.FreeSyle:
                menu = Instantiate(prefab_FreeSyle, UI_CameraMenu_Access.CameraMenu.transform);
                break;
            case Mode.pila:
                pool.Iniciar();
                menu = Instantiate(prefab_Pila, UI_CameraMenu_Access.CameraMenu.transform);
                break;
        }
        menu.GetComponent<Canvas>().worldCamera = UI_CameraMenu_Access.CameraMenu;

        OnFinish -= SetupModes;
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
