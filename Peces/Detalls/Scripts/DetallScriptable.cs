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



    [Tooltip("El/s gameobject/s que es crearan sobre la pe�a.")]
    public GameObject GameObject(Pe�a pe�a, TilePotencial tile) => gameObject.Get(pe�a, tile);



    [Tooltip("A quins tiles de la pe�a (0-5) es crearan. Ex: 0; 0,1,2,3,4,5; 1,2,4...")]
    public int[] Tiles(Pe�a pe�a) => tiles.Get(pe�a);



    public Detall_Modificacio[] Modificacios => modificacions;
}
