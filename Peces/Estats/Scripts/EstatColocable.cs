using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Estats/Coloca")]
public class EstatColocable : ScriptableObject
{
    public void Setup(Estat inicial, UI_Peca peça)
    {
        this.inicial = inicial;
        this.peça = peça;
    }

    [SerializeScriptableObject][SerializeField] Estat inicial;
    [SerializeField] UI_Peca peça;


    public Estat Estat => inicial;
    public UI_Peca Prefab => peça;



}

