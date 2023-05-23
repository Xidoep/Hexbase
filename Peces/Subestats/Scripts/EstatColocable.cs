using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Estats/Coloca")]
public class EstatColocable : ScriptableObject
{
    [SerializeScriptableObject][SerializeField] Subestat inicial;
    [SerializeField] UI_Peca peça;


    public Subestat SubestatInicial => inicial;
    public UI_Peca Prefab => peça;



}

