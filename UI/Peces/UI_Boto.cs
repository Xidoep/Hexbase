using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UI_Boto : MonoBehaviour, IPointerExitHandler
{
    [SerializeField] UnityEvent OnExit;
    public void OnPointerExit(PointerEventData eventData)
    {
        OnExit.Invoke();
    }

}
