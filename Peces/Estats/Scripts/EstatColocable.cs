using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Estats/Coloca")]
public class EstatColocable : ScriptableObject
{
    [SerializeScriptableObject][SerializeField] Estat inicial;
    [SerializeField] UI_Peca pe�a;


    public Estat SubestatInicial => inicial;
    public UI_Peca Prefab => pe�a;



}

