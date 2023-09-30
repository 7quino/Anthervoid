using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class MeshGenerator12 : MonoBehaviour
{
    [Header("Branch Resources")]
    public AntherData antherData;
    public GameObject branchPrefab;
    public GameObject flowerPrefab;
    FlowerGenerator flowerGenerator;
    public GameObject leafPrefab;
    LeafGenerator leafGenerator;
    public Material material;
    [HideInInspector]
    public Material stemMaterial;
    

    [Header("Branch Properties")]
    public int sides = 3;
    public int segments = 1;
    public float radius = 0.1f;
    public float lenght = 1.0f;
    public float scale = 0.9f;
    public float rotation = 60;
    public float bend = 15;
    public float tapering = 0.8f;
    public float randomness = 0.4f;
    public bool lerpGrowth = true;
    public float growthSpeed = 1;
    public int stemGenerations = 1;
    public int rootGenerations = 1;
    public float endSegmentLenght = 0.5f;

    [Header("Curvature")]
    public float curveLenght = 0.5f;
    public float amplitude = 0.1f;
    [HideInInspector]
    public float curLenght;
    [HideInInspector]
    Vector3 curveVector = new Vector3();

    [Header("Rules stem")]
    public string ruleA;
    public string ruleB;
    public string ruleC;
    public string ruleD;

    [Header("Rules root")]
    public string ruleE;
    public string ruleF;
    public string ruleG;
    public string ruleH;

    //Private system properties
    [HideInInspector]
    public List<char> ruleListA = new List<char>();
    [HideInInspector]
    public List<char> ruleListB = new List<char>();
    [HideInInspector]
    public List<char> ruleListC = new List<char>();
    [HideInInspector]
    public List<char> ruleListD = new List<char>();
    [HideInInspector]
    public List<char> ruleListE = new List<char>();
    [HideInInspector]
    public List<char> ruleListF = new List<char>();
    [HideInInspector]
    public List<char> ruleListG = new List<char>();
    [HideInInspector]
    public List<char> ruleListH = new List<char>();
    [HideInInspector]
    List<Branch5> branches = new List<Branch5>();
    float sideAngle = 0;

    void Start()
    {
        CreateRuleQueues();

        flowerGenerator = Instantiate(flowerPrefab, transform).GetComponent<FlowerGenerator>();
        flowerGenerator.CreateBaseData(antherData);

        leafGenerator = Instantiate(leafPrefab, transform).GetComponent<LeafGenerator>();
        leafGenerator.CreateBaseData();

        SetMaterialProperties();

        CreateFirstBranch(0);
        CreateFirstBranch(1);
    }

    void CreateRuleQueues()
    {
        CreateQueue(ruleA, ruleListA);
        CreateQueue(ruleB, ruleListB);
        CreateQueue(ruleC, ruleListC);
        CreateQueue(ruleD, ruleListD);
        CreateQueue(ruleE, ruleListE);
        CreateQueue(ruleF, ruleListF);
        CreateQueue(ruleG, ruleListG);
        CreateQueue(ruleH, ruleListH);
    }

    void CreateQueue(string rules, List<char> list)
    {
        if (rules.Length == 0) return;
        for (int i = 0; i < rules.Length; i++)
        {
            list.Add((char)rules[i]);
        }
    }

    void SetMaterialProperties()
    {
        float gradiennt = Random.Range(0.0f, 1.0f);

        stemMaterial = Instantiate(material);
        stemMaterial.SetFloat("_ColorPosition", gradiennt);

        leafGenerator.SetMaterialProperties(gradiennt);
    }

    void CreateFirstBranch(int id)
    {
        GameObject newBranch = Instantiate(branchPrefab, gameObject.transform);
        branches.Add(newBranch.GetComponent<Branch5>());
        branches[id].id = id;
        branches[id].meshGenerator = gameObject.GetComponent<MeshGenerator12>();

        if (id == 0)
        {
            branches[id].rules.AddRange(ruleListA);
            branches[id].generations = stemGenerations;
            branches[id].stem = true;
        }
        if (id == 1)
        {
            branches[id].rules.AddRange(ruleListE);
            branches[id].generations = rootGenerations;
            branches[id].stem = false;
        } 
    }

    public void CreateBaseShape(int id)
    {
        if (id == 0)
        {
            branches[id].lenghtV = new Vector3(0, lenght / segments, 0);
            branches[id].radiusV *= radius;
            sideAngle = 360 / sides;
            branches[id].centerVector = branches[id].lenghtV;

            for (int i = 0; i < sides; i++)
            {
                branches[id].vertices.Add(Quaternion.AngleAxis(sideAngle * i, Vector3.up) * new Vector3(branches[id].radiusV.x, 0, branches[id].radiusV.z));
                branches[id].lerpEndPos.Add(branches[id].vertices[i]);
            }
        }

        if (id == 1)
        {
            branches[id].lenghtV = new Vector3(0, -lenght / segments, 0);
            branches[id].radiusV *= radius;
            sideAngle = 360 / sides;
            branches[id].centerVector = branches[id].lenghtV;
            branches[id].radiusV = Quaternion.AngleAxis(180, branches[id].rotateAxis) * branches[id].radiusV;

            for (int i = 0; i < sides; i++)
            {
                branches[id].vertices.Add(Quaternion.AngleAxis(sideAngle * i, -Vector3.up) * new Vector3(branches[id].radiusV.x, 0, branches[id].radiusV.z));
                branches[id].lerpEndPos.Add(branches[id].vertices[i]);
            }
        }
    }

    public void Scale(int id)
    {
        branches[id].lenghtV *= scale;
        branches[id].RunNextRule();
    }


    public void Bend(int id, int direction)
    {
        branches[id].randomFactor = 1 + Random.Range(-randomness / 2, randomness / 2);
        branches[id].curBend += (bend * direction) * branches[id].randomFactor;
        branches[id].lenghtV = Quaternion.AngleAxis(branches[id].curBend, branches[id].bendAxis) * branches[id].lenghtV;
        branches[id].centerVector = Quaternion.AngleAxis(bend * direction, branches[id].bendAxis) * branches[id].lenghtV;
        branches[id].radiusV = Quaternion.AngleAxis(branches[id].curBend, branches[id].bendAxis) * branches[id].radiusV;
        branches[id].RunNextRule();
    }


    public void Rotation(int id, int direction)
    {
        branches[id].randomFactor = 1 + Random.Range(-randomness, randomness);
        branches[id].curRotation = (branches[id].curRotation + (rotation * direction * branches[id].randomFactor)) % 360;
        branches[id].bendAxis = Quaternion.AngleAxis(rotation * direction * branches[id].randomFactor, branches[id].centerVector) * branches[id].bendAxis;
        branches[id].lenghtV = Quaternion.AngleAxis(rotation * direction * branches[id].randomFactor, branches[id].centerVector) * branches[id].lenghtV;

        branches[id].RunNextRule();
    }


    public void Forward(int id)
    {
        if (branches[id].meshGenerator.lerpGrowth) StartCoroutine(LerpGrowth(id));
        else StartCoroutine(InstantGrowth(id));
    }

    public void ForwardPos(int id)
    {
        //Randomize values
        branches[id].randomFactor = 1 + Random.Range(-randomness, randomness);
        branches[id].spaceVector += branches[id].centerVector * branches[id].randomFactor;

        //Adding curvature
        curLenght += lenght / segments;
        float curveheight = amplitude * math.sin(branches[id].spaceVector.magnitude / curveLenght);
        curveVector = new Vector3(curveheight, 0, curveheight);

        //tapering
        branches[id].radiusV *= 1 + ((tapering - 1) / segments);

        //Add vertices
        branches[id].lerpStartPos.Clear();
        branches[id].lerpEndPos.Clear();


        for (int i = 0; i < sides; i++)
        {
            int index = branches[id].vertices.Count - sides;
            branches[id].vertices.Add(branches[id].vertices[index]);
            branches[id].lerpStartPos.Add(branches[id].vertices[index]);
        }

        for (int i = 0; i < sides; i++)
        {
            branches[id].lerpEndPos.Add((Quaternion.AngleAxis(sideAngle * i, branches[id].centerVector) * branches[id].radiusV) + branches[id].spaceVector + curveVector);
        }


        //Add triangles
        for (int i = 0; i < sides; i++)
        {
            int index = branches[id].vertices.Count - sides * 2 + i;

            //Bottom triangle
            branches[id].triangles.Add(index);

            if (i == sides - 1) branches[id].triangles.Add(branches[id].vertices.Count - sides * 2);
            else branches[id].triangles.Add(index + 1);

            branches[id].triangles.Add(index + sides);

            //Top triangle
            branches[id].triangles.Add(index + sides);

            if (i == sides - 1)
            {
                branches[id].triangles.Add(branches[id].vertices.Count - sides * 2);
                branches[id].triangles.Add(branches[id].vertices.Count - sides);
            }
            else
            {
                branches[id].triangles.Add(index + 1);
                branches[id].triangles.Add(index + sides + 1);
            }
        }
    }

    public IEnumerator InstantGrowth(int id)
    {
        for (int j = 0; j < branches[id].meshGenerator.segments; j++)
        {
            ForwardPos(id);
            for (int i = 0; i < sides; i++)
            {
                int vertIndex = branches[id].vertices.Count - sides + i;
                branches[id].vertices[vertIndex] = branches[id].lerpEndPos[i];
                branches[id].UpdateMesh(stemMaterial);
            }
            yield return null;
        }
        
        branches[id].RunNextRule();
    }


    public IEnumerator LerpGrowth(int id)
    {
        for (int j = 0; j < branches[id].meshGenerator.segments; j++)
        {
            ForwardPos(id);

            //Average endpoint
            Vector3 avgEndpoint = new Vector3();
            foreach (Vector3 point in branches[id].lerpEndPos) avgEndpoint += point;
            avgEndpoint /= branches[id].lerpEndPos.Count;

            float timeElapsed = Time.deltaTime;
            float timeEnd = growthSpeed + Time.deltaTime;

            while (timeElapsed < timeEnd)
            {
                for (int i = 0; i < sides; i++)
                {
                    int vertIndex = branches[id].vertices.Count - sides + i;
                    //branches[id].vertices[vertIndex] = Vector3.Lerp(branches[id].lerpStartPos[i], branches[id].spaceVector + curveVector, timeElapsed / timeEnd);
                    branches[id].vertices[vertIndex] = Vector3.Lerp(branches[id].lerpStartPos[i], avgEndpoint, timeElapsed / timeEnd);
                    branches[id].UpdateMesh(stemMaterial);
                }

                timeElapsed += Time.deltaTime;
                yield return null;
            }
            StartCoroutine(LerpGrowthRadius(id, branches[id].vertices.Count - sides));
        }
        branches[id].RunNextRule();
    }

    public IEnumerator LerpGrowthRadius(int id, int vertIndex)
    {
        float timeElapsed = Time.deltaTime;
        float timeEnd = growthSpeed + Time.deltaTime;
        List<Vector3> startPos = new List<Vector3>();
        List<Vector3> endPos = new List<Vector3>();

        for (int i = 0; i < sides; i++)
        {
            startPos.Add(branches[id].vertices[vertIndex + i]);
            endPos.Add(branches[id].lerpEndPos[i]);
        }

        while (timeElapsed < timeEnd)
        {
            for (int i = 0; i < sides; i++)
            {
                branches[id].vertices[vertIndex + i] = Vector3.Lerp(startPos[i], endPos[i], timeElapsed / timeEnd);
                branches[id].UpdateMesh(stemMaterial);
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
    


    public void AddBranch(int id, LSystemBranch branch)
    {
        if (branch == LSystemBranch.A || branch == LSystemBranch.E)
        {
            if(branches[id].ruleIndex == branches[id].rules.Count) branches[id].curGen++;

            branches[id].rules.Clear();
            if (branch == LSystemBranch.A) branches[id].rules.AddRange(ruleListA);
            if (branch == LSystemBranch.E) branches[id].rules.AddRange(ruleListE);
            branches[id].ruleIndex = 0;
            branches[id].RunNextRule();
            return;
        }

        GameObject newBranch = Instantiate(branchPrefab, gameObject.transform);
        branches.Add(newBranch.GetComponent<Branch5>());

        int newId = branches.Count - 1;
        branches[newId].id = newId;
        branches[newId].meshGenerator = gameObject.GetComponent<MeshGenerator12>();
        branches[newId].branch = branch;
        switch (branch)
        {
            case LSystemBranch.B: branches[newId].rules.AddRange(ruleListB); break;
            case LSystemBranch.C: branches[newId].rules.AddRange(ruleListC); break;
            case LSystemBranch.D: branches[newId].rules.AddRange(ruleListD); break;
            case LSystemBranch.F: branches[newId].rules.AddRange(ruleListF); break;
            case LSystemBranch.G: branches[newId].rules.AddRange(ruleListG); break;
            case LSystemBranch.H: branches[newId].rules.AddRange(ruleListH); break;
        }
        branches[newId].meshGenerator.CreateBranchShape(newId, id);
        branches[newId].assignMesh();

        branches[id].RunNextRule();
        branches[newId].RunNextRule();
    }

    public void CreateBranchShape(int newId, int oldId)
    {
        branches[newId].transform.rotation = branches[oldId].transform.rotation;
        branches[newId].centerVector = branches[oldId].centerVector;
        branches[newId].spaceVector = branches[oldId].spaceVector;
        branches[newId].rotateAxis = branches[oldId].rotateAxis;
        branches[newId].bendAxis = branches[oldId].bendAxis;
        branches[newId].radiusV = branches[oldId].radiusV;
        branches[newId].lenghtV = branches[oldId].lenghtV;
        branches[newId].curRotation = branches[oldId].curRotation;
        branches[newId].curBend = branches[oldId].curBend;
        branches[newId].generations = branches[oldId].generations;
        branches[newId].curGen = branches[oldId].curGen;
        branches[newId].stem = branches[oldId].stem;

        for (int i = 0; i < sides; i++)
        {
            int index = branches[oldId].vertices.Count - sides + i;
            //branches[newId].vertices.Add(branches[oldId].lerpEndPos[i]);
            branches[newId].vertices.Add(branches[oldId].vertices[index]);
            branches[newId].lerpStartPos.Add(branches[newId].vertices[i]);
        }

        StartCoroutine(BranchShapeRadius(newId, oldId));
    }

    IEnumerator BranchShapeRadius(int newId, int oldId)
    {
        float timeElapsed = Time.deltaTime;
        float timeEnd = growthSpeed + Time.deltaTime;
        List<Vector3> startPos = new List<Vector3>();
        List<Vector3> endPos = new List<Vector3>();

        for (int i = 0; i < sides; i++)
        {
            startPos.Add(branches[newId].vertices[i]);
            endPos.Add(branches[oldId].lerpEndPos[i]);
        }

        while (timeElapsed < timeEnd)
        {
            for (int i = 0; i < sides; i++)
            {
                branches[newId].vertices[i] = Vector3.Lerp(startPos[i], endPos[i], timeElapsed / timeEnd);
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void EndSegment(int id, bool createFlower)
    {
        ForwardPos(id);

        branches[id].lerpEndPos.Clear();
        for (int i = 0; i < sides; i++)
        {
            branches[id].lerpEndPos.Add((endSegmentLenght * branches[id].centerVector) + (branches[id].spaceVector - branches[id].centerVector) + curveVector);
        }

        StartCoroutine(EndGrowth(id, createFlower));
    }

    public IEnumerator EndGrowth(int id, bool createFlower)
    {
        float timeElapsed = Time.deltaTime;
        float timeEnd = growthSpeed + Time.deltaTime;

        while (timeElapsed < timeEnd)
        {
            for (int i = 0; i < sides; i++)
            {
                int vertIndex = branches[id].vertices.Count - sides + i;
                branches[id].vertices[vertIndex] = Vector3.Lerp(branches[id].lerpStartPos[i], branches[id].lerpEndPos[i], timeElapsed / timeEnd);
                branches[id].UpdateMesh(stemMaterial);
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        if (branches[id].stem == true && createFlower)
        {
            CreateFlower(id);
        }
        else if (branches[id].stem == true && !createFlower)
        {
            CreateLeaf(id);
        }
    }

    public void CreateFlower(int id)
    {
        GameObject flower = flowerGenerator.CreateFlower(branches[id].transform, antherData);
        float size = flowerGenerator.flowerData.size;
        float growthSpeed = branches[id].meshGenerator.flowerGenerator.flowerData.growthSpeed;

        Vector3 averagePoint = Vector3.zero;
        for (int i = 0; i < sides; i++)
        {
            averagePoint += branches[id].vertices[branches[id].vertices.Count - sides * 2 + i];
        }
        averagePoint /= sides;

        Vector3 flowerAngle = (branches[id].vertices[branches[id].vertices.Count - 1] - averagePoint);
        flower.transform.localRotation = Quaternion.LookRotation(flowerAngle, Vector3.up);
        flower.transform.Rotate(90, 0, 0);
        flower.transform.localPosition = branches[id].vertices[branches[id].vertices.Count - 1];

        StartCoroutine(GrowFlowerLeaf(id, flower.transform, size, growthSpeed));
    }


    public void CreateLeaf(int id)
    {
        GameObject leaf = new GameObject("Leaf");
        leaf.transform.parent = branches[id].transform;
        GameObject leafOffset = leafGenerator.CreateLeaf(leaf.transform);
        float size = leafGenerator.leafData.size;
        float growthSpeed = leafGenerator.leafData.growthSpeed;

        Vector3 averagePoint = Vector3.zero;
        for (int i = 0; i < sides; i++)
        {
            averagePoint += branches[id].vertices[branches[id].vertices.Count - sides * 2 + i];
        }
        averagePoint /= sides;

        Vector3 leafAngle = (branches[id].vertices[branches[id].vertices.Count - 1] - averagePoint);
        Quaternion rot = Quaternion.LookRotation(leafAngle, Vector3.up);
        leaf.transform.localRotation = rot;
        leafOffset.transform.localPosition = new Vector3(0, -branches[id].lenghtV.magnitude/3*2, 0);
        leaf.transform.Rotate(90, 0, 0);
        leaf.transform.localPosition = branches[id].vertices[branches[id].vertices.Count -1];

        StartCoroutine(GrowFlowerLeaf(id, leaf.transform, size, growthSpeed));
    }


    public IEnumerator GrowFlowerLeaf(int id, Transform modelTransform, float size, float growthSpeed)
    {
        float timeElapsed = Time.deltaTime;
        float timeEnd = growthSpeed + Time.deltaTime;
        float randomScale = Random.Range(0.6f, 1.3f);

        while (timeElapsed < timeEnd)
        {
            modelTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * size * randomScale, timeElapsed / timeEnd);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}