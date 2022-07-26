using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Pool")]
public class PoolPeces : ScriptableObject
{
    [SerializeField] List<Estat> peces;
    [Linia]
    [SerializeField] Estat[] estats;


    public void Inicialize(int peces)
    {
        this.peces = new List<Estat>();
        for (int i = 0; i < peces; i++)
        {
            this.peces.Add(estats[Random.Range(0, estats.Length)]);
        }
    }
    public void Inicialize(List<Estat> peces)
    {
        this.peces = peces;
    }

    public void Add(int quantitat) 
    {
        for (int i = 0; i < quantitat; i++)
        {
            Add(estats[Random.Range(0, estats.Length)]);
        }       
    }
    public void Add(Estat estat) => peces.Add(estat);
    public Estat Get
    {
        get
        {
            Estat estat = peces[0];
            peces.RemoveAt(0);
            return estat;
        }
    }

    private void OnValidate()
    {
        estats = XS_Editor.LoadAllAssetsAtPath<Estat>("Assets/XidoStudio/Hexbase/Estats/Estats").ToArray();
    }
}
