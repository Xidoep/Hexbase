using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Connexions/Connexio")]
public class Connexio : ScriptableObject
{
    [SerializeField] List<Connexio> connexionsViables;
    //public virtual Connexio Viable => this;
    public virtual bool EncaixaAmb(Connexio connexio) => connexionsViables.Contains(connexio);
}