using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "Flower", menuName = "New Flower")]
public class FlowerData : ScriptableObject
{
    public string id;

    [Header("Resources")]
    public GameObject[] antherObjects;
    public GameObject[] ovuleObjects;
    public GameObject[] petalObjects;
    public GameObject[] receptacleObjects;

    public Material[] antherMaterials;
    public Material[] ovuleMaterials;
    public Material[] petalMaterials;
    public Material[] receptacleMaterials;

    [Header("Data")]
    [HideInInspector]
    public int nrPetals;
    public float randomPetalAngle;
    public float randomPetalSize;
    public int maxSize;
    [HideInInspector]
    public int size;
    public int growthSpeed;

    [HideInInspector]
    public int antherIndex;
    [HideInInspector]
    public int ovuleIndex;
    [HideInInspector]
    public int petalIndex;
    [HideInInspector]
    public int receptacleIndex;
}
