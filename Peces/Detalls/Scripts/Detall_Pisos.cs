using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Detall_Pisos : Detall
{
    public override void Setup(string[] arg)
    {
        pisos = GetComponentsInChildren<Detall_Pis>();
    }


    Peça peça;
    TilePotencial tile;
    [SerializeField] Detall_Pis[] pisos;
    [SerializeField] int indexTile = -1;
    [SerializeField] int[] altures;

    Detall_Pisos buscat;
    int altura;

    [ShowInInspector] int ExtComparat//0
    {
        get
        {
            if (tile == null)
                return -1;
            if (!Application.isPlaying)
                return -1;
            if (tile.Veins[0] == null)
                return -1;
            if (!tile.Veins[0].TileFisic.TryGetComponent(out buscat))
                return -1;
            if (buscat.altures.Length == 0)
                return Mathf.Min(altures[0], tile.Peça.CasesLength);

            return Mathf.Min(altures[0], buscat.altures[0]);
        }
    }
    [ShowInInspector] int DreComparat//1
    {
        get
        {
            if (tile == null)
                return -1;
            if (!Application.isPlaying)
                return -1;
            if (tile.Veins[1] == null)
                return -1;
            if (!tile.Veins[1].TileFisic.TryGetComponent(out buscat))
                return -1;
            if (buscat.altures.Length == 0)
                return Mathf.Min(altures[1], tile.Peça.CasesLength);

            return Mathf.Min(altures[1], buscat.altures[2]);
        }
    }
    [ShowInInspector] int EsqComparat//2
    {
        get
        {
            if (tile == null)
                return -1;
            if (!Application.isPlaying)
                return -1;
            if (tile.Veins[2] == null)
                return -1;
            if (!tile.Veins[2].TileFisic.TryGetComponent(out buscat))
                return -1;
            if(buscat.altures.Length == 0)
                return Mathf.Min(altures[2], tile.Peça.CasesLength);

            return Mathf.Min(altures[2], buscat.altures[1]);
        }
    }



    private void OnEnable()
    {
        if (peça == null) peça = GetComponentInParent<Peça>();
        if (pisos == null || pisos.Length == 0) pisos = GetComponentsInChildren<Detall_Pis>();

        peça.enCrearDetalls += CompararAltures;
    }


    private void OnDisable()
    {
        peça.enCrearDetalls -= CompararAltures;
    }



    void CompararAltures()
    {
        for (int i = 0; i < peça.Tiles.Length; i++)
        {
            if (peça.Tiles[i].TileFisic == gameObject)
            {
                indexTile = i;
                break;
            }
        }

        tile = peça.Tiles[indexTile];
        for (int i = 0; i < pisos.Length; i++)
        {
            pisos[i].orientacioFisica = tile.OrientacioFisica;
        }



        SetAltures();
        

        if (tile.Veins[0] != null)
        {
            if (tile.Veins[0].TileFisic.TryGetComponent(out buscat))
            {
                if (buscat.altures == null || buscat.altures.Length == 0)
                {
                    Debug.LogError($"{buscat.gameObject.name} no te altures", buscat.gameObject);
                    buscat.SetAltures(new int[] { altures[0], -1, -1 });
                }

                //altures[0] = Mathf.Min(peça.CasesLength, tile.Veins[0].Peça.CasesLength);
                altures[0] = Mathf.Min(peça.CasesLength, buscat.altures[0]);
            }
            else altures[0] = 1;
        }
        else altures[0] = 1;

        if (tile.Veins[1].TileFisic.TryGetComponent(out buscat))
        {
            if (buscat.altures == null || buscat.altures.Length == 0)
            {
                Debug.LogError($"{buscat.gameObject.name} no te altures", buscat.gameObject);
                buscat.SetAltures(new int[] { -1, altures[1], -1 });
            }

            altures[1] = Mathf.Min(altures[1], buscat.altures[2]);
        }
        //else altures[1] = peça.CasesLength;

        if (tile.Veins[2].TileFisic.TryGetComponent(out buscat))
        {
            if (buscat.altures == null || buscat.altures.Length == 0)
            {
                Debug.LogError($"{buscat.gameObject.name} no te altures", buscat.gameObject);
                buscat.SetAltures(new int[] { -1, -1, altures[2] });
            }

            altures[2] = Mathf.Min(altures[2], buscat.altures[1]);
        }
        //else altures[2] = peça.CasesLength;

        Crear();
    }



    public void SetAltures()
    {
        altures = new int[]
        {
            Random.Range(1, peça.CasesLength),
            Random.Range(1, peça.CasesLength),
            Random.Range(1, peça.CasesLength),
        };
    }
    public void SetAltures(int[] altures)
    {

        this.altures = new int[]
        {
            altures[0] == -1 ? Random.Range(1, peça.CasesLength) : altures[0],
            altures[1] == -1 ? Random.Range(1, peça.CasesLength) : altures[1],
            altures[2] == -1 ? Random.Range(1, peça.CasesLength) : altures[2]
        };
    }

    [ContextMenu("Crear")]
    void Crear()
    {
        //En comptes d'agafar el Propietat com a valor final per crear la casa.
        //modificarem l'altura i la guardarem perque les seguents cases ho puguin llegir.
        altures[0] = ExtComparat;
        altures[1] = DreComparat;
        altures[2] = EsqComparat;

        for (int i = 0; i < pisos.Length; i++)
        {
            /*altura = 0;
            if (pisos[i].OrientacioFinal == 0) altura = ExtComparat;
            else if (pisos[i].OrientacioFinal == 1) altura = DreComparat;
            else if (pisos[i].OrientacioFinal == 2) altura = EsqComparat;

            pisos[i].Crear(altura);
            */

            if      (pisos[i].OrientacioFinal == 0) pisos[i].Crear(altures[0]);
            else if (pisos[i].OrientacioFinal == 1) pisos[i].Crear(altures[1]);
            else if (pisos[i].OrientacioFinal == 2) pisos[i].Crear(altures[2]);
        }
    }



    private void OnValidate()
    {
        if (pisos == null || pisos.Length == 0) pisos = GetComponentsInChildren<Detall_Pis>();
    }
}
