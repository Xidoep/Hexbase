using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Detalls/Modificacio/Rotacio Exterior")]
public class Detall_Modif_DireccioExterior : Detall_Modificacio
{
    [Nota("Sempre mira cap a fora, l'offset afageix graus a aquesta rotació.")]
    [SerializeField] float offset;
    public override void Modificar(TilePotencial tile, GameObject detall)
    {
        detall.transform.rotation = Quaternion.Euler(0, tile.Orientacio * 60 + offset, 0);

    }
}
