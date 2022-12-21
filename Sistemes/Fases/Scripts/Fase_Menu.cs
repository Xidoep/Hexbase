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
    public const string SEGONA_PARTIDA = "SegonaPartida";

    [Apartat("GUARDAT")]
    [SerializeField] SaveHex save;
    [SerializeField] Guardat guardat;
    [SerializeField] Grups grups;
    [SerializeField] CapturarPantalla capturarPantalla;

    [Apartat("SEGÜENT FASE")]
    [SerializeField] Fase colocar;

    Grid grid;
    bool inici = true;


    //OVERRIDES
    public override void FaseStart()
    {
        OnFinish += MarcarComIniciat;

        if (grid == null) grid = FindObjectOfType<Grid>();

        grid.CrearGrid();

        if (inici)
        {
            if (save.TePeces)
                save.Load(grups, colocar);
            else
                Opcions();
        }
        else
            Opcions();
        
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

        guardat.SetLocal(SEGONA_PARTIDA, true);

        IEnumerator SortirTemps(float temps)
        {
            yield return new WaitForSeconds(temps);
            Debugar.Log("SORTIR");
            Application.Quit();
        }
    }

    void MarcarComIniciat()
    {
        inici = true;
        OnFinish -= MarcarComIniciat;
    }

    //PRIVADES
    void Opcions()
    {
        grid.Resetejar();
        grid.CrearBoto(grid.Centre);
    }

    void ConfigurarMode() 
    {
        inici = false;
    }

    
    new void OnDisable()
    {
        base.OnDisable();
        inici = true;
    }

    private void OnValidate()
    {
        if (guardat == null) guardat = XS_Editor.LoadGuardat<Guardat>();
    }
}
