using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/Casa")]
public class ConstruccioCases : DetallScriptable
{
    [SerializeField][Range(1,3)] int cantonsConnectats;
    [SerializeField] Subestat casa;
    //[SerializeField] Substituible_Planta substituible;
    public override GameObject GameObject(Pe�a pe�a, TilePotencial tile)
    {
        SetAlturaSiCal(tile, pe�a);

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

        //la feina est� festa.

        //vale, estic espes...
        //ara, aix� te la informacio de un dels tiles, i saps la seva altura.
        //Ara hauriem de comen�ar a construir.
        //pero cada tile ha de tenir les seves "plantes" possibles.
        //Aixo hauria d'estar guardat aqu� mateix.
        //i cada "planta" que es crei, ha de tenir les seves propies "plantes" possibles superiors.
        //aixo ja no se a on colocar-ho.
        //Podriem colocar un "sustituible" a la "planta", aix� buscar les opcions que dona.
        //Els subtituible es destruir a canvi. Osigui, que el sustituible ha d'estar a dins la "planta" com un objecte buid.
        //aixo tamb� funciona desde el tile.

        //Osigui, que s'ha de fer aix� fins que s'acabin els pisos.
        //Es busca el Component Substituible al child.
        //Es sustituieix aquest pel que sigui.
        //Es torna a repetir el mateix fins a arribar a la altura.
        //Quan s'iguala l'altura, s'agafa la versio Top*.

        //*El sustituible es especial, ja que cont� 2 llistes. Un de "normal" i l'altre de top.

        //En el primer pas. Com difernciem si hi ha m�s d'un costat del tile amb edifici?
        //aqu� hi ha la questio. Potse es pot posar un petit identificador al substituible.
        //ja que aix� s'ha de fer per tots, potser es pot fer a la vegada.
        //Osigui, per cada Substituible que trobis al inici...
        //ja, pero com assignes un altura especifica a cada un?
        //dificil...
        //Potser amb la gerarquia?
        //el 0 al principi, emparentat a ell, l'1...
        //Potser poden ser scripts diferents.
        //potser te mes sentit aixi. Es dira Subsituible_Planta i heredaran de Subsituible.
        //I cada costat tindra un Subsituible_Planta0/1/2... que hereder� de Subsituible planta..

        //Aquest valor d'altura, fins i tot, es pot colocar al subsituible, aixi treiem merda de la Pe�a.
        //perfecte

        //PEr ultim, al loop zero, esl busca un Substituible Especific. l'1, el 2 o el 3
        //A partir d'aquest, sempre es busca el generic. Substituible_Planta.
        //i es continua fins que la Planta no tingui subsituible o fins que s'iguali l'index amb l'escala i es coloqui la pe�a top.




        //Despres ja veurem com condicionar la pe�a amb la pe�a a la que est� agafada, per tenir mes varietat d'edificis.

        Substituible_Planta[] substituibles = tile.TileFisic.GetComponentsInChildren<Substituible_Planta>();
        for (int s = 0; s < substituibles.Length; s++)
        {
            CrearPlantes(substituibles[s], 
                substituibles[s].Costat == 0 ? tile.altura0 : 
                substituibles[s].Costat == 1 ? tile.altura1 : tile.altura2);
        }



        return base.GameObject(pe�a, tile);
    }

    void CrearPlantes(Substituible_Planta substituible, int plantes)
    {
        Debug.Log($"Crear {plantes} plantes");
        Substituible_Planta plantaActual = substituible.Substituir(false).GetComponentInChildren<Substituible_Planta>();
        Debug.Log($"PlantaActual Null == {plantaActual == null}");
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

        if (tile.Pe�a.SubestatIgualA(casa))
            SetAlturaSiCal(tile, tile.Pe�a);
    }

    void SetAlturaSiCal(TilePotencial tile, Pe�a pe�a)
    {
        if (tile.TeAltura)
            return;

        tile.Altura = Random.Range(1, pe�a.Casa.Necessitats.Length);
    }
}


