using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XS_Utils;



public class Peça : Hexagon, IPointerEnterHandler, IPointerExitHandler
{
    //VARIABLES PRIVADES
    [SerializeField] TilePotencial[] tiles;

    //PROPIETATS
    public override bool EsPeça => true;

    //INTERN
    TilePotencial inicial;
    float startTime;
    public TilePotencial[] Tiles => tiles;
    public TilePotencial Inicial => inicial;

    public override void Iniciar()
    {
        Debug.Log($"Iniciar ({EstatName}({Coordenades}))");
        name = $"{EstatName}({Coordenades})";

        WaveFunctionColapse.UltimaPeçaCreada = this;

        startTime = Time.realtimeSinceStartup;
        Actualitzar();
        //1 Comproves.

        //Comprovacions.Combinar(this, Actualitzar);


        //Actualitzar();
        //WaveFunctionColapse.Process(inicial, FerComprovacio);
        /*for (int i = 0; i < tiles.Length; i++)
        {
            WaveFunctionColapse.Add(tiles[i]);
        }*/
    }

    public override void Actualitzar()
    {
        //2.- Ferla encaixar
        
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
        //Comprovacions.EnviarVeinsAComprovacio(this);

        Estat.TilesInicials(tiles);
        WaveFunctionColapse.Process(this, IniciarComprovacions);
        TornarVeinsAmbiguusSiCal();
       
    }

    void IniciarComprovacions() 
    {
        Debug.LogError($"------------------------------------------------------------------------------- Cost Time = {Time.realtimeSinceStartup - startTime}");
        //Comprovacions.Iniciar();
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






    //INTERACCIO
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {/*Show info*/ }
    public void OnPointerExit(PointerEventData eventData) {/*Hide info*/ }


}


