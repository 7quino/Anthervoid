using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;


public class MeshGenerator3 : MonoBehaviour
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
    int indexA = 0;
    Vector3 centerVector = Vector3.zero;
    List<Vector3> lerpStartPos = new List<Vector3>();
    List<Vector3> lerpEndPos = new List<Vector3>();
    float sidesAngle;
    float radius;
    float curBend = 0;
    float curRotation = 0;
    float curTapering = 1;

    //mesh
    Mesh mesh;
    MeshRenderer mr;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();

    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateBaseShape();
        CreateRuleQueues();
        //RunNextRule();
        AddSegment();
        CalculateSegmentEndPos();
        StartCoroutine(GrowForward());
    }

    void CreateBaseShape()
    {
        sidesAngle = 360 / sides;
        radius = diameter / 2;

        //Add verticies
        float p1x = radius * math.sin(sidesAngle * math.PI / 180.0f);
        float p1y = radius * math.cos(sidesAngle * math.PI / 180.0f);
        vertices.Add(new Vector3(p1x, 0, p1y));

        for (int i = 1; i < sides; i++)
        {
            float px = p1x * math.cos(sidesAngle * i * math.PI / 180.0f) - p1y * math.sin(sidesAngle * i * math.PI / 180.0f);
            float py = p1x * math.sin(sidesAngle * i * math.PI / 180.0f) + p1y * math.cos(sidesAngle * i * math.PI / 180.0f);
            vertices.Add(new Vector3(px, 0, py));
        }
    }

    void CreateRuleQueues()
    {
        CreateQueue(ruleA, ruleListA);
        CreateQueue(ruleB, ruleListB);
        CreateQueue(ruleC, ruleListC);
        CreateQueue(ruleD, ruleListD);
    }

    void CreateQueue(string rules, List<char> queue)
    {
        if (rules.Length == 0) return;
        for (int i = 0; i < rules.Length; i++)
        {
            queue.Add(rules[i]);
        }
    }

    void RunNextRule()
    {
        char curRule;

        curRule = ruleListA[indexA];

        switch (curRule)
        {
            case 'u':
            {
                break;
            }
            case 'd':
            {
                break;
            }
            case 'r':
            {
                break;
            }
            case 'g':
            {
                break;
            }
            case 'A':
            {
                break;
            }
            case 'B':
            {
                break;
            }
            case 'C':
            {
                break;
            }
            case 'D':
            {
                break;
            }
        }
    }



    void AddSegment()
    {
        lerpStartPos.Clear();

        for (int i = 0; i < sides; i++)
        {
            int index = vertices.Count - sides;
            vertices.Add(new Vector3(vertices[index].x, vertices[index].y, vertices[index].z));
            lerpStartPos.Add(vertices[i]);
        }

        for (int i = 0; i < sides; i++)
        {
            int index = vertices.Count - sides * 2 + i;

            //Bottom triangle
            triangles.Add(index);
            triangles.Add(index + sides);

            if (i == sides - 1)
            {
                triangles.Add(vertices.Count - sides * 2);
            }
            else
            {
                triangles.Add(index + 1);
            }

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
    }
    

    void CalculateSegmentEndPos()
    {
        //Old center vector position
        Vector3 oldPos = centerVector;

        //Calculate new center vector bend
        curBend = bend;
        float px = length * math.sin(curBend * math.PI / 180.0f);
        float py = length * math.cos(curBend * math.PI / 180.0f);
        float pz = oldPos.z;
        centerVector.x = px;
        centerVector.y += py;
        centerVector.z = pz;
        

        //calculate new center vector rotation
        curRotation = rotation;
        centerVector.x = px * math.cos(curRotation * math.PI / 180.0f) + pz * math.sin(curRotation * math.PI / 180.0f);
        centerVector.z = pz * math.cos(curRotation * math.PI / 180.0f) - px * math.sin(curRotation * math.PI / 180.0f);
        

        //Add vertices end pos
        lerpEndPos.Clear();

        curTapering = tapering;
        radius *= curTapering;

        float p1x = (radius * math.sin(sidesAngle * math.PI / 180.0f));
        float ply = -radius * math.sin(curBend * math.PI / 180.0f) * math.cos(rotation * math.PI / 180.0f);
        float p1z = (radius * math.cos(sidesAngle * math.PI / 180.0f));

        lerpEndPos.Add(new Vector3(centerVector.x + p1x, centerVector.y + ply, centerVector.z + p1z));

        for (int i = 1; i < sides; i++)
        {
            px = p1x * math.cos(sidesAngle * i * math.PI / 180.0f) - p1z * math.sin(sidesAngle * i * math.PI / 180.0f);
            py = -radius * math.sin(curBend * math.PI / 180.0f) * math.cos((rotation + sidesAngle * i) * math.PI / 180.0f);
            pz = p1x * math.sin(sidesAngle * i * math.PI / 180.0f) + p1z * math.cos(sidesAngle * i * math.PI / 180.0f);

            lerpEndPos.Add(new Vector3(centerVector.x + px, centerVector.y + py, centerVector.z + pz));
        }
    }

    

    IEnumerator GrowForward()
    {
        float timeElapsed = 0;
        while (timeElapsed < GrowthSpeed)
        {
            for (int i = 0; i < sides; i++)
            {
                int index = vertices.Count - sides + i;
                vertices[index] = Vector3.Lerp(lerpStartPos[i], lerpEndPos[i], timeElapsed / GrowthSpeed);
                UpdateMesh();
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mr.material = material;
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
        

void TestCalculation()
{
    float px = 1 * math.sin(15 * math.PI / 180.0f);
    float py = 1 * math.cos(15 * math.PI / 180.0f);

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
}

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