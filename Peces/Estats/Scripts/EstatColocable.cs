using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Estats/Coloca")]
public class EstatColocable : ScriptableObject
{
    public void Setup(Estat inicial, UI_Peca pe�a)
    {
        this.inicial = inicial;
        this.pe�a = pe�a;
    }

    [SerializeScriptableObject][SerializeField] Estat inicial;
    [SerializeField] UI_Peca pe�a;


    public Estat Estat => inicial;
    public UI_Peca Prefab => pe�a;



}

