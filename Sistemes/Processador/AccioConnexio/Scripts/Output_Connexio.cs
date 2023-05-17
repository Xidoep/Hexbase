using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Productes/Connexio")]
public class Output_Connexio : MonoBehaviour, IProcessable
{
    [SerializeField] Peça.ConnexioEnum connexio;

    public void Processar(Peça peça)
    {
        throw new System.NotImplementedException();
    }

}
