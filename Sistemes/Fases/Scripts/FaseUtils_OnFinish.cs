using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FaseUtils_OnFinish : MonoBehaviour
{
    [SerializeField] Fase fase;
    [SerializeField] UnityEvent amagar;

    private void OnEnable()
    {
        fase.OnFinish += amagar.Invoke;
    }

    private void OnDisable()
    {
        fase.OnFinish -= amagar.Invoke;
    }
}
