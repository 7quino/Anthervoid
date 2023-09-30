using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Branch5 : MonoBehaviour
{
    public int id;

    //mesh
    public Mesh mesh;
    public MeshRenderer meshRenderer;
    public MeshGenerator12 meshGenerator;

    //Branch Variables
    public Vector3 centerVector = Vector3.zero;
    public Vector3 spaceVector = Vector3.zero;
    public Vector3 rotateAxis = Vector3.up;
    public Vector3 bendAxis = Vector3.forward;
    public Vector3 radiusV = Vector3.one;
    public Vector3 lenghtV = Vector3.one;
    public float curRotation = 0;
    public float curBend = 0;
    public int generations = 0;
    public int curGen = 0;
    public float randomFactor = 0;
    public bool stem = true;

    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Vector3> lerpStartPos = new List<Vector3>();
    public List<Vector3> lerpEndPos = new List<Vector3>();

    //Rules
    public LSystemBranch branch = LSystemBranch.A;
    public List<char> rules = new List<char>();
    public int ruleIndex = 0;
    public char curRule;

    void Start()
    {
        assignMesh();

        if (id == 1) transform.eulerAngles = new Vector3 (0, 180, 0);

        if (id == 0 || id == 1)
        {
            meshGenerator.CreateBaseShape(id);
            RunNextRule();
        }
    }

    public void assignMesh()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void RunNextRule()
    {
        if (curGen >= generations)
        { 
            meshGenerator.EndSegment(id, true);
            return; 
        }

        UpdateRuleIndex();
        if (ruleIndex >= rules.Count + 1) return;

        switch (curRule)
        {
            case '"': meshGenerator.Scale(id); break;
            case '^': meshGenerator.Bend(id, 1); break;
            case '&': meshGenerator.Bend(id, -1); break;
            case '|': meshGenerator.Rotation(id, 1); break;
            case '/': meshGenerator.Rotation(id, -1); break;
            case 'f': meshGenerator.Forward(id); break;
            case 'j': meshGenerator.EndSegment(id, false); break;
            case 'k': meshGenerator.EndSegment(id, true); break;
            case 'A': meshGenerator.AddBranch(id, LSystemBranch.A); break;
            case 'B': meshGenerator.AddBranch(id, LSystemBranch.B); break;
            case 'C': meshGenerator.AddBranch(id, LSystemBranch.C); break;
            case 'D': meshGenerator.AddBranch(id, LSystemBranch.D); break;
            case 'E': meshGenerator.AddBranch(id, LSystemBranch.E); break;
            case 'F': meshGenerator.AddBranch(id, LSystemBranch.F); break;
            case 'G': meshGenerator.AddBranch(id, LSystemBranch.G); break;
            case 'H': meshGenerator.AddBranch(id, LSystemBranch.H); break;
        }
    }

    void UpdateRuleIndex()
    {
        if (ruleIndex < rules.Count)
            curRule = rules[ruleIndex];
        ruleIndex++;
    }

    public void UpdateMesh(Material material)
    {
        mesh.Clear();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        meshRenderer.material = material;
    }
}
