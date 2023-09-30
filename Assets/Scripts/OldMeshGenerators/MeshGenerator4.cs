using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class MeshGenerator4 : MonoBehaviour
{
    public int sides = 3;
    public int segments = 1;
    public float length = 1;
    public float diameter = 1;
    public float tapering = 1;
    public float bend = 0;
    public float rotation = 0;
    public float randomness = 0;
    public float GrowthSpeed = 10;
    public int generations = 1;
    public Material material;

    public string ruleA;
    public string ruleB;
    public string ruleC;
    public string ruleD;

    //Private system properties
    List<char> ruleListA = new List<char>();
    List<char> ruleListB = new List<char>();
    List<char> ruleListC = new List<char>();
    List<char> ruleListD = new List<char>();
    //char curRule;
    int curGeneration;

    List<MeshSegment> meshSegments = new List<MeshSegment>();
    List<int> branchIndex = new List<int>();

    //mesh
    Mesh mesh;
    MeshRenderer mr;
    List<Vector3> meshVertices = new List<Vector3>();
    List<int> meshTriangles = new List<int>();


    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        //TestCalculation();

        CreateBaseShape();
        CreateRuleQueues();
        RunNextRule(0);
    }

    void CreateBaseShape()
    {
        meshSegments.Add(new MeshSegment());
        meshSegments[0].sidesAngle = 360 / sides;
        meshSegments[0].radius = diameter / 2;

        //Add verticies
        float p1x = meshSegments[0].radius * math.sin(meshSegments[0].sidesAngle * math.PI / 180.0f);
        float p1y = meshSegments[0].radius * math.cos(meshSegments[0].sidesAngle * math.PI / 180.0f);
        meshSegments[0].vertices.Add(new Vector3(p1x, 0, p1y));

        for (int i = 1; i < sides; i++)
        {
            float px = p1x * math.cos(meshSegments[0].sidesAngle * i * math.PI / 180.0f) - p1y * math.sin(meshSegments[0].sidesAngle * i * math.PI / 180.0f);
            float py = p1x * math.sin(meshSegments[0].sidesAngle * i * math.PI / 180.0f) + p1y * math.cos(meshSegments[0].sidesAngle * i * math.PI / 180.0f);
            meshSegments[0].vertices.Add(new Vector3(px, 0, py));
        }
    }

    void CreateRuleQueues()
    {
        CreateQueue(ruleA, ruleListA);
        CreateQueue(ruleB, ruleListB);
        CreateQueue(ruleC, ruleListC);
        CreateQueue(ruleD, ruleListD);
    }

    void CreateQueue(string rules, List<char> list)
    {
        if (rules.Length == 0) return;
        for (int i = 0; i < rules.Length; i++)
        {
            list.Add((char)rules[i]);
        }
    }

    void RunNextRule(int index)
    {
        switch (meshSegments[index].branch)
        {
            case LSystemBranch.A:
                {
                    UpdateRuleIndex(index, ref meshSegments[index].indexA, ruleListA);
                    if (meshSegments[index].indexA >= ruleListA.Count + 1) return;
                    break;
                }
            case LSystemBranch.B:
                {
                    UpdateRuleIndex(index, ref meshSegments[index].indexB, ruleListB);
                    if (meshSegments[index].indexB >= ruleListB.Count + 1) return;
                    break;
                } 
            case LSystemBranch.C:
                {
                    UpdateRuleIndex(index, ref meshSegments[index].indexC, ruleListC);
                    if (meshSegments[index].indexC >= ruleListC.Count + 1) return;
                    break;
                }
            case LSystemBranch.D:
                {
                    UpdateRuleIndex(index, ref meshSegments[index].indexD, ruleListD);
                    if (meshSegments[index].indexD >= ruleListD.Count + 1) return;
                    break;
                }
        }


        switch (meshSegments[index].curRule)
        {
            case 'u': RuleUpDown(index, -bend); break;
            case 'd': RuleUpDown(index, bend); break;
            case 'r': RuleRotate(index); break;
            case 'f': RuleForward(index); break;
            case 'A': RuleA(index); break;
            case 'B': RuleBCD(index, LSystemBranch.B); break;
            case 'C': RuleBCD(index, LSystemBranch.C); break;
            case 'D': RuleBCD(index, LSystemBranch.D); break;
        }
    }

    void UpdateRuleIndex(int index, ref int indexRuleList, List<char> ruleList)
    {
        if (indexRuleList < ruleList.Count)
            meshSegments[index].curRule = ruleList[indexRuleList];
        indexRuleList++;
    }

    void RuleUpDown(int index, float bend)
    {
        meshSegments[index].curBend += bend;
        RunNextRule(index);
    }

    void RuleRotate(int index)
    {
        meshSegments[index].curRotation += rotation;
        RunNextRule(index);
    }

    void RuleForward(int index)
    {
        AddSegment(index);
        CalculateSegmentEndPos(index);
        StartCoroutine(GrowForward(index));
    }

    void RuleA(int index)
    {
        meshSegments[index].indexA = 0;

        if (meshSegments[index].branch == LSystemBranch.A)
        {
            curGeneration++;
            if (curGeneration == meshSegments.Count) return;
        }

        meshSegments[index].branch = LSystemBranch.A;
        RunNextRule(index);
    }

    void RuleBCD(int index, LSystemBranch branch)
    {
        int newIndex = index + 1;

        meshSegments.Add(new MeshSegment());
        meshSegments[newIndex].branch = branch;
        AddBranchBaseShape(newIndex);

        RunNextRule(index);
        RunNextRule(newIndex);
    }

    void AddBranchBaseShape(int newIndex)
    {
        meshSegments[newIndex].sidesAngle = 360 / sides;
        meshSegments[newIndex].radius = meshSegments[newIndex - 1].radius;
        meshSegments[newIndex].curBend = meshSegments[newIndex - 1].curBend;
        meshSegments[newIndex].curRotation = meshSegments[newIndex - 1].curRotation;
        meshSegments[newIndex].curTapering = meshSegments[newIndex - 1].curTapering;

        for (int i = 0; i < sides; i++)
        {
            int index = meshSegments[newIndex - 1].vertices.Count - sides;
            meshSegments[newIndex].vertices.Add(new Vector3(meshSegments[newIndex - 1].vertices[index].x, meshSegments[newIndex - 1].vertices[index].y, meshSegments[newIndex - 1].vertices[index].z));
            meshSegments[newIndex].lerpStartPos.Add(meshSegments[newIndex].vertices[i]);
        }
    }


    void AddSegment(int msIndex)
    {
        meshSegments[msIndex].lerpStartPos.Clear();

        //Add verticies
        for (int i = 0; i < sides; i++)
        {
            int index = meshSegments[msIndex].vertices.Count - sides;
            meshSegments[msIndex].vertices.Add(new Vector3(meshSegments[msIndex].vertices[index].x, meshSegments[msIndex].vertices[index].y, meshSegments[msIndex].vertices[index].z));
            meshSegments[msIndex].lerpStartPos.Add(meshSegments[msIndex].vertices[index]);
        }



        for (int i = 0; i < sides; i++)
        {
            int index = meshSegments[msIndex].vertices.Count - sides * 2 + i;

            //Bottom triangle
            meshSegments[msIndex].triangles.Add(index);
            meshSegments[msIndex].triangles.Add(index + sides);

            if (i == sides - 1)
            {
                meshSegments[msIndex].triangles.Add(meshSegments[msIndex].vertices.Count - sides * 2);
            }
            else
            {
                meshSegments[msIndex].triangles.Add(index + 1);
            }

            //Top triangle
            meshSegments[msIndex].triangles.Add(index + sides);

            if (i == sides - 1)
            {
                meshSegments[msIndex].triangles.Add(meshSegments[msIndex].vertices.Count - sides);
                meshSegments[msIndex].triangles.Add(meshSegments[msIndex].vertices.Count - sides * 2);
            }
            else
            {
                meshSegments[msIndex].triangles.Add(index + sides + 1);
                meshSegments[msIndex].triangles.Add(index + 1);
            }
        }

        //Add to mesh lists
        int oldMeshVertexCount = meshVertices.Count;

        for (int i = meshSegments[msIndex].vertices.Count - sides * 2; i < meshSegments[msIndex].vertices.Count; i++)
        {
            meshVertices.Add(meshSegments[msIndex].vertices[i]);
        }

        for (int i = meshSegments[msIndex].triangles.Count - sides * 6; i < meshSegments[msIndex].triangles.Count; i++)
        {
            meshSegments[msIndex].triangles[i] += oldMeshVertexCount;
            meshTriangles.Add(meshSegments[msIndex].triangles[i]);
        }

        Debug.Log("lodcount: " + oldMeshVertexCount);
        Debug.Log(meshVertices.Count);
        Debug.Log(meshTriangles.Count);
    }
    

    void CalculateSegmentEndPos(int index)
    {
        //Old center vector position
        Vector3 oldPos = meshSegments[index].centerVector;

        //Calculate new center vector bend
        //meshSegments[index].curBend = bend;
        float px = length * math.sin(meshSegments[index].curBend * math.PI / 180.0f);
        float py = length * math.cos(meshSegments[index].curBend * math.PI / 180.0f);
        float pz = oldPos.z;
        meshSegments[index].centerVector.x = px;
        meshSegments[index].centerVector.y += py;
        meshSegments[index].centerVector.z = pz;

        //calculate new center vector rotation
        //meshSegments[index].curRotation = rotation;
        meshSegments[index].centerVector.x = px * math.cos(meshSegments[index].curRotation * math.PI / 180.0f) + pz * math.sin(meshSegments[index].curRotation * math.PI / 180.0f);
        meshSegments[index].centerVector.z = pz * math.cos(meshSegments[index].curRotation * math.PI / 180.0f) - px * math.sin(meshSegments[index].curRotation * math.PI / 180.0f);

        //Add vertices end pos
        meshSegments[index].lerpEndPos.Clear();

        meshSegments[index].curTapering = tapering;
        meshSegments[index].radius *= meshSegments[index].curTapering;

        float p1x = (meshSegments[index].radius * math.sin(meshSegments[index].sidesAngle * math.PI / 180.0f));
        float ply = -meshSegments[index].radius * math.sin(meshSegments[index].curBend * math.PI / 180.0f) * math.cos(rotation * math.PI / 180.0f);
        float p1z = (meshSegments[index].radius * math.cos(meshSegments[index].sidesAngle * math.PI / 180.0f));

        meshSegments[index].lerpEndPos.Add(new Vector3(meshSegments[index].centerVector.x + p1x, meshSegments[index].centerVector.y + ply, meshSegments[index].centerVector.z + p1z));

        for (int i = 1; i < sides; i++)
        {
            px = p1x * math.cos(meshSegments[index].sidesAngle * i * math.PI / 180.0f) - p1z * math.sin(meshSegments[index].sidesAngle * i * math.PI / 180.0f);
            py = -meshSegments[index].radius * math.sin(meshSegments[index].curBend * math.PI / 180.0f) * math.cos((rotation + meshSegments[index].sidesAngle * i) * math.PI / 180.0f);
            pz = p1x * math.sin(meshSegments[index].sidesAngle * i * math.PI / 180.0f) + p1z * math.cos(meshSegments[index].sidesAngle * i * math.PI / 180.0f);

            meshSegments[index].lerpEndPos.Add(new Vector3(meshSegments[index].centerVector.x + px, meshSegments[index].centerVector.y + py, meshSegments[index].centerVector.z + pz));
        }
    }

    
    IEnumerator GrowForward(int index)
    {
        float timeElapsed = Time.deltaTime;
        float timeEnd = GrowthSpeed + Time.deltaTime;

        while (timeElapsed < timeEnd)
        {
            for (int i = 0; i < sides; i++)
            {
                //int vertIndex = meshSegments[index].vertices.Count - sides + i;
                //meshSegments[index].vertices[vertIndex] = Vector3.Lerp(meshSegments[index].lerpStartPos[i], meshSegments[index].lerpEndPos[i], timeElapsed / timeEnd);
                int vertIndex = meshVertices.Count - sides + i;
                meshVertices[vertIndex] = Vector3.Lerp(meshSegments[index].lerpStartPos[i], meshSegments[index].lerpEndPos[i], timeElapsed / timeEnd);
                UpdateMesh(index);
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        RunNextRule(index);
    }


    void UpdateMesh(int index)
    {
        mesh.Clear();

        mesh.vertices = meshVertices.ToArray();
        mesh.triangles = meshTriangles.ToArray();
        mesh.RecalculateNormals();
        mr.material = material;
    }



    void TestCalculation()
    {
        float px = 1 * math.sin(15 * math.PI / 180.0f);
        float py = 1 * math.cos(15 * math.PI / 180.0f);

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();


        vertices.Clear();
        vertices.Add(new Vector3(0, 0, 0));
        vertices.Add(new Vector3(1, 0, 0));
        vertices.Add(new Vector3(px, py, 0));
        vertices.Add(new Vector3(px + 1, py, 0));


        triangles.Clear();
        triangles.Add(0);
        triangles.Add(2);
        triangles.Add(1);

        triangles.Add(2);
        triangles.Add(3);
        triangles.Add(1);

        //mesh.Clear();
        meshVertices = new List<Vector3>();
        meshTriangles = new List<int>();

        meshVertices.AddRange(vertices);
        meshTriangles.AddRange(triangles);

        mesh.vertices = meshVertices.ToArray();
        mesh.triangles = meshTriangles.ToArray();
        mesh.RecalculateNormals();
        mr.material = material;

        List<Vector3> vertices2 = new List<Vector3>();
        List<int> triangles2 = new List<int>();

        vertices2.Add(new Vector3(0, py, 0));
        vertices2.Add(new Vector3(1, py, 0));
        vertices2.Add(new Vector3(px, py + 1, 0));
        vertices2.Add(new Vector3(px + 1, py + 1, 0));


        triangles2.Clear();
        triangles2.Add(0);
        triangles2.Add(2);
        triangles2.Add(1);

        triangles2.Add(2);
        triangles2.Add(3);
        triangles2.Add(1);


        for ( int i = 0; i < triangles2.Count; i++)
        {
            triangles2[i] += meshVertices.Count;
        }

        meshVertices.AddRange(vertices2);
        meshTriangles.AddRange(triangles2);

        mesh.Clear();

        mesh.vertices = meshVertices.ToArray();
        mesh.triangles = meshTriangles.ToArray();
        mesh.RecalculateNormals();
        mr.material = material;

        Debug.Log(meshVertices.Count);
        Debug.Log(meshTriangles.Count);
        Debug.Log(meshTriangles[0] + " " + meshTriangles[1] + " " + meshTriangles[2]);
        Debug.Log(meshTriangles[3] + " " + meshTriangles[4] + " " + meshTriangles[5]);
        Debug.Log(meshTriangles[6] + " " + meshTriangles[7] + " " + meshTriangles[8]);
        Debug.Log(meshTriangles[9] + " " + meshTriangles[10] + " " + meshTriangles[11]);
    }
}

/*
 * 
/*
Debug.Log("create shape triangles: " + triangles.Count/3);
Debug.Log("create shape Vertices: " + vertices.Count);
*/        /*
Debug.Log("create shape triangles: " + triangles.Count/3);
Debug.Log("create shape Vertices: " + vertices.Count);
        



Debug.Log("add segment vertices" + vertices.Count);
Debug.Log("add segment triangles: " + triangles.Count/3);
Debug.Log(triangles[0] + " " + triangles[1] + " " + triangles[2]);
Debug.Log(triangles[3] + " " + triangles[4] + " " + triangles[5]);
Debug.Log(triangles[6] + " " + triangles[7] + " " + triangles[8]);
Debug.Log(triangles[9] + " " + triangles[10] + " " + triangles[11]);
Debug.Log(triangles[12] + " " + triangles[13] + " " + triangles[14]);
Debug.Log(triangles[15] + " " + triangles[16] + " " + triangles[17]);
Debug.Log(triangles[18] + " " + triangles[19] + " " + triangles[20]);
Debug.Log(triangles[21] + " " + triangles[22] + " " + triangles[23]);
Debug.Log(triangles[24] + " " + triangles[25] + " " + triangles[26]);
Debug.Log(triangles[27] + " " + triangles[28] + " " + triangles[29]);
Debug.Log(triangles[30] + " " + triangles[31] + " " + triangles[32]);
Debug.Log(triangles[33] + " " + triangles[34] + " " + triangles[35]);

if (triangles.Count/3 > 12)
{
    Debug.Log(triangles[36] + " " + triangles[37] + " " + triangles[38]);
    Debug.Log(triangles[39] + " " + triangles[40] + " " + triangles[41]);
    Debug.Log(triangles[42] + " " + triangles[43] + " " + triangles[44]);
    Debug.Log(triangles[45] + " " + triangles[46] + " " + triangles[47]);
    Debug.Log(triangles[48] + " " + triangles[49] + " " + triangles[50]);
    Debug.Log(triangles[51] + " " + triangles[52] + " " + triangles[53]);
}
*/