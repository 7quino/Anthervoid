using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerGeneratorOld : MonoBehaviour
{
    public GameObject[] antherMesh;
    public GameObject[] ovuleMesh;
    public GameObject[] petalMesh;
    public GameObject[] receptacleMesh;

    public Material[] antherMaterials;
    public Material[] ovuleMaterials;
    public Material[] petalMaterials;
    public Material[] receptacleMaterials;

    public float randomAngle;
    public float randomSize;
    public bool generateFlower = false;

    //Flower data
    int antherIndex;
    int ovuleIndex;
    int petalIndex;
    int receptacleIndex;

    void Start()
    {
        CreateFlowerPartObject("Anther", antherIndex, antherMesh, antherMaterials);
        CreateFlowerPartObject("Ovule", ovuleIndex, ovuleMesh, ovuleMaterials);
        CreateFlowerPartObject("Receptacle", receptacleIndex, receptacleMesh, receptacleMaterials);

        GameObject petalobject = CreateFlowerPartObject("Petal1", petalIndex, petalMesh, petalMaterials);
        int nrPetals = Random.Range(3, 9);
        float sideAngle = 360 / nrPetals;
        for (int i = 1; i < nrPetals; i++)
        {
            GameObject petalInstans = Instantiate(petalobject, transform);
            petalInstans.name = "Petal" + (i + 1).ToString();
            petalInstans.transform.Rotate( Random.Range(-randomAngle, randomAngle), sideAngle * i + Random.Range(-randomAngle, randomAngle), Random.Range(-randomAngle, randomAngle));
            petalInstans.transform.localScale = Vector3.one * (1 + Random.Range(-randomSize, randomSize));
        }
    }

    GameObject CreateFlowerPartObject(string name, int index, GameObject[] meshObject, Material[] materials)
    {
        GameObject flowerObject = new GameObject();
        flowerObject.transform.parent = transform;
        flowerObject.name = name;

        MeshFilter meshFilter = flowerObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = flowerObject.AddComponent<MeshRenderer>();

        index = Random.Range(0, meshObject.Length);
        meshFilter.mesh = meshObject[index].GetComponent<MeshFilter>().sharedMesh;
        meshRenderer.materials = materials;

        return flowerObject;
    }
}
