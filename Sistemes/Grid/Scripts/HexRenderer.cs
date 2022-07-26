using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class HexRenderer : MonoBehaviour
{
    Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    List<Face> faces;

    [SerializeField] float innerSize;
    [SerializeField] float outerSize;
    [SerializeField] float height;
    [SerializeField] bool isFlatTopped;
    [SerializeField] Material material;

    public void Setup()
    {

    }


    void OnEnable()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        mesh = new Mesh();
        mesh.name = "Hex";

        meshFilter.mesh = mesh;
        meshRenderer.material = material;

        DrawMesh();
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            DrawMesh();
        }
    }


    public void DrawMesh(float innerSize, float outerSize, float height, bool isFlatTopped, Material material)
    {
        this.innerSize = innerSize;
        this.outerSize = outerSize;
        this.height = height;
        this.isFlatTopped = isFlatTopped;
        meshRenderer.material = material;
        DrawMesh();
    }
    void DrawMesh()
    {
        DrawFaces();
        CombineFaces();
    }


    void DrawFaces()
    {
        faces = new List<Face>();

        //Top faces
        for (int point = 0; point < 6; point++)
        {
            faces.Add(CreateFace(innerSize, outerSize, height / 2f, height / 2f, point));
        }
    }

    Vector3 GetPoint(float size, float heihgt, int index)
    {
        float angle_deg = 60 * index - (isFlatTopped ? 0 : 30);
        float angle_rad = Mathf.PI / 180f * angle_deg;
        return new Vector3((size * Mathf.Cos(angle_rad)), heihgt, size * Mathf.Sin(angle_rad));
    }
    Face CreateFace(float innerRad, float outerRad, float heightA, float heightB, int point, bool reverse = false)
    {
        Vector3 _pointA = GetPoint(innerRad, heightB, point);
        Vector3 _pointB = GetPoint(innerRad, heightB, (point < 5) ? point + 1 : 0);
        Vector3 _pointC = GetPoint(outerRad, heightA, (point < 5) ? point + 1 : 0);
        Vector3 _pointD = GetPoint(outerRad, heightA, point);

        List<Vector3> _vertices = new List<Vector3>() { _pointA, _pointB, _pointC, _pointD };
        List<int> _trianges = new List<int>() { 0, 1, 2, 2, 3, 0 };
        List<Vector2> _uvs = new List<Vector2>() { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
        if (reverse)
        {
            _vertices.Reverse();
        }
        return new Face(_vertices, _trianges, _uvs);
    }

    void CombineFaces()
    {
        List<Vector3> _vertices = new List<Vector3>();
        List<int> _triangles = new List<int>();
        List<Vector2> _uvs = new List<Vector2>();

        for (int i = 0; i < faces.Count; i++)
        {
            _vertices.AddRange(faces[i].vertices);
            _uvs.AddRange(faces[i].uvs);

            int _offset = (4 * i);
            foreach (var _triangle in faces[i].triangles)
            {
                _triangles.Add(_triangle + _offset);
            }
        }

        mesh.vertices = _vertices.ToArray();
        mesh.triangles = _triangles.ToArray();
        mesh.uv = _uvs.ToArray();
        mesh.RecalculateNormals();
    }



    public struct Face
    {
        public List<Vector3> vertices { get; private set; }
        public List<int> triangles { get; private set; }
        public List<Vector2> uvs { get; private set; }

        public Face(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
        {
            this.vertices = vertices;
            this.triangles = triangles;
            this.uvs = uvs;
        }
    }
}

