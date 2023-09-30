using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Branch1 : MonoBehaviour
{
    public int id;
    public bool firstInstance = false;

    //mesh
    public Mesh mesh;
    public MeshRenderer meshRenderer;
    //Material material;
    public MeshGenerator8 meshGenerator;

    //Branch Variables
    public Vector3 centerVector = Vector3.zero;
    public Vector3 spaceVector = Vector3.zero;
    public Vector3 rotateAxis = Vector3.up;
    public Vector3 bendAxis = Vector3.forward;
    public Vector3 radiusV = Vector3.one;
    public Vector3 lenghtV = Vector3.one;
    public float curRotation = 0;
    public float curBend = 0;
    public int curGen = 0;

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

        if (firstInstance)
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
        if (curGen >= meshGenerator.generations) return;

        UpdateRuleIndex();
        if (ruleIndex >= rules.Count + 1) return;

        switch (curRule)
        {
            case 'u': meshGenerator.BendUp(id); break;
            case 'd': meshGenerator.BendDown(id); break;
            case 'r': meshGenerator.Rotation(id); break;
            case 'f': meshGenerator.Forward(id); break;
            case 'A': meshGenerator.RuleABCD(id, LSystemBranch.A); break;
            case 'B': meshGenerator.RuleABCD(id, LSystemBranch.B); break;
            case 'C': meshGenerator.RuleABCD(id, LSystemBranch.C); break;
            case 'D': meshGenerator.RuleABCD(id, LSystemBranch.D); break;
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
        meshRenderer.material = material;
    }
}
