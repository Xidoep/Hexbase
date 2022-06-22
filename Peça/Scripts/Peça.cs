using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XS_Utils;



public class Peça : Hexagon, IPointerEnterHandler, IPointerExitHandler
{
    public override void Setup(Grid grid, Vector2Int coordenades, EstatPeça estat)
    {
        base.Setup(grid, coordenades, estat);
        subestat = estat.SubestatInicial;
    }

    //VARIABLES PRIVADES
    TilePotencial[] tiles;
    [SerializeField] Grups grups;
    [SerializeField] int indexgGrup;
    [SerializeField] Proximitat proximitat;


    //PROPIETATS   
    public override bool EsPeça => true;
    public TilePotencial[] Tiles => tiles;
    public TilePotencial Inicial => inicial;
    public 


    //INTERN
    TilePotencial inicial;
    float startTime;



    public override void Iniciar()
    {
        Debug.Log($"Iniciar ({EstatName}({Coordenades}))");
        name = $"{EstatName}({Coordenades})";

        WaveFunctionColapse.UltimaPeçaCreada = this;

        startTime = Time.realtimeSinceStartup;
        Actualitzar();
    }

    public override void Actualitzar()
    {
        Debug.Log($"Actualitzar ({EstatName}({Coordenades}))");
        name = $"{EstatName}({Coordenades})";

        CrearTilesPotencials();
        AssignarVeinsTiles(tiles);
        #region DEBUG
        for (int i = 0; i < tiles.Length; i++)
        {
            string _debug = "";
            _debug += $"ID = {tiles[i].ID} | Potencials = (";
            for (int p = 0; p < tiles[i].Possibilitats.Length; p++)
            {
                _debug += tiles[i].Possibilitats[p].name + ", ";
            }
            _debug += ") \n";

            _debug += $"|A- {(tiles[i].Veins[0] != null ? $"{tiles[i].Veins[0].ID}\n" : "NULL\n")}";
            _debug += $"|E- {(tiles[i].Veins[1] != null ? $"{tiles[i].Veins[1].ID}\n" : "NULL\n")}";
            _debug += $"|D- {(tiles[i].Veins[2] != null ? $"{tiles[i].Veins[2].ID}\n" : "NULL\n")}";
            _debug += " ";
            Debugar.Log(_debug);
        }
        Debugar.Log("------------------------");
        #endregion
        SeleccionarInteraccioInicial();

        Estat.TilesInicials(tiles);
        WaveFunctionColapse.Process(this, Agrupar);
        TornarVeinsAmbiguusSiCal();
       

        //PROCEDIMENT CORRECTE
        //1 Activar el WFC.
        //2 Afegir a grups.
        //3 Comprovar proximitats
        //4 Mirar quins patrons interns encaixen.
        //5 Crear peces/patrons
        

    }

    void Agrupar() 
    {
        grups.Agrupdar(this, Proximitat);
    }
    void Proximitat()
    {
        //proximitat.Add(this);
        
        Debug.LogError($"------------------------------------------------------------------------------- Cost Time = {Time.realtimeSinceStartup - startTime}", this);
    }

    public void ComprovarProximitats()
    {
        
    }

    void CrearTilesPotencials()
    {
        tiles = new TilePotencial[6];
        for (int i = 0; i < 6; i++)
        {
            tiles[i] = new TilePotencial(Estat, this, i);
        }
    }
    void AssignarVeinsTiles(TilePotencial[] tilesPotencials)
    {
        for (int i = 0; i < tilesPotencials.Length; i++)
        {
            tilesPotencials[i].GetVeins(this);

            if (tiles[i].Veins[0] == null)
                continue;

            tiles[i].Veins[0].Veins[0] = tiles[i];
        }
    }
    void SeleccionarInteraccioInicial()
    {
        inicial = tiles[0];
        for (int i = 1; i < tiles.Length; i++)
        {
            if (tiles[i].Veins[0] != null)
            {
                inicial = tiles[i];
                break;
            }
        }

    }
    void TornarVeinsAmbiguusSiCal()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i].Veins[0] == null)
                continue;

            //if (tiles[i].Veins[0].Peça.acabadaDeCrear)
            //    continue;

            tiles[i].Veins[0].Ambiguo(true);
            tiles[i].Veins[0].Veins[0] = tiles[i];
            tiles[i].Veins[0].Veins[1].Ambiguo(true);
            tiles[i].Veins[0].Veins[2].Ambiguo(true);
        }
    }



    public TilePotencial GetTile(int index) => tiles[index];

    public void CrearTilesFisics()
    {
        //*************************
        //ANALITZAR PATRONS INTERNS
        //*************************

        //if(!acabadaDeCrear)
            //animacioPerCodi.Play();

        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i].TileFisic != null)
                Destroy(tiles[i].TileFisic);

            tiles[i].Crear();
        }
    }

    public override void CanviarSubestat(Subestat subestat)
    {
        base.CanviarSubestat(subestat);
        for (int i = 0; i < Tiles.Length; i++)
        {
            Tiles[i].Detalls(subestat);
        }
    }





    //INTERACCIO
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {/*Show info*/ }
    public void OnPointerExit(PointerEventData eventData) {/*Hide info*/ }



    void OnValidate()
    {
        if (grups == null) grups = XS_Editor.LoadAssetAtPath<Grups>("Assets/XidoStudio/Hexbase/Grups/Grups.asset");
        if (proximitat == null) proximitat = XS_Editor.LoadAssetAtPath<Proximitat>("Assets/XidoStudio/Hexbase/Proximitat/Proximitat.asset");
    }
}



