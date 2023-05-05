using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Estats/Basic")]
public class Estat : ScriptableObject
{
    [SerializeScriptableObject][SerializeField] Subestat inicial;
    [SerializeField] GameObject prefab;


    public Subestat SubestatInicial => inicial;
    public GameObject Prefag => prefab;





}

