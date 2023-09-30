using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    MeshRenderer mr;

    public Material material;

    //Varje triangel består av 3 punkter = Vector 3
    Vector3[] vertices = new Vector3[]
        {
            new Vector3(0,0,0),
            new Vector3(0,1,0),
            new Vector3(1,0,0),            
            new Vector3(1,1,0),
            new Vector3(1,0,1),
            new Vector3(1,1,1),
            new Vector3(0,0,1),
            new Vector3(0,1,1)
        };

    //Måste lägga in punkterna klockvis pga culling mask
    int[] triangles = new int[]
    {
            0,1,2,
            1,3,2,
            2,3,4,
            3,5,4,
            4,5,6,
            5,7,6,
            6,7,0,
            7,1,0
    };


    void Start()
    {
        mr = GetComponent<MeshRenderer>();

        //skapa en mesh
        mesh = new Mesh();

        //sätt meshen i obbjektets mesh filter
        GetComponent<MeshFilter>().mesh = mesh;

        //sätt meshens shape
        UpdateMesh();
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mr.material = material;
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            if(vertices[i].y > 0)
            {
                vertices[i].y += 0.1f * Time.deltaTime;
            }
        }

        UpdateMesh();
    }
}
