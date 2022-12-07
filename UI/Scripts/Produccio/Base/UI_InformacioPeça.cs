using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UI_InformacioPeça : MonoBehaviour
{
    protected const string ICONE_NOM = "_Icone";
    protected const string COVERTA_NOM = "_Coverta";
    public abstract GameObject Setup(Peça peça, int index);

    [SerializeField] MeshRenderer meshRenderer;

    protected MeshRenderer MeshRenderer => meshRenderer;
}
