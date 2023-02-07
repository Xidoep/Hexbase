using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/Altura")]
public class Detall_Altura : DetallScriptable
{
    [SerializeField][Range(1,3)] int cantonsConnectats;
    [SerializeField] Subestat casa;
    //[SerializeField] Substituible_Planta substituible;
    public override GameObject GameObject(Peça peça, TilePotencial tile)
    {
        SetAlturaSiCal(tile, peça);

        for (int i = 0; i < tile.Veins.Length; i++)
        {
            SetAlturaVeinaSiEsCasa(tile.Veins[i], casa);
        }

        switch (cantonsConnectats)
        {
            case 1:
                switch (tile.OrientacioFisica)
                {
                    case 0:
                        tile.altura0 = tile.Veins[0] != null ? Mathf.Min(tile.Veins[0].Altura, tile.Altura) : tile.Altura;
                        break;
                    case 1:
                        tile.altura0 = Mathf.Min(tile.Veins[2].Altura, tile.Altura);
                        break;
                    case 2:
                        tile.altura0 = Mathf.Min(tile.Veins[1].Altura, tile.Altura);
                        break;
                }
                break;
            case 2:
                switch (tile.OrientacioFisica)
                {
                    case 0:
                        tile.altura1 = Mathf.Min(tile.Veins[1].Altura, tile.Altura);
                        tile.altura2 = Mathf.Min(tile.Veins[2].Altura, tile.Altura);
                        break;
                    case 1:
                        tile.altura1 = tile.Veins[0] != null ? Mathf.Min(tile.Veins[0].Altura, tile.Altura) : tile.Altura;
                        tile.altura2 = Mathf.Min(tile.Veins[1].Altura, tile.Altura);
                        break;
                    case 2:
                        tile.altura1 = Mathf.Min(tile.Veins[2].Altura, tile.Altura);
                        tile.altura2 = tile.Veins[0] != null ? Mathf.Min(tile.Veins[0].Altura, tile.Altura) : tile.Altura;
                        break;
                }
                break;
            case 3:
                switch (tile.OrientacioFisica)
                {
                    case 0:
                        tile.altura0 = tile.Veins[0] != null ? Mathf.Min(tile.Veins[0].Altura, tile.Altura) : tile.Altura;
                        tile.altura1 = Mathf.Min(tile.Veins[1].Altura, tile.Altura);
                        tile.altura2 = Mathf.Min(tile.Veins[2].Altura, tile.Altura);
                        break;
                    case 1:
                        tile.altura0 = Mathf.Min(tile.Veins[2].Altura, tile.Altura);
                        tile.altura1 = tile.Veins[0] != null ? Mathf.Min(tile.Veins[0].Altura, tile.Altura) : tile.Altura;
                        tile.altura2 = Mathf.Min(tile.Veins[1].Altura, tile.Altura);
                        break;
                    case 2:
                        tile.altura0 = Mathf.Min(tile.Veins[1].Altura, tile.Altura);
                        tile.altura1 = Mathf.Min(tile.Veins[2].Altura, tile.Altura);
                        tile.altura2 = tile.Veins[0] != null ? Mathf.Min(tile.Veins[0].Altura, tile.Altura) : tile.Altura;
                        break;
                }
                break;
        }


        Substituible_Planta[] substituibles = tile.TileFisic.GetComponentsInChildren<Substituible_Planta>();
        for (int s = 0; s < substituibles.Length; s++)
        {
            CrearPlantes(substituibles[s], 
                substituibles[s].Costat == 0 ? tile.altura0 : 
                substituibles[s].Costat == 1 ? tile.altura1 : tile.altura2);
        }



        return null;
    }

    void CrearPlantes(Substituible_Planta substituible, int plantes)
    {
        Debug.Log($"Crear {plantes} plantes");
        Substituible_Planta plantaActual = substituible.Substituir(false).GetComponentInChildren<Substituible_Planta>();

        if (plantaActual == null)
            return;

        for (int a = 1; a <= plantes; a++)
        {
            plantaActual = plantaActual.Substituir(a == plantes).GetComponentInChildren<Substituible_Planta>();
            if (plantaActual == null)
                break;
        }
    }

    void SetAlturaVeinaSiEsCasa(TilePotencial tile, Subestat casa)
    {
        if (tile == null)
            return;

        if (tile.Peça.SubestatIgualA(casa))
            SetAlturaSiCal(tile, tile.Peça);
    }

    void SetAlturaSiCal(TilePotencial tile, Peça peça)
    {
        if (tile.TeAltura)
            return;

        tile.Altura = Random.Range(1, peça.Casa.Necessitats.Length);
    }
}


