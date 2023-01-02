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

    public void Produccio(Visualitzacions.Producte v, bool ultima, System.Action enFinalitzar)
    {
        XS_Coroutine.StartCoroutine_Ending(0.55f + (v.indexProducte * 0.3f), Animacio);
        //v.productor.Extraccio.mostrarInformacio?.Invoke(v.productor, true);
        //v.productor.Extraccio.BlocarInformacio = true;

        void Animacio()
        {
            
            if (v.productor == null)
            {
                DestruirProducte(v.productor, v.indexProducte, ultima, enFinalitzar);
            }
            else
            {
                ExtreureProducte(v.productor, v.indexProducte);

                if (v.casa != null)
                    RepartirProducte(v.productor, v.indexProducte, v.casa, v.indexNecessitat, ultima, enFinalitzar);
                else
                    DestruirProducte(v.productor, v.indexProducte, ultima, enFinalitzar);
            }
        }

        void ExtreureProducte(Peça p, int i)
        {
            GameObject producte = p.Extraccio.productesExtrets[i].informacio.gameObject;

            Vector3 offset = producte.transform.localPosition;
            new Animacio_Posicio(p.Extraccio.transform.position + offset, p.transform.position + offset, false, false).Play(producte, 0.5f, Transicio.clamp);
        }
        void RepartirProducte(Peça p, int iProd, Peça c, int iNeed, bool ultima, System.Action enFinalitzar)
        {
            XS_Coroutine.StartCoroutine_Ending(0.75f, Animacio);

            void Animacio()
            {
                //c.mostrarInformacio?.Invoke(c, true);
                //c.BlocarInformacio = true;

                GameObject producte = p.Extraccio.productesExtrets[iProd].informacio.gameObject;

                float temps = Vector3.Distance(p.transform.position, c.transform.position) * 0.25f;

                new Animacio_Posicio(p.transform.position, c.transform.position, false, false).Play(producte, temps, Transicio.clamp);

                ProducteProveir(p, producte, temps + 1);
                NecessitatProveida(c, c.Casa.Necessitats[iNeed].Informacio.gameObject, temps + 1, ultima, enFinalitzar);
            }
        }
        void ProducteProveir(Peça p, GameObject ui, float delay)
        {
            XS_Coroutine.StartCoroutine_Ending(delay, Animacio);

            void Animacio()
            {
                animacions.producteProveir.Play(ui);
                //Destroy(gameObject, 1.5f);
                p.Extraccio.BlocarInformacio = false;
                //p.Extraccio.mostrarInformacio?.Invoke(p.Extraccio, false);
            }
        }

        void NecessitatProveida(Peça c, GameObject ui, float delay, bool ultima, System.Action enFinalitzar)
        {
            XS_Coroutine.StartCoroutine_Ending(delay, Animacio);

            void Animacio()
            {
                animacions.necessitatProveida.Play(ui);
                GuanyarPunts(ui.transform.position);
                c.BlocarInformacio = false;
                c.mostrarInformacio?.Invoke(c, false);

                if(ultima)
                    XS_Coroutine.StartCoroutine_Ending(0.5f, enFinalitzar);
            }
        }

        void GuanyarPunts(Vector3 posicio)
        {
            if (camera == null) camera = Camera.main.transform;
            particules.guanyarPunts.Instantiate(posicio, Quaternion.Euler(camera.forward));
        }
    }


    public void DestruirProducte(Peça p, int i, bool ultima, System.Action enFinalitzar) 
    {

        p.Extraccio.productesExtrets[i].informacio.gameObject.GetComponent<UI_Producte>().Destruir(1.5f);
        XS_Coroutine.StartCoroutine_Ending(1.6f, RemostrarLaInformacio);

        void RemostrarLaInformacio()
        {
            p.Extraccio.BlocarInformacio = false;

            if (ultima)
                enFinalitzar.Invoke();
            //productor.Extraccio.mostrarInformacio?.Invoke(productor.Extraccio.Informacio, productor, false);
        }
        
    }

    [System.Serializable]
    public struct Producte
    {
        public Producte(Peça productor, int indexProducte, Peça casa, int indexNecessitat)
        {
            this.productor = productor;
            this.indexProducte = indexProducte;
            this.casa = casa;
            this.indexNecessitat = indexNecessitat;
        }
        public Peça productor;
        public int indexProducte;
        public Peça casa;
        public int indexNecessitat;
    }
}
