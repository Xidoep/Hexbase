using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/GameObject/One")]
public class Detall_GameObject : ScriptableObject
{
    [SerializeField] protected GameObject[] detalls;
    public virtual GameObject Get(Peça peça, TilePotencial tile) => detalls[0];


    [System.Serializable]
    public struct Dependencia
    {

        public Tile tile;
        [Tooltip("Si es -1, no ho tindrà en compte")] public int orientacioFisica;
        public int indexDetall;

        public bool Cohincideix(TilePotencial tilePotencial) => tilePotencial.PossibilitatsVirtuals.Get(0).Tile == tile && (orientacioFisica == -1 || tilePotencial.OrientacioFisica == orientacioFisica);
    }
}
