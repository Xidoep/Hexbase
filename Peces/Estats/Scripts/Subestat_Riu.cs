using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Substat/Substat Riu")]
public class Subestat_Riu : Estat
{
    public override Estat Setup(Peça peça)
    {
        return base.Setup(peça);
    }
  
    /*public override Connexio[] ConnexionsNules
    {
        get
        {
            if (peça.VeinsPeça.Count < 2)
                return ConnexionsPossibles;

            return ConnexionsNules;
        }
    }*/

}
