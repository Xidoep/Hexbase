using UnityEngine;
using XS_Utils;
[CreateAssetMenu(menuName = "Xido Studio/Hex/Fase/Controller")]
public class FasesControlador : ScriptableObject
{
    [SerializeField] Fase actual;
    public Fase Actual
    {
        set {
            if(actual != null) Debugar.LogError($"{actual.name.ToUpper()}>>>");
            actual?.Finalitzar();
            actual = value;
            Debugar.LogError($">>>{actual.name.ToUpper()}");
            actual.Inicialitzar();
        }
    }
    private void OnDisable() => actual = null;
}

[System.Serializable]
public abstract class Fase : ScriptableObject
{
    [SerializeField] FasesControlador controlador;
    System.Action onFinish;
    void OnFinish_Invocar() => onFinish?.Invoke();
    protected object arg;
    public System.Action OnFinish { get => onFinish; set => onFinish = value; }


    public void Iniciar() => controlador.Actual = this;
    public void Iniciar(object arg = null)
    {
        this.arg = arg;
        controlador.Actual = this;
    }
    public abstract void Inicialitzar();
    public void Finalitzar() => OnFinish_Invocar();
    protected void OnDisable() => onFinish = null;
}