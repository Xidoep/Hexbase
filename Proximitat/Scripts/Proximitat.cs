using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Proximitat")]
public class Proximitat : ScriptableObject
{
    //ACTIVADORS
    //1.- Si estas aprop d'1 o + peces d'un tipus. i d'un nivell concret o de qualsevol.
    //2.- Si formen part d'un grup de X o +.
    //3.- Com l'1, pero en tot el grup.
    [SerializeField] Estat[] estats;

    [SerializeField] Queue<Peça> peces;

    bool start;

    void OnEnable()
    {
        peces = new Queue<Peça>();
    }
    public void Add(Peça peça)
    {
        if(!peces.Contains(peça)) peces.Enqueue(peça);
        //if not started start the proces.
        if (!start) Process();
    }

    void Process()
    {
        start = true;
    }

    [System.Serializable]
    public class Estat
    {
        public EstatPeça estat;
        public Condicio[] condicions;
    }

    [System.Serializable]
    public class Condicio
    {
        public EstatPeça[] buscats;
        public bool utilitzarGrup;
        public int nivell;
        public int resultat;
    }
}


