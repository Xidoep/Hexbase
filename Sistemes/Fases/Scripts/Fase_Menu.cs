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
    [SerializeField] Fase_Iniciar iniciar;
    [SerializeField] Fase colocar;

    [Apartat("UI")]
    [SerializeField] Utils_InstantiableFromProject sortir1;
    [SerializeField] Utils_InstantiableFromProject sortir2;
    [SerializeField] Utils_InstantiableFromProject fadeOut;

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
    void Sortir()
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
        iniciar.Reset();
    }

    void ConfigurarMode() 
    {
        inici = false;
    }

    


    public void PopupSortir() => sortir1.InstantiateReturn().GetComponent<Utils_EsdevenimentDelegatBool>().Registrar(BromaSortir);
    void BromaSortir(bool sortir)
    {
        if (sortir)
        {
            bool segonaPartida = (bool)guardat.Get(Fase_Menu.SEGONA_PARTIDA, false);

            if (segonaPartida)
            {
                Sortir();
                fadeOut.Instantiate();
            }
            else
            {
                sortir2.Instantiate();
            }
        }
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
