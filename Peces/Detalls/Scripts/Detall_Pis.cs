using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;
using Sirenix.OdinInspector;

public class Detall_Pis : Detall
{
    public override void Setup(string[] arg)
    {
        if (arg[0] == "Ext") this.orientacioCasa = 0;
        else if (arg[0] == "Dre") this.orientacioCasa = 1;
        else if (arg[0] == "Esq") this.orientacioCasa = 2;

        alturaInicial = int.Parse(arg[1]);

        this.orientacioFisica = -1;
        this.altura = 0;
    }

    [SerializeField] public int orientacioCasa = -1;
    [SerializeField] public int alturaInicial;

    [SerializeField, ReadOnly] public int orientacioFisica = -1;
    [SerializeField, ReadOnly] public int altura;
    [ShowInInspector] public int OrientacioFinal
    {
        get
        {
            switch (orientacioFisica * 10 + orientacioCasa)
            {
                case 00:
                    return 0;
                case 01:
                    return 1;
                case 02:
                    return 2;

                case 10:
                    return 2;
                case 11:
                    return 0;
                case 12:
                    return 1;

                case 20:
                    return 1;
                case 21:
                    return 2;
                case 22:
                    return 0;

                default:
                    return -1;
            }
        }
    }
    [SerializeField, ReadOnly] GameObject[] pisos;
    [SerializeField, ReadOnly] GameObject[] sostres;

    public void Crear(int altura)
    {
        this.altura = altura;
        for (int i = 0; i < this.altura; i++)
        {
            Instantiate(i < this.altura - 1 ? pisos[Random.Range(0, pisos.Length)] : sostres[Random.Range(0, sostres.Length)], 
                transform.position + Vector3.up * 0.25f * (i + 1), 
                transform.rotation, 
                transform
                );
        }
    }

    private void OnValidate()
    {
        bool zeroD = XS_Editor.LoadAssetAtPath<GameObject>($"Assets/XidoStudio/Hexbase/Peces/Detalls/{gameObject.name.Substring(0, gameObject.name.Length - 2)}0D.prefab") != null;
        pisos = new GameObject[]
        {
            XS_Editor.LoadAssetAtPath<GameObject>($"Assets/XidoStudio/Hexbase/Peces/Detalls/{gameObject.name.Substring(0,gameObject.name.Length - 2)}{(zeroD ? "0" : "1")}D.prefab"),
            XS_Editor.LoadAssetAtPath<GameObject>($"Assets/XidoStudio/Hexbase/Peces/Detalls/{gameObject.name.Substring(0,gameObject.name.Length - 2)}{(zeroD ? "1" : "2")}D.prefab"),
            XS_Editor.LoadAssetAtPath<GameObject>($"Assets/XidoStudio/Hexbase/Peces/Detalls/{gameObject.name.Substring(0,gameObject.name.Length - 2)}{(zeroD ? "2" : "3")}D.prefab"),
        };
        sostres = new GameObject[]
        {
            XS_Editor.LoadAssetAtPath<GameObject>($"Assets/XidoStudio/Hexbase/Peces/Detalls/{gameObject.name.Substring(0,gameObject.name.Length - 2)}SostreD.prefab"),
        };
    }
}
