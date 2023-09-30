using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;



[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator2 : MonoBehaviour
{

    public int sides;
    public float segHeight;
    public float diameter;
    public float diameterScale;
    public float randomness;

    //float curSegHeight;
    public int generations;
    public Material material;

    public string RuleA;
    public string RuleB;
    public string RuleC;

    //Lerp
    float timeElapsed;
    float lerpDuration = 10;
    float lerpStartValue;
    float lerpEndValue;

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

        lerpStartValue = segHeight;
        lerpEndValue = segHeight * 2;

        CreateBaseShape();
        AddSegment();
        UpdateMesh();
    }

    void CreateBaseShape()
    {
        float angle = 360 / sides;
        float l = diameter / 2;
        float p1x = l * math.sin(angle * math.PI / 180.0f);
        float p1y = l * math.cos(angle * math.PI / 180.0f);

        //Add vertices
        for (int j = 0; j < 2; j++)
        {
            if (j == 0)
                vertices.Add(new Vector3(p1x, 0, p1y));
            else
                vertices.Add(new Vector3(p1x, segHeight, p1y));

            for (int i = 1; i < sides; i++)
            {
                float px = p1x * math.cos(angle * i * math.PI / 180.0f) - p1y * math.sin(angle * i * math.PI / 180.0f);
                float py = p1x * math.sin(angle * i * math.PI / 180.0f) + p1y * math.cos(angle * i * math.PI / 180.0f);

                if (j == 0)
                    vertices.Add(new Vector3(px, 0, py));
                else
                    vertices.Add(new Vector3(px, segHeight, py));
            }
        }

        //Add triangles
        for (int i = 0; i < sides; i++)
        {
            //Bottom triangle
            triangles.Add(i);
            triangles.Add(i + sides);

            if (i == sides - 1)
            {
                triangles.Add(0);
            }
            else
            {
                triangles.Add(i + 1);
            }


            //Top triangle
            triangles.Add(i + sides);

            if (i == sides - 1)
            {
                triangles.Add(sides);
                triangles.Add(0);
            }
            else
            {
                triangles.Add(i + sides + 1);
                triangles.Add(i + 1);
            }
        }

        /*
        Debug.Log("create shape triangles: " + triangles.Count/3);
        Debug.Log("create shape Vertices: " + vertices.Count);
        */
    }

    void AddSegment()
    {
        
        for (int i = 0; i < sides; i++)
        {
            int index = vertices.Count - sides;
            vertices.Add(new Vector3(vertices[index].x, vertices[index].y, vertices[index].z));
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

        /*
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
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mr.material = material;
    }

    private void FixedUpdate()
    {
        /*
        Debug.Log("v6: " + vertices[6]);
        Debug.Log("v7: " + vertices[7]);
        Debug.Log("v8: " + vertices[8]);
        if (vertices.Count > 9)
        {
            Debug.Log("v9: " + vertices[9]);
            Debug.Log("v10: " + vertices[10]);
            Debug.Log("v11: " + vertices[11]);
        }
        */
        

        if (timeElapsed < lerpDuration)
        {
            for (int i = 0; i < sides; i++)
            {
                int index = vertices.Count - sides + i;
                Vector3 point = vertices[index];
                point.y = Mathf.Lerp(lerpStartValue, lerpEndValue, timeElapsed / lerpDuration);
                vertices[index] = point;
                UpdateMesh();
            }

            timeElapsed += Time.deltaTime;
        }
        else
        {
            AddSegment();
            lerpStartValue = lerpEndValue;
            lerpEndValue += segHeight;
            lerpDuration += Time.deltaTime;
            timeElapsed = Time.deltaTime;
        }
    }
}

/*
void CreateBaseShapeGenerations()
{
    float angle = 360 / sides;
    float l = diameter / 2;
    float p1x = l * math.sin(angle * math.PI / 180.0f);
    float p1y = l * math.cos(angle * math.PI / 180.0f);

    //Add vertices
    for (int j = 0; j < generations; j++)
    {
        if (j == 0)
            curSegHeight = 0;
        else
            curSegHeight += segHeight;

        vertices.Add(new Vector3(p1x, curSegHeight, p1y));

        for (int i = 1; i < sides; i++)
        {
            float px = p1x * math.cos(angle * i * math.PI / 180.0f) - p1y * math.sin(angle * i * math.PI / 180.0f);
            float py = p1x * math.sin(angle * i * math.PI / 180.0f) + p1y * math.cos(angle * i * math.PI / 180.0f);

            vertices.Add(new Vector3(px, curSegHeight, py));
        }
    }

    //Add triangles
    for (int j = 0; j < generations - 1; j++)
    {
        int generationOffset = j * sides;

        for (int i = 0; i < sides; i++)
        {
            //Bottom triangle
            triangles.Add(i + generationOffset);
            triangles.Add(i + sides + generationOffset);

            if (i == sides - 1)
            {
                triangles.Add(generationOffset);
            }
            else
            {
                triangles.Add(i + 1 + generationOffset);
            }


            //Top triangle
            triangles.Add(i + sides + generationOffset);

            if (i == sides - 1)
            {
                triangles.Add(sides + generationOffset);
                triangles.Add(generationOffset);
            }
            else
            {
                triangles.Add(i + sides + 1 + generationOffset);
                triangles.Add(i + 1 + generationOffset);
            }
        }
    }
}
*/

