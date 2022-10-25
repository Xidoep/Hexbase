using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Menu")]
public class Fase_Menu : Fase
{
    enum Mode { FreeSyle }

    Grid grid;

    [SerializeField] Grups grups;
    [SerializeField] Fase colocar;
    [Linia]
    [SerializeField] SaveHex save;
    [Linia]
    [SerializeField] Mode mode;
    [SerializeField] GameObject prefab_FreeSyle;
    GameObject freeSyle;

    public override void Actualitzar()
    {
        if (grid == null) grid = FindObjectOfType<Grid>();

        grid.CrearGrid();

        OnFinish += FreeSyle;

        if (!save.TePeces)
        {
            grid.CrearBoto(grid.Centre);
        }
        else
        {
            save.Load(grups, colocar);
        }
    }


    public override void Finalitzar()
    {
        OnFinish_Invocar();
        OnFinish -= FreeSyle;
    }

    void FreeSyle() 
    {
        freeSyle = Instantiate(prefab_FreeSyle, UI_CameraMenu_Access.CameraMenu.transform);
        freeSyle.GetComponent<Canvas>().worldCamera = UI_CameraMenu_Access.CameraMenu;
    } 

    
}
