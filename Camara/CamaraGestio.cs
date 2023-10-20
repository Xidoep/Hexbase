using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using XS_Utils;
using Cinemachine;

/// <summary>
/// Gestiona el moviment, rotacio i zoom de la camara. I els seus limits.
/// </summary>
public class CamaraGestio : MonoBehaviour
{
    [SerializeField] Transform objectiu;

    [Space(10)]
    [SerializeField] Transform nord;
    [SerializeField] Transform sud;
    [SerializeField] Transform est;
    [SerializeField] Transform oest;


    [Linia]
    [SerializeField] CamaraMoviment camMovement;
    [Linia]
    [SerializeField] CamaraZoom camZoom;
    [Linia]
    [SerializeField] CamaraRotacio camRotation;

    public float Zoom => camZoom.Zoom;

    private void OnEnable()
    {
        camMovement.Enable();
        camZoom.Enable();
        camRotation.Enable();

        Grid.Instance.SetEnDimensionar = SetDimensions;
    }
    void Start()
    {
        camMovement.Start(objectiu);
        camZoom.Start();
        camRotation.Start(objectiu);
    }
    void Update()
    {
        camMovement.Update(objectiu);
        camRotation.Update(objectiu);
        camZoom.Update();
    }
    void OnDisable()
    {
        camMovement.Disable();
        camZoom.Disable();
        camRotation.Disable();
    }

    void SetDimensions(Hexagon nord, Hexagon sud, Hexagon est, Hexagon oest)
    {
        if (nord == null)
            return;

        this.nord = nord.transform;
        this.sud = sud.transform;
        this.est = est.transform;
        this.oest = oest.transform;

        camMovement.Centrar(this.nord, this.sud, this.est, this.oest);
        camMovement.Limits = new Vector4(this.est.position.x, this.nord.position.z, this.oest.position.x, this.sud.position.z);
        camZoom.Dimensions = new Vector2(this.oest.position.x - this.oest.position.x, this.nord.position.z - this.sud.position.z);
    }

    void OnDrawGizmos()
    {
        if (nord == null)
            return;
        Gizmos.color = Color.red - (Color.black * .5f);

        Gizmos.DrawWireCube(new Vector3((est.position.x + oest.position.x) * 0.5f, 0, (nord.position.z + sud.position.z) * 0.5f),
            new Vector3(oest.transform.position.x - est.transform.position.x, 1, sud.transform.position.z - nord.transform.position.z)
            );

    }

}
