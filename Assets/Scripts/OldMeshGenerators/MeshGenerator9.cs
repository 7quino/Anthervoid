using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class MeshGenerator9 : MonoBehaviour
{
    //mesh
    public GameObject branchPrefab;
    public Material material;

    public int sides = 3;
    public float segments = 1;
    public float radius = 0.1f;
    public float lenght = 1.0f;
    public float constantTwist = 0;
    public float rotation = 0;
    public float bend = 0;
    public float tapering = 1;
    public float randomness = 0;
    public float GrowthSpeed = 10;
    public int generations = 1;
    public float endSegmentLenght = 0.5f;

    public string ruleA;
    public string ruleB;
    public string ruleC;
    public string ruleD;

    //Private system properties
    [HideInInspector]
    public List<char> ruleListA = new List<char>();
    [HideInInspector]
    public List<char> ruleListB = new List<char>();
    [HideInInspector]
    public List<char> ruleListC = new List<char>();
    [HideInInspector]
    public List<char> ruleListD = new List<char>();
    int curGeneration;
    List<Branch2> branches = new List<Branch2>();
    float sideAngle = 0;

    void Start()
    {
        CreateRuleQueues();
        CreateFirstBranch();
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

    void CreateFirstBranch()
    {
        GameObject newBranch = Instantiate(branchPrefab, gameObject.transform);
        branches.Add(newBranch.GetComponent<Branch2>());
        branches[0].rules.AddRange(ruleListA);
        branches[0].id = 0;
        branches[0].firstInstance = true;
        branches[0].meshGenerator = gameObject.GetComponent<MeshGenerator9>();
    }

    public void CreateBaseShape(int id)
    {
        branches[id].lenghtV = new Vector3(0, lenght, 0);
        branches[id].radiusV *= radius;
        sideAngle = 360 / sides;
        branches[id].centerVector = branches[id].lenghtV;

        for (int i = 0; i < sides; i++)
        {
            branches[id].vertices.Add(Quaternion.AngleAxis(sideAngle * i, Vector3.up) * branches[id].radiusV);
        }
    }

    public void CreateBranchShape(int newId, int oldId)
    {
        branches[newId].centerVector = branches[oldId].centerVector;
        branches[newId].spaceVector = branches[oldId].spaceVector;
        branches[newId].rotateAxis = branches[oldId].rotateAxis;
        branches[newId].bendAxis = branches[oldId].bendAxis;
        branches[newId].radiusV = branches[oldId].radiusV;
        branches[newId].lenghtV = branches[oldId].lenghtV;
        branches[newId].curRotation = branches[oldId].curRotation;
        branches[newId].curBend = branches[oldId].curBend;
        branches[newId].curGen = branches[oldId].curGen;

        for (int i = 0; i < sides; i++)
        {
            int index = branches[oldId].vertices.Count - sides + i;
            branches[newId].vertices.Add(branches[oldId].vertices[index]);
            branches[newId].lerpStartPos.Add(branches[newId].vertices[i]);
        }
    }

    public void BendUp(int id)
    {
        branches[id].randomFactor = 1 + Random.Range(-branches[id].meshGenerator.randomness / 2, branches[id].meshGenerator.randomness / 2);
        branches[id].curBend += bend * branches[id].randomFactor;
        branches[id].lenghtV = Quaternion.AngleAxis(branches[id].curBend, branches[id].bendAxis) * branches[id].lenghtV;
        branches[id].centerVector = Quaternion.AngleAxis(bend, branches[id].bendAxis) * branches[id].lenghtV;
        branches[id].radiusV = Quaternion.AngleAxis(branches[id].curBend, branches[id].bendAxis) * branches[id].radiusV;
        branches[id].RunNextRule();
    }

    public void BendDown(int id)
    {
        branches[id].randomFactor = 1 + Random.Range(-branches[id].meshGenerator.randomness / 2, branches[id].meshGenerator.randomness / 2);
        branches[id].curBend -= bend * branches[id].randomFactor;
        branches[id].lenghtV = Quaternion.AngleAxis(branches[id].curBend, branches[id].bendAxis) * branches[id].lenghtV;
        branches[id].centerVector = Quaternion.AngleAxis(-bend * branches[id].randomFactor, branches[id].bendAxis) * branches[id].lenghtV;
        branches[id].radiusV = Quaternion.AngleAxis(branches[id].curBend, branches[id].bendAxis) * branches[id].radiusV;
        branches[id].RunNextRule();
    }

    public void Rotation(int id)
    {
        branches[id].randomFactor = 1 + Random.Range(-branches[id].meshGenerator.randomness / 2, branches[id].meshGenerator.randomness / 2);
        branches[id].curRotation = (branches[id].curRotation + (rotation * branches[id].randomFactor)) % 360;
        branches[id].bendAxis = Quaternion.AngleAxis(rotation * branches[id].randomFactor, branches[id].centerVector) * branches[id].bendAxis;
        branches[id].lenghtV = Quaternion.AngleAxis(rotation * branches[id].randomFactor, branches[id].centerVector) * branches[id].lenghtV;
        branches[id].RunNextRule();
    }

    public void Forward(int id)
    {
        StartCoroutine(ForwardGrowth(id));
    }

    public void ForwardPos(int id)
    {
        //Randomize values
        branches[id].randomFactor = 1 + Random.Range(-branches[id].meshGenerator.randomness / 2, branches[id].meshGenerator.randomness / 2);
        branches[id].spaceVector += branches[id].centerVector * branches[id].randomFactor;
        //branches[id].spaceVector += branches[id].centerVector;

        //tapering and twist
        branches[id].radiusV *= tapering;
        branches[id].radiusV = Quaternion.AngleAxis(branches[id].meshGenerator.constantTwist, branches[id].centerVector) * branches[id].radiusV;



        branches[id].lerpStartPos.Clear();
        branches[id].lerpEndPos.Clear();
        //Add vertices
        for (int i = 0; i < sides; i++)
        {
            int index = branches[id].vertices.Count - sides;
            branches[id].vertices.Add(branches[id].vertices[index]);
            branches[id].lerpStartPos.Add(branches[id].vertices[index]);
        }

        for (int i = 0; i < sides;  i++)
        {
            branches[id].lerpEndPos.Add((Quaternion.AngleAxis(sideAngle * i, branches[id].centerVector) * branches[id].radiusV) + branches[id].spaceVector);
        }


        //Add triangles
        for (int i = 0; i < sides; i++)
        {
            int index = branches[id].vertices.Count - sides * 2 + i;

            //Bottom triangle
            branches[id].triangles.Add(index);
            if (i == sides - 1)
                branches[id].triangles.Add(branches[id].vertices.Count - sides * 2);
            else
                branches[id].triangles.Add(index + 1);
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


    public IEnumerator ForwardGrowth(int id)
    {
        for (int j = 0; j < branches[id].meshGenerator.segments; j++)
        {
            ForwardPos(id);

            float timeElapsed = Time.deltaTime;
            float timeEnd = GrowthSpeed + Time.deltaTime;

            while (timeElapsed < timeEnd)
            {
                for (int i = 0; i < sides; i++)
                {
                    int vertIndex = branches[id].vertices.Count - sides + i;
                    branches[id].vertices[vertIndex] = Vector3.Lerp(branches[id].lerpStartPos[i], branches[id].lerpEndPos[i], timeElapsed / timeEnd);
                    branches[id].UpdateMesh(material);
                }

                timeElapsed += Time.deltaTime;
                yield return null;
            }
        }

        branches[id].RunNextRule();
    }


    public void RuleABCD(int id, LSystemBranch branch)
    {
        if (branch == LSystemBranch.A && branches[id].ruleIndex == branches[id].rules.Count)
        {
            branches[id].curGen++;
        }

        GameObject newBranch = Instantiate(branchPrefab, gameObject.transform);
        branches.Add(newBranch.GetComponent<Branch2>());

        int newId = branches.Count - 1;
        branches[newId].id = newId;
        branches[newId].meshGenerator = gameObject.GetComponent<MeshGenerator9>();
        branches[newId].branch = branch;
        switch (branch)
        {
            case LSystemBranch.A: branches[newId].rules.AddRange(ruleListA); break;
            case LSystemBranch.B: branches[newId].rules.AddRange(ruleListB); break;
            case LSystemBranch.C: branches[newId].rules.AddRange(ruleListC); break;
            case LSystemBranch.D: branches[newId].rules.AddRange(ruleListD); break;
        }
        branches[newId].meshGenerator.CreateBranchShape(newId, id);
        branches[newId].assignMesh();

        branches[id].RunNextRule();
        branches[newId].RunNextRule();        
    }

    public void EndSegment(int id)
    {
        ForwardPos(id);

        branches[id].lerpEndPos.Clear();
        for (int i = 0; i < sides; i++)
        {
            branches[id].lerpEndPos.Add(endSegmentLenght * branches[id].spaceVector);
        }

        StartCoroutine(EndGrowth(id));
    }

    public IEnumerator EndGrowth(int id)
    {
        float timeElapsed = Time.deltaTime;
        float timeEnd = GrowthSpeed + Time.deltaTime;

        while (timeElapsed < timeEnd)
        {
            for (int i = 0; i < sides; i++)
            {
                int vertIndex = branches[id].vertices.Count - sides + i;
                branches[id].vertices[vertIndex] = Vector3.Lerp(branches[id].lerpStartPos[i], branches[id].lerpEndPos[i], timeElapsed / timeEnd);
                branches[id].UpdateMesh(material);
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}
