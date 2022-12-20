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
    [SerializeField] GameObject[] prefabs_FreeSyle;
    [SerializeField] GameObject[] prefabs_Pila;

    [Apartat("PECES")]
    [SerializeField] PoolPeces pool;

    [Apartat("SEGÜENT FASE")]
    [SerializeField] Fase colocar;

    Grid grid;
    GameObject menu;
    bool inici = true;

    //PORPIETATS
    public Mode Mode => mode;


    //OVERRIDES
    public override void Inicialitzar()
    {
        OnFinish += ConfigurarMode;
        
        if (grid == null) grid = FindObjectOfType<Grid>();
        grid.CrearGrid();

        if (inici)
        {
            if (!save.TePeces)
            {
                IniciarNet();
            }
            else
            {
                save.Load(grups, colocar);
            }
        }
        else
        {
            //Instanciar la UI amb els punts guanyats i les opcions 
        }
        
    }

    //PUBLIQUES
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

        IEnumerator SortirTemps(float temps)
        {
            yield return new WaitForSeconds(temps);
            Debugar.Log("SORTIR");
            Application.Quit();
        }
    }

    //PRIVADES
    void IniciarNet()
    {
        grid.Resetejar();
        grid.CrearBoto(grid.Centre);
    }

    void ConfigurarMode() 
    {
        switch (mode)
        {
            case Mode.FreeSyle:
                for (int i = 0; i < prefabs_FreeSyle.Length; i++)
                {
                    menu = Instantiate(prefabs_FreeSyle[i], UI_CameraMenu_Access.CameraMenu.transform);
                    SetupMenuCanvasWorldCamera();
                }
               
                break;
            case Mode.pila:
                pool.Iniciar();
                for (int i = 0; i < prefabs_Pila.Length; i++)
                {
                    menu = Instantiate(prefabs_Pila[i], UI_CameraMenu_Access.CameraMenu.transform);
                    SetupMenuCanvasWorldCamera();
                }
                    
                break;
        }

        inici = false;

        OnFinish -= ConfigurarMode;
        
        void SetupMenuCanvasWorldCamera() => menu.GetComponent<Canvas>().worldCamera = UI_CameraMenu_Access.CameraMenu;
    }

    new void OnDisable()
    {
        base.OnDisable();
        inici = true;
    }

  
}
