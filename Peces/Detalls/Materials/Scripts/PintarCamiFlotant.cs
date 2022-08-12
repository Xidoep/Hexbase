using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XS_Utils;

public class PintarCamiFlotant : MonoBehaviour
{
    //MeshRenderer meshRenderer;
    [SerializeField] LayerMask layerMask;
    MeshFilter meshFilter;

    Vector3[] vertexs;

    Collider[] results;

    private void LateUpdate()
    {
        Pintar();
        Destroy(this);
    }

    public void Pintar()
    {
        meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;
        Vector3[] vertexs = meshFilter.mesh.vertices;
        Color[] colors = new Color[vertexs.Length];

        Debug.LogError($"{vertexs.Length} vertexs i {colors.Length} colors");


        for (int v = 0; v < vertexs.Length; v++)
        {

            if (Physics.OverlapSphere(transform.position + (transform.forward * vertexs[v].z) + (transform.right * vertexs[v].x), 0.15f, layerMask).Length > 0)
                colors[v] = Color.black;
            else colors[v] = Color.white;
            Debug.LogError($"Vertex {v} has color { colors[v].ToString()}");
            Debug.DrawRay(transform.position + (transform.forward * vertexs[v].z) + (transform.right * vertexs[v].x) + (Vector3.up * 0.2f), Vector3.down, Color.red, 20f);
            
        }

        meshFilter.mesh.colors = colors;
        //mesh.colors = colors;
        //meshFilter.mesh = mesh;
    }
    void OnDrawGizmosSelected()
    {
        if(meshFilter == null) meshFilter = GetComponent<MeshFilter>();
        //SMesh mesh = meshFilter.mesh;
        Vector3[] vertexs = meshFilter.sharedMesh.vertices;
        Color[] colors = new Color[vertexs.Length];

        Matrix4x4[] matrix4X4 = new Matrix4x4[vertexs.Length];
        //Matrix4x4 matTrans = transform.worldToLocalMatrix;
        Matrix4x4 matTrans = transform.localToWorldMatrix;


        for (int v = 0; v < vertexs.Length; v++)
        {
            matrix4X4[v] = Matrix4x4.TRS(vertexs[v] + transform.position, Quaternion.identity, Vector3.zero);


            Gizmos.color = Physics.OverlapSphere(transform.position + (transform.forward * vertexs[v].z) + (transform.right * vertexs[v].x), 0.15f, layerMask).Length > 0 ? Color.red : Color.green;
            //Gizmos.DrawCube(transform.position + (transform.forward * vertexs[v].z) + (transform.right * vertexs[v].x), Vector3.one * 0.2f);
            Gizmos.DrawSphere(transform.position + (transform.forward * vertexs[v].z) + (transform.right * vertexs[v].x), 0.15f);
        }
    }
}
