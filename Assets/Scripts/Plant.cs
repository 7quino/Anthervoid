using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class Plant : MonoBehaviour
{
    [Header("Branch Resources")]
    public AntherData antherData;
    public GameObject flowerPrefab;
    FlowerGenerator flowerGenerator;
    public GameObject leafPrefab;
    LeafGenerator leafGenerator;
    public Material material;
    [HideInInspector]
    public Material stemMaterial;


    [Header("Branch Properties")]
    protected int sides = 3;
    public int segments = 3;
    public float radius = 0.07f;
    public float lenght = 1.2f;
    public float scale = 0.9f;
    public float rotation = 40;
    public float bend = 15;
    public float tapering = 0.8f;
    public float randomness = 0.15f;
    public bool lerpGrowth = true;
    public float growthSpeed = 1;
    public float growthMultiplierRadius = 4;
    public int stemGenerations = 2;
    public int rootGenerations = 2;
    public float endSegmentLenght = 2f;

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

    //Private rules
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
    List<PlantBranch> branches = new List<PlantBranch>();
    float sideAngle = 0;

    void Start()
    {
        SetData();

        CreateRuleQueues();

        flowerGenerator = Instantiate(flowerPrefab, transform).GetComponent<FlowerGenerator>();
        flowerGenerator.CreateBaseData(antherData);

        leafGenerator = Instantiate(leafPrefab, transform).GetComponent<LeafGenerator>();
        leafGenerator.CreateBaseData();

        SetMaterialProperties();

        CreateFirstBranch(0);
        CreateFirstBranch(1);

        StartCoroutine(PositionRotatePlant());
    }

    void SetData()
    {
        foreach (PropertyData propertyData in antherData.properties)
        {
            switch (propertyData.propertyType)
            {
                case PropertyType.Length: lenght = propertyData.value; break;
                case PropertyType.Radius: radius = propertyData.value; break;
                case PropertyType.Rotation: rotation = propertyData.value; break;
                case PropertyType.Bend: bend = propertyData.value; break;
                case PropertyType.Scale: scale = propertyData.value; break;
                case PropertyType.Curve: amplitude = propertyData.value; break;
            }
        }

        foreach (RuleData ruleData in antherData.rules)
        {
            switch (ruleData.ruleType)
            {
                case RuleType.A: ruleA = ruleData.rule; break;
                case RuleType.B: ruleB = ruleData.rule; break;
                case RuleType.C: ruleC = ruleData.rule; break;
                case RuleType.D: ruleD = ruleData.rule; break;
                case RuleType.E: ruleE = ruleData.rule; break;
                case RuleType.F: ruleF = ruleData.rule; break;
                case RuleType.G: ruleG = ruleData.rule; break;
                case RuleType.H: ruleH = ruleData.rule; break;
            }
        }

        sides = Random.Range(3,6);
        growthSpeed += Random.Range(-0.5f, 2.0f);
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
        float gradient = Random.Range(0.0f, 1.0f);

        stemMaterial = Instantiate(material);
        stemMaterial.SetFloat("_ColorPosition", gradient);
        leafGenerator.SetMaterialProperties(gradient);
    }

    IEnumerator PositionRotatePlant()
    {
        yield return new WaitForSeconds(0.3f);

        float timeElapsed = Time.deltaTime;
        float timeEndRot = 0.1f + Time.deltaTime;
        float timeEndPos = 50 + Time.deltaTime;
        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(transform.position.x, transform.position.y + Random.Range(-0.7f, 1.5f), transform.position.z);
        Vector3 rotation = new Vector3(0, Random.Range(-180, 180), 0);

        while (timeElapsed < timeEndRot)
        {
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(rotation), timeElapsed / timeEndRot);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        while (timeElapsed < timeEndPos)
        {
            transform.position = Vector3.Lerp(startPos, endPos, timeElapsed / timeEndPos);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }


    //Create branch object
    void CreateBranchObject(int id)
    {
        GameObject newBranch = new GameObject("Branch");
        newBranch.transform.parent = transform;
        newBranch.transform.localPosition = Vector3.zero;
        newBranch.AddComponent<PlantBranch>();
        branches.Add(newBranch.GetComponent<PlantBranch>());
        branches[id].id = id;
        branches[id].meshGenerator = gameObject.GetComponent<Plant>();
    }

    void CreateFirstBranch(int id)
    {
        CreateBranchObject(id);

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
        int upDirection = branches[id].stem ? 1 : -1;
        branches[id].lenghtV = new Vector3(0, (upDirection * lenght) / segments, 0);
        branches[id].radiusV *= radius;
        sideAngle = 360 / sides;
        branches[id].centerVector = branches[id].lenghtV;
        if (!branches[id].stem) branches[id].radiusV = Quaternion.AngleAxis(-180, branches[id].rotateAxis) * branches[id].radiusV;
        if (!branches[id].stem) branches[id].transform.eulerAngles = new Vector3(0, -180, 0);

        for (int i = 0; i < sides; i++)
        {
            branches[id].vertices.Add(Vector3.zero);
            branches[id].lerpEndPos.Add(Quaternion.AngleAxis(sideAngle * i, (upDirection * Vector3.up)) * branches[id].radiusV);
        }

        StartCoroutine(LerpGrowth(id, growthSpeed * growthMultiplierRadius, 0, AveragePointPositions(branches[id].lerpEndPos), LerpPointPositions(branches[id].lerpEndPos, 0), true));
    }

      public void AddBranch(int id, LSystemBranch branch)
    {
        if (branch == LSystemBranch.A || branch == LSystemBranch.E)
        {
            if (branches[id].ruleIndex == branches[id].rules.Count) branches[id].curGen++;

            branches[id].rules.Clear();
            if (branch == LSystemBranch.A) branches[id].rules.AddRange(ruleListA);
            if (branch == LSystemBranch.E) branches[id].rules.AddRange(ruleListE);
            branches[id].ruleIndex = 0;
            branches[id].RunNextRule();
            return;
        }

        int newId = branches.Count;
        CreateBranchObject(newId);
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
        branches[newId].randomFactor = branches[oldId].randomFactor;

        for (int i = 0; i < sides; i++)
        {
            int index = branches[oldId].vertices.Count - sides + i;
            branches[newId].vertices.Add(branches[oldId].vertices[index]);
            branches[newId].lerpStartPos.Add(branches[newId].vertices[i]);
        }

        StartCoroutine(LerpGrowth(newId, growthSpeed * growthMultiplierRadius, 0, LerpPointPositions(branches[newId].vertices, 0), LerpPointPositions(branches[oldId].lerpEndPos, 0), false));
    }



    //Rules
    public void Randomize(int id)
    {
        branches[id].randomFactor += Random.Range(-randomness, randomness);
        branches[id].spaceVector += branches[id].centerVector * branches[id].randomFactor;
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
        if (branches[id].meshGenerator.lerpGrowth) StartCoroutine(ForwardGrowth(id));
        else StartCoroutine(InstantGrowth(id));
    }

    public void ForwardPos(int id)
    {
        //Adding curvature
        curLenght += lenght / segments;
        float curveheight = amplitude * math.sin(branches[id].spaceVector.magnitude / curveLenght);
        curveVector = new Vector3(curveheight, 0, curveheight);
        //curveVector = Quaternion.AngleAxis(rotation * branches[id].randomFactor, Vector3.up) * curveVector;
        //curveVector = new Vector3(curveVector.x, 0, curveVector.z);

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



    //Lerp functions
    Vector3 AveragePoint(List<Vector3> points, int index)
    {
        Vector3 averagePoint = Vector3.zero;
        for (int i = 0; i < sides; i++)
        {
            averagePoint += points[index + i];
        }
        averagePoint /= sides;
        return averagePoint;
    }


    List<Vector3> AveragePointPositions(List<Vector3> points)
    {
        Vector3 avgPoint = new Vector3();
        foreach (Vector3 point in points) avgPoint += point;
        avgPoint /= points.Count;

        List<Vector3> avgPoints = new List<Vector3>();
        for (int i = 0; i < sides; i++) avgPoints.Add(avgPoint);
        return avgPoints;
    }


    List<Vector3> LerpPointPositions(List<Vector3> points, int vertexIndex)
    {
        List<Vector3> lerpPositions = new List<Vector3>();
        for (int i = 0; i < sides; i++) lerpPositions.Add(points[vertexIndex + i]);
        return lerpPositions;
    }


    public IEnumerator InstantGrowth(int id)
    {
        for (int j = 0; j < branches[id].meshGenerator.segments; j++)
        {
            Randomize(id);
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


    //General lerp growth function
    public IEnumerator LerpGrowth(int id, float lerpTime, int vertexIndex, List<Vector3> startPos, List<Vector3> endPos, bool updateMesh)
    {
        float timeElapsed = Time.deltaTime;
        float timeEnd = lerpTime + Time.deltaTime;

        while (timeElapsed < timeEnd)
        {
            for (int i = 0; i < sides; i++)
            {
                branches[id].vertices[vertexIndex + i] = Vector3.Lerp(startPos[i], endPos[i], timeElapsed / timeEnd);
                if (updateMesh) branches[id].UpdateMesh(stemMaterial);
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }


    public IEnumerator ForwardGrowth(int id)
    {
        for (int j = 0; j < branches[id].meshGenerator.segments; j++)
        {
            Randomize(id);
            ForwardPos(id);

            yield return StartCoroutine(LerpGrowth(id, growthSpeed, branches[id].vertices.Count - sides, branches[id].lerpStartPos, AveragePointPositions(branches[id].lerpEndPos), true));

            StartCoroutine(LerpGrowth(id, growthSpeed * growthMultiplierRadius, branches[id].vertices.Count - sides,
                LerpPointPositions(branches[id].vertices, branches[id].vertices.Count - sides), LerpPointPositions(branches[id].lerpEndPos, 0), true));

        }
        branches[id].RunNextRule();
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
        yield return StartCoroutine(LerpGrowth(id, growthSpeed, branches[id].vertices.Count - sides, branches[id].lerpStartPos, branches[id].lerpEndPos, true));

        if (branches[id].stem && createFlower) CreateFlower(id);
        else if (branches[id].stem && !createFlower) CreateLeaf(id);
    }

    public void CreateFlower(int id)
    {
        GameObject flower = flowerGenerator.CreateFlower(branches[id].transform, antherData);
        float size = flowerGenerator.flowerData.size;
        float growthSpeed = branches[id].meshGenerator.flowerGenerator.flowerData.growthSpeed;

        Vector3 averagePoint = AveragePoint(branches[id].vertices, branches[id].vertices.Count - sides * 2);
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

        Vector3 averagePoint = AveragePoint(branches[id].vertices, branches[id].vertices.Count - sides * 2);
        Vector3 leafAngle = (branches[id].vertices[branches[id].vertices.Count - 1] - averagePoint);
        Quaternion rot = Quaternion.LookRotation(leafAngle, Vector3.up);
        leaf.transform.localRotation = rot;
        leafOffset.transform.localPosition = new Vector3(0, -branches[id].lenghtV.magnitude / 5, 0);;
        leaf.transform.Rotate(90, 0, 0);
        leaf.transform.localPosition = branches[id].vertices[branches[id].vertices.Count - 1];

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




public class PlantBranch : MonoBehaviour
{
    public int id;

    //mesh
    [HideInInspector]
    public Mesh mesh;
    [HideInInspector]
    public MeshRenderer meshRenderer;
    [HideInInspector]
    public Plant meshGenerator;

    //Branch Variables
    public Vector3 centerVector = Vector3.zero;
    public Vector3 spaceVector = Vector3.zero;
    [HideInInspector]
    public Vector3 rotateAxis = Vector3.up;
    public Vector3 bendAxis = Vector3.forward;
    public Vector3 radiusV = Vector3.one;
    public Vector3 lenghtV = Vector3.one;
    [HideInInspector]
    public float curRotation = 0;
    [HideInInspector]
    public float curBend = 0;
    [HideInInspector]
    public int generations = 0;
    public int curGen = 0;
    public float randomFactor = 1;
    public bool stem = true;

    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    [HideInInspector]
    public List<Vector3> lerpStartPos = new List<Vector3>();
    [HideInInspector]
    public List<Vector3> lerpEndPos = new List<Vector3>();


    //Rules
    [HideInInspector]
    public LSystemBranch branch = LSystemBranch.A;
    public List<char> rules = new List<char>();
    public int ruleIndex = 0;
    [HideInInspector]
    public char curRule;

    void Start()
    {
        assignMesh();

        //if (id == 1) transform.eulerAngles = new Vector3(0, 180, 0);

        if (id == 0 || id == 1)
        {
            meshGenerator.CreateBaseShape(id);
            RunNextRule();
        }
    }

    public void assignMesh()
    {
        if (!gameObject.GetComponent<MeshFilter>()) gameObject.AddComponent<MeshFilter>();
        if (!gameObject.GetComponent<MeshRenderer>()) gameObject.AddComponent<MeshRenderer>();

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

