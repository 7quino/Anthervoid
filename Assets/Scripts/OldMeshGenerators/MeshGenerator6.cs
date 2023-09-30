using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


public class MeshGenerator6 : MonoBehaviour
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
    int curGeneration;

    public GameObject branchPrefab;
    List<Branch0> branches = new List<Branch0>();
    List<int> branchIndex = new List<int>();

    void Start()
    {
        //TestCalculation();

        CreateBaseShape();
        CreateRuleQueues();
        RunNextRule(0);
    }

    void CreateBaseShape()
    {
        GameObject newBranch = Instantiate(branchPrefab, gameObject.transform);
        branches.Add(newBranch.GetComponent<Branch0>());

        branches[0].sidesAngle = 360 / sides;
        branches[0].radius = diameter / 2;

        //Add verticies
        float p1x = branches[0].radius * math.sin(branches[0].sidesAngle * math.PI / 180.0f);
        float p1y = branches[0].radius * math.cos(branches[0].sidesAngle * math.PI / 180.0f);
        branches[0].vertices.Add(new Vector3(p1x, 0, p1y));

        for (int i = 1; i < sides; i++)
        {
            float px = p1x * math.cos(branches[0].sidesAngle * i * math.PI / 180.0f) - p1y * math.sin(branches[0].sidesAngle * i * math.PI / 180.0f);
            float py = p1x * math.sin(branches[0].sidesAngle * i * math.PI / 180.0f) + p1y * math.cos(branches[0].sidesAngle * i * math.PI / 180.0f);
            branches[0].vertices.Add(new Vector3(px, 0, py));
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
        switch (branches[index].branch)
        {
            case LSystemBranch.A:
                {
                    UpdateRuleIndex(index, ref branches[index].indexA, ruleListA);
                    if (branches[index].indexA >= ruleListA.Count + 1) return;
                    break;
                }
            case LSystemBranch.B:
                {
                    UpdateRuleIndex(index, ref branches[index].indexB, ruleListB);
                    if (branches[index].indexB >= ruleListB.Count + 1) return;
                    break;
                }
            case LSystemBranch.C:
                {
                    UpdateRuleIndex(index, ref branches[index].indexC, ruleListC);
                    if (branches[index].indexC >= ruleListC.Count + 1) return;
                    break;
                }
            case LSystemBranch.D:
                {
                    UpdateRuleIndex(index, ref branches[index].indexD, ruleListD);
                    if (branches[index].indexD >= ruleListD.Count + 1) return;
                    break;
                }
        }


        switch (branches[index].curRule)
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
            branches[index].curRule = ruleList[indexRuleList];
        indexRuleList++;
    }

    void RuleUpDown(int index, float bend)
    {
        branches[index].curBend += bend;
        RunNextRule(index);
    }

    void RuleRotate(int index)
    {
        branches[index].curRotation += rotation;
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
        branches[index].indexA = 0;

        if (branches[index].branch == LSystemBranch.A)
        {
            curGeneration++;
            if (curGeneration == branches.Count) return;
        }

        branches[index].branch = LSystemBranch.A;
        RunNextRule(index);
    }

    void RuleBCD(int index, LSystemBranch branch)
    {
        //int newIndex = index + 1;

        GameObject newBranch = Instantiate(branchPrefab, gameObject.transform);
        branches.Add(newBranch.GetComponent<Branch0>());

        int newIndex = branches.Count - 1;

        branches[newIndex].branch = branch;
        AddBranchBaseShape(newIndex, index);

        RunNextRule(index);
        RunNextRule(newIndex);
    }

    void AddBranchBaseShape(int newIndex, int oldIndex)
    {
        branches[newIndex].centerVector = branches[oldIndex].centerVector;
        branches[newIndex].sidesAngle = 360 / sides;
        branches[newIndex].radius = branches[oldIndex].radius;
        branches[newIndex].curBend = branches[oldIndex].curBend;
        branches[newIndex].curRotation = branches[oldIndex].curRotation;
        branches[newIndex].curTapering = branches[oldIndex].curTapering;

        for (int i = 0; i < sides; i++)
        {
            int index = branches[oldIndex].vertices.Count - sides + i;
            branches[newIndex].vertices.Add(branches[oldIndex].vertices[index]);
            branches[newIndex].lerpStartPos.Add(branches[newIndex].vertices[i]);
        }

        /*
        branches[newIndex].centerVector = branches[newIndex - 1].centerVector;
        branches[newIndex].sidesAngle = 360 / sides;
        branches[newIndex].radius = branches[newIndex - 1].radius;
        branches[newIndex].curBend = branches[newIndex - 1].curBend;
        branches[newIndex].curRotation = branches[newIndex - 1].curRotation;
        branches[newIndex].curTapering = branches[newIndex - 1].curTapering;
        

        for (int i = 0; i < sides; i++)
        {
            int index = branches[newIndex - 1].vertices.Count - sides + i;
            branches[newIndex].vertices.Add(branches[newIndex - 1].vertices[index]);
            branches[newIndex].lerpStartPos.Add(branches[newIndex].vertices[i]);
        }
        */
    }


    void AddSegment(int msIndex)
    {
        branches[msIndex].lerpStartPos.Clear();

        //Add verticies
        for (int i = 0; i < sides; i++)
        {
            int index = branches[msIndex].vertices.Count - sides;
            branches[msIndex].vertices.Add(branches[msIndex].vertices[index]);
            branches[msIndex].lerpStartPos.Add(branches[msIndex].vertices[index]);
        }



        for (int i = 0; i < sides; i++)
        {
            int index = branches[msIndex].vertices.Count - sides * 2 + i;

            //Bottom triangle
            branches[msIndex].triangles.Add(index);
            branches[msIndex].triangles.Add(index + sides);

            if (i == sides - 1)
            {
                branches[msIndex].triangles.Add(branches[msIndex].vertices.Count - sides * 2);
            }
            else
            {
                branches[msIndex].triangles.Add(index + 1);
            }

            //Top triangle
            branches[msIndex].triangles.Add(index + sides);

            if (i == sides - 1)
            {
                branches[msIndex].triangles.Add(branches[msIndex].vertices.Count - sides);
                branches[msIndex].triangles.Add(branches[msIndex].vertices.Count - sides * 2);
            }
            else
            {
                branches[msIndex].triangles.Add(index + sides + 1);
                branches[msIndex].triangles.Add(index + 1);
            }
        }
    }


    void CalculateSegmentEndPos(int index)
    {
        //Old center vector position
        Vector3 oldPos = branches[index].centerVector;

        //Calculate new center vector bend
        //meshSegments[index].curBend = bend;
        float px = length * math.sin(branches[index].curBend * math.PI / 180.0f);
        float py = length * math.cos(branches[index].curBend * math.PI / 180.0f);
        float pz = oldPos.z;
        branches[index].centerVector.x = px;
        branches[index].centerVector.y += py;
        branches[index].centerVector.z = pz;

        //calculate new center vector rotation
        //meshSegments[index].curRotation = rotation;
        branches[index].centerVector.x = px * math.cos(branches[index].curRotation * math.PI / 180.0f) + pz * math.sin(branches[index].curRotation * math.PI / 180.0f);
        branches[index].centerVector.z = pz * math.cos(branches[index].curRotation * math.PI / 180.0f) - px * math.sin(branches[index].curRotation * math.PI / 180.0f);

        //Add vertices end pos
        branches[index].lerpEndPos.Clear();

        branches[index].curTapering = tapering;
        branches[index].radius *= branches[index].curTapering;

        float p1x = (branches[index].radius * math.sin(branches[index].sidesAngle * math.PI / 180.0f));
        float ply = -branches[index].radius * math.sin(branches[index].curBend * math.PI / 180.0f) * math.cos(rotation * math.PI / 180.0f);
        float p1z = (branches[index].radius * math.cos(branches[index].sidesAngle * math.PI / 180.0f));

        branches[index].lerpEndPos.Add(new Vector3(branches[index].centerVector.x + p1x, branches[index].centerVector.y + ply, branches[index].centerVector.z + p1z));

        for (int i = 1; i < sides; i++)
        {
            px = p1x * math.cos(branches[index].sidesAngle * i * math.PI / 180.0f) - p1z * math.sin(branches[index].sidesAngle * i * math.PI / 180.0f);
            py = -branches[index].radius * math.sin(branches[index].curBend * math.PI / 180.0f) * math.cos((rotation + branches[index].sidesAngle * i) * math.PI / 180.0f);
            pz = p1x * math.sin(branches[index].sidesAngle * i * math.PI / 180.0f) + p1z * math.cos(branches[index].sidesAngle * i * math.PI / 180.0f);

            branches[index].lerpEndPos.Add(new Vector3(branches[index].centerVector.x + px, branches[index].centerVector.y + py, branches[index].centerVector.z + pz));
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
                int vertIndex = branches[index].vertices.Count - sides + i;
                branches[index].vertices[vertIndex] = Vector3.Lerp(branches[index].lerpStartPos[i], branches[index].lerpEndPos[i], timeElapsed / timeEnd);
                branches[index].UpdateMesh(material);
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }
        RunNextRule(index);
    }
}
