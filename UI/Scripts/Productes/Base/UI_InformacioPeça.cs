using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UI_InformacioPeça : MonoBehaviour
{
    protected const string ICONE = "_Icone";
    protected const string COVERTA = "_Coverta";
    protected const string GASTADA = "_Gastada";
    protected const string PTENCIAL = "_Potencial";
    protected const string START_TIME = "_StartTime";
    public abstract GameObject Setup(Peça peça, int index);

    [SerializeField] MeshRenderer meshRenderer;


    protected MeshRenderer MeshRenderer => meshRenderer;
}
