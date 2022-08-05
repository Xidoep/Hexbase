using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/Detall")]
public class DetallScriptable : ScriptableObject
{
    [SerializeField] Detall_GameObject gameObject;
    [SerializeField] Detall_Tiles tiles;
    [SerializeField] Detall_Modificacio[] modificacions;



    [Tooltip("El/s gameobject/s que es crearan sobre la peça.")]
    public GameObject GameObject(Peça peça, TilePotencial tile) => gameObject.Get(peça, tile);



    [Tooltip("A quins tiles de la peça (0-5) es crearan. Ex: 0; 0,1,2,3,4,5; 1,2,4...")]
    public int[] Tiles(Peça peça) => tiles.Get(peça);



    public Detall_Modificacio[] Modificacios => modificacions;
}
