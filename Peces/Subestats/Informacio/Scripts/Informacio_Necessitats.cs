using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Informacio/Necessitats")]
public class Informacio_Necessitats : Informacio
{
    //INTERN
    List<GameObject> necessitats;

    public override GameObject[] Mostrar(Peça peça) 
    {
        if (necessitats == null)
            necessitats = new List<GameObject>();
        else necessitats.Clear();

        if (!peça.TeCasa)
            return necessitats.ToArray();

        for (int i = 0; i < peça.Casa.Necessitats.Length; i++)
        {
            necessitats.Add(Instantiate(Prefab, peça.transform.position + DesplaçamentLateral(i), Quaternion.identity, peça.transform).GetComponent<UI_InformacioPeça>().Setup(peça, i));
        }

        return necessitats.ToArray();
    }

}
