using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FlowerGenerator : MonoBehaviour
{
    public FlowerData baseData;
    [HideInInspector]
    public FlowerData flowerData;
    PickPlantAnther pickPlantAnther;

    [Header("Generate flower on start")]
    public Transform flowerParent;
    public bool generateFlower = false;
    public AntherData antherData;

    Material[] emissiveMaterials = new Material[1];
    Material[] antherMaterials = new Material[2];
    Material[] ovuleMaterials = new Material[2];
    Material[] petalMaterial = new Material[1];

    private void Start()
    {
        if (generateFlower)
        {
            CreateBaseData(antherData);
            GameObject flower = CreateFlower(flowerParent, antherData);
            flower.transform.position = flowerParent.position;
        }
    }

    public void CreateBaseData(AntherData antherData)
    {
        flowerData = Instantiate(baseData);
        flowerData.antherIndex = Random.Range(0, flowerData.antherObjects.Length);
        flowerData.ovuleIndex = Random.Range(0, flowerData.ovuleObjects.Length);
        flowerData.petalIndex = Random.Range(0, flowerData.petalObjects.Length);
        flowerData.receptacleIndex = Random.Range(0, flowerData.receptacleObjects.Length);
        flowerData.nrPetals = Random.Range(5, 12);
        flowerData.size = Random.Range(1, flowerData.maxSize);
        SetFlowerMaterials(antherData);
    }

    public GameObject CreateFlower(Transform parentObject, AntherData antherData)
    {
        GameObject flower = new GameObject();
        flower.transform.parent = parentObject.transform;
        flower.name = "Flower";
        
        GameObject offsetFlower = new GameObject();
        offsetFlower.transform.parent = flower.transform;
        offsetFlower.name = "FlowerOffset";

        //create flower objects
        CreateFlowerPartObject("Anther", flowerData.antherIndex, flowerData.antherObjects, flowerData.antherMaterials, offsetFlower.transform);
        CreateFlowerPartObject("Ovule", flowerData.ovuleIndex, flowerData.ovuleObjects, flowerData.ovuleMaterials, offsetFlower.transform);
        float offset = CreateFlowerPartObject("Receptacle", flowerData.receptacleIndex, flowerData.receptacleObjects, flowerData.receptacleMaterials, offsetFlower.transform);
        CreatePetalObjects(offsetFlower.transform);

        offsetFlower.transform.position += new Vector3(0, offset / 2, 0);

        //Add PickPlantAnther data
        pickPlantAnther = flower.AddComponent<PickPlantAnther>();
        pickPlantAnther.antherData = antherData;
        pickPlantAnther.flowerOffset = offsetFlower;
        pickPlantAnther.SetUpComponent();
        return flower;
    }

    public void SetFlowerMaterials(AntherData antherData)
    {
        emissiveMaterials[0] = antherData.antherMaterial;
        flowerData.antherMaterials[0] = emissiveMaterials[0];
        flowerData.ovuleMaterials[1] = emissiveMaterials[0];

        float colorGradient = Random.Range(0.0f, 1.0f);
        float shape1Gradient = Random.Range(0.0f, 1.0f);
        float shape2Gradient = Random.Range(0.0f, 1.0f);
        petalMaterial[0] = Instantiate(flowerData.petalMaterials[0]);
        petalMaterial[0].SetFloat("_ColorPosition", colorGradient);
        petalMaterial[0].SetFloat("_ColorPositionShapeLayer1", shape1Gradient);
        petalMaterial[0].SetFloat("_ColorPositionShapeLayer2", shape2Gradient);
    }

    public float CreateFlowerPartObject(string name, int index, GameObject[] meshObject, Material[] materials, Transform parentObject)
    {
        GameObject flowerObject = new GameObject();
        flowerObject.transform.parent = parentObject;
        flowerObject.name = name;

        MeshFilter meshFilter = flowerObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = flowerObject.AddComponent<MeshRenderer>();

        meshFilter.mesh = meshObject[index].GetComponent<MeshFilter>().sharedMesh;
        meshRenderer.materials = materials;

        return meshFilter.mesh.bounds.size.y;
    }

    public void CreatePetalObjects(Transform parentObject)
    {
        GameObject petalObject = new GameObject();
        petalObject.transform.parent = parentObject;
        petalObject.name = "Petal";

        SkinnedMeshRenderer meshRenderer = petalObject.AddComponent<SkinnedMeshRenderer>();
        meshRenderer.sharedMesh = flowerData.petalObjects[flowerData.petalIndex].GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
        meshRenderer.materials = petalMaterial;
        
        //Generate petals
        float sideAngle = 360 / flowerData.nrPetals;
        for (int i = 1; i < flowerData.nrPetals; i++)
        {
            GameObject petalInstans = Instantiate(petalObject, parentObject);
            petalInstans.name = "Petal";
            petalInstans.transform.Rotate(Random.Range(-flowerData.randomPetalAngle, flowerData.randomPetalAngle), sideAngle * i + Random.Range(-flowerData.randomPetalAngle, flowerData.randomPetalAngle), Random.Range(-flowerData.randomPetalAngle, flowerData.randomPetalAngle));
            petalInstans.transform.localScale = Vector3.one * (1 + Random.Range(-flowerData.randomPetalSize, flowerData.randomPetalSize));
            petalInstans.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, Random.Range(0, 100));
        }
    }
}
