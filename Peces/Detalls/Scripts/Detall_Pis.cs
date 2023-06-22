using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;
using Sirenix.OdinInspector;

public class Detall_Pis : Detall
{
    public override void Setup(string[] orientacio)
    {
        if (orientacio[0] == "Ext") this.orientacioCasa = 0;
        else if (orientacio[0] == "Esq") this.orientacioCasa = 1;
        else if (orientacio[0] == "Dre") this.orientacioCasa = 2;
    }

    [SerializeField] public int orientacioCasa = -1;
    [SerializeField] public int orientacioFisica = -1;
    [SerializeField] public int altura;
    [SerializeField, ReadOnly] GameObject[] pisos;
    [SerializeField, ReadOnly] GameObject[] sostres;

    public void SetAltura(int altura, TilePotencial tile)
    {
        this.altura = altura;
        if (tile.Veins[0] != null && tile.Veins[0].Peça.TeCasa)
            this.altura = Mathf.Min(this.altura, tile.Veins[0].Peça.CasesLength);

        if (tile.Veins[1].TileFisic.TryGetComponent(out Detall_Pis pisD))
            this.altura = Mathf.Min(this.altura, pisD.altura);
        
        if (tile.Veins[2].TileFisic.TryGetComponent(out Detall_Pis pisE))
            this.altura = Mathf.Min(this.altura, pisE.altura);
        
    }
    public void Crear(int altura, TilePotencial tile)
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
