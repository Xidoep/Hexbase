using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Estats/Basic")]
public class Estat : ScriptableObject
{
    [SerializeScriptableObject][SerializeField] Subestat inicial;
    [SerializeField] UI_Peca pe�a;


    public Subestat SubestatInicial => inicial;
    public UI_Peca Prefab => pe�a;



}

