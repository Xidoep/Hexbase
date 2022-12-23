using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

[CreateAssetMenu(menuName = "Xido Studio/Hex/Visualitzacions")]
public class Visualitzacions : ScriptableObject
{
    [SerializeField] Animacions animacions;
    [Space(20)]
    [SerializeField] Particules particules;

    public List<Vector3> posicions;

    Transform camera;

    [System.Serializable]
    public struct Animacions
    {
        [Header("PRODUCCIO")]
        public Animacio_Scriptable producteProveir;
        public Animacio_Scriptable necessitatProveida;

        [Apartat("PECES UI")]
        public Animacio_Scriptable primeraPosicio;
        public Animacio_Scriptable segonaPosicio;
        public Animacio_Scriptable segonaPosicioParent;
        public Animacio_Scriptable colocarPeça;
        public Animacio_Scriptable colocarPeçaParent;

        [Apartat("INFORMACIO")]
        public Animacio_Scriptable amagarInformacio;

        [Apartat("NIVELL")]
        public Animacio_Scriptable guanyarExperiencia;

        [Apartat("PECES")]
        public Animacio_Scriptable canviarEstat;
    }

    [System.Serializable]
    public struct Particules
    {
        [Header("PUNTS")]
        public Utils_InstantiableFromProject guanyarPunts;

    }


    public void CanviarEstat(Peça peça) => animacions.canviarEstat.Play(peça.Parent);

    public void PrimeraPosicio(Transform transform) => animacions.primeraPosicio.Play(transform);
    public void SegonaPosicio(Transform transform) 
    {
        animacions.segonaPosicio.Play(transform);
        animacions.segonaPosicioParent.Play(transform.parent.GetComponent<RectTransform>());
    } 

    public void ColocarPeça(Transform transform) 
    {
        animacions.colocarPeça.Play(transform);
        animacions.colocarPeçaParent.Play(transform.parent.GetComponent<RectTransform>());
    }

    public void AddGuanyarPunts(Vector3 posicio)
    {
        if (posicions == null) posicions = new List<Vector3>();

        posicions.Add(posicio);
    }
   
    public void GuanyarPunts(float delay)
    {
        if (camera == null) camera = Camera.main.transform;

        XS_Coroutine.StartCoroutine_Ending(delay, Animacio);

        void Animacio() 
        {
            for (int i = 0; i < posicions.Count; i++)
            {
                particules.guanyarPunts.Instantiate(posicions[0], Quaternion.Euler(camera.forward));
                posicions.RemoveAt(0);
            }
        } 
    }

    public void AmagarInformacio(GameObject gameObject)
    {
        animacions.amagarInformacio.Play(gameObject);
        Destroy(gameObject, 0.51f);
    }

    public void GuanyarExperiencia(Transform transform) => animacions.guanyarExperiencia.Play(transform);

    public void Produccio(GameObject producte, Peça productor, float indexProducte, Peça casa, int indexNecessitat)
    {
        XS_Coroutine.StartCoroutine_Ending(0.55f + (indexProducte * 0.3f), Animacio);

        void Animacio()
        {
            if (productor == null)
            {
                DestruirProducte(producte);
            }
            else
            {
                ExtreureProducte(producte, productor);

                if (casa != null)
                    RepartirProducte(producte, productor.transform, casa, indexNecessitat);
                else
                    DestruirProducte(producte);
            }
        }

        void ExtreureProducte(GameObject producte, Peça productor)
        {
            Vector3 offset = producte.transform.localPosition;
            new Animacio_Posicio(productor.Extraccio.transform.position + offset, productor.transform.position + offset, false, false).Play(producte, 0.5f, Transicio.clamp);
        }
        void RepartirProducte(GameObject producte, Transform productor, Peça casa, int indexNecessitat)
        {
            XS_Coroutine.StartCoroutine_Ending(0.75f, Animacio);

            void Animacio()
            {
                float temps = Vector3.Distance(productor.position, casa.transform.position) * 0.5f;

                new Animacio_Posicio(productor.position, casa.transform.position, false, false).Play(producte, temps, Transicio.clamp);

                ProducteProveir(producte, temps + 1);
                NecessitatProveida(casa.Get_UINecessitat(indexNecessitat), temps + 1);
            }
        }
        void ProducteProveir(GameObject gameObject, float delay)
        {
            XS_Coroutine.StartCoroutine_Ending(delay, Animacio);

            void Animacio()
            {
                animacions.producteProveir.Play(gameObject);
                Destroy(gameObject, 1.5f);
            }
        }

        void NecessitatProveida(GameObject gameObject, float delay)
        {
            XS_Coroutine.StartCoroutine_Ending(delay, Animacio);

            void Animacio()
            {
                animacions.necessitatProveida.Play(gameObject);
                GuanyarPunts(gameObject.transform.position);
            }
        }

        void GuanyarPunts(Vector3 posicio)
        {
            if (camera == null) camera = Camera.main.transform;
            particules.guanyarPunts.Instantiate(posicio, Quaternion.Euler(camera.forward));
        }
    }
    

    public void DestruirProducte(GameObject producte) => producte.GetComponent<UI_Producte>().Destruir(1.5f);
}
