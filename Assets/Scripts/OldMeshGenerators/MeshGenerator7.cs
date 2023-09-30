using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MeshGenerator7 : MonoBehaviour
{
    //mesh
    public Mesh mesh;
    public MeshRenderer meshRenderer;
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public Material material;
    public List<char> rules = new List<char>();

    public int sides = 3;
    public float radius = 0.1f;
    public float lenght = 1.0f;
    public float rotation = 0;
    public float bend = 0;
    public float tapering = 1;

    Vector3 centerVector = Vector3.zero;
    Vector3 rotateAxis = Vector3.up;
    Vector3 bendAxis = Vector3.forward;
    Vector3 radiusV = Vector3.one;
    Vector3 lenghtV = Vector3.one;
    float sideAngle = 0;
    float curBend = 0;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        

        CreateBase();
        Forward();
    }

    void Rotation()
    {
        bendAxis = Quaternion.AngleAxis(rotation, rotateAxis) * bendAxis;
    }

    void BendUpDown()
    {
        curBend += bend;
    }

    void Forward()
    {
        bendAxis = Quaternion.AngleAxis(rotation, rotateAxis) * bendAxis;
        curBend += bend;
        centerVector += Quaternion.AngleAxis(curBend, bendAxis) * lenghtV;
        radiusV *= tapering;

        //Add vertices
        for (int i = 0; i < sides;  i++)
        {
            vertices.Add((Quaternion.AngleAxis(sideAngle * i, centerVector) * radiusV) + centerVector);
        }

        //Add triangles
        for (int i = 0; i < sides; i++)
        {
            int index = vertices.Count - sides * 2 + i;

            //Bottom triangle
            triangles.Add(index);
            triangles.Add(index + sides);

            if (i == sides - 1)
                triangles.Add(vertices.Count - sides * 2);
            else
                triangles.Add(index + 1);

            //Top triangle
            triangles.Add(index + sides);

            if (i == sides - 1)
            {
                triangles.Add(vertices.Count - sides);
                triangles.Add(vertices.Count - sides * 2);
            }
            else
            {
                triangles.Add(index + sides + 1);
                triangles.Add(index + 1);
            }
        }

        UpdateMesh();
    }

    public void CreateBase()
    {
        lenghtV = new Vector3(0, lenght, 0);
        radiusV *= radius;
        sideAngle = 360 / sides;

        for (int i = 0; i < sides; i++)
        {
            vertices.Add(Quaternion.AngleAxis(sideAngle * i, rotateAxis) * radiusV);
        }
    }

    public void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        meshRenderer.material = material;
    }
}
