using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Tiles/Tile Compost")]
public class TileCompost : ScriptableObject
{
    [SerializeField] GameObject prefab;
    [Linia]
    [Header("Tiles")]
    [SerializeField] FakeTile[] tiles;
    [Linia]
    [Nota("Això haurà d'anar a l'estat, perque cada estat pugui tenir possiblitats diferents", NoteType.Error)]
    [SerializeField] int pes;


    public GameObject Prefab => prefab;
    public int Pes => pes;


    //public virtual bool Comprovar(TilePotencial tile, int orientacioFisica, Connexio exterior, Connexio esquerra, Connexio dreta) => true;



    [System.Serializable] public class FakeTile
    {
        [Header("Connexions")]
        [SerializeField] Connexio exterior;
        [SerializeField] Connexio esquerra;
        [SerializeField] Connexio dreta;

        public Connexio Exterior(int orientacioFisica)
        {
            //return Abaix;
            switch (orientacioFisica)
            {
                default:
                    return exterior;
                case 1:
                    return dreta;
                case 2:
                    return esquerra;
            }
        }
        public Connexio Esquerra(int orientacioFisica)
        {
            //return Esquerra;
            switch (orientacioFisica)
            {
                default:
                    return dreta;
                case 1:
                    return esquerra;
                case 2:
                    return exterior;
            }
        }
        public Connexio Dreta(int orientacioFisica)
        {
            //return Dreta;
            switch (orientacioFisica)
            {
                default:
                    return esquerra;
                case 1:
                    return exterior;
                case 2:
                    return dreta;
            }
        }
    }
}
