using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Leaf", menuName = "New Leaf")]
public class LeafData : ScriptableObject
{
    public string id;

    [Header("Resources")]
    public GameObject[] leafObjects;
    public Material leafMaterial;

    [Header("Data")]
    public float randomSize;
    public float randomAngle;
    public float maxSize;
    [HideInInspector]
    public float size;
    public int growthSpeed;

    public int leafIndex;
}
