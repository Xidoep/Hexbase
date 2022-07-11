using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Casa : System.Object
{
    public Casa(Peça peça, int habitants)
    {
        if (this.habitants == null) this.habitants = new List<Habitant>();

        for (int i = 0; i < habitants; i++)
        {
            this.habitants.Add(new Habitant(peça));
        }
    }

    [SerializeField] List<Habitant> habitants;

    //INTERN
    int index = -1;


    public bool TeHabitantDisponible()
    {
        return Disponible != -1;
    }
    public Habitant HabitantDisponible()
    {
        if (Disponible != -1)
            return habitants[Disponible];
        else return null;
    }

    int Disponible
    {
        get
        {
            index = -1;
            for (int i = 0; i < habitants.Count; i++)
            {
                if (!habitants[i].ocupat)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

    }
}
