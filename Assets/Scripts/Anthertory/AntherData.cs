using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AntherType
{
    Amborellales,
    Nymphaeales,
    Magnoliids,
    Chloranthales,
    Monocots,
    Eudicots
}

public enum PropertyType
{
    Length,
    Radius,
    Rotation,
    Bend,
    Scale,
    Curve,
}

public enum RuleType
{
    A,
    B,
    C,
    D,
    E,
    F,
    G,
    H
}

[CreateAssetMenu(fileName = "Anther", menuName = "New Anther")]
public class AntherData : ScriptableObject
{
    public string id;

    [Header("Prefabs")]
    public Material transparentMaterial;
    public Material antherMaterial;
    public GameObject antherModel;
    public GameObject antherPrefab;
    public GameObject plantPrefab;

    [Header("Info")]
    public AntherType type;
    public Color uiColor;
    public string description;
    public int collected;
    public int quantity;
    
    public float genergy;
    public float splicergy;

    [Header("Stacking")]
    public bool canStack;
    public int maxStackAmount;

    [Header("Properties")]
    public int gen;
    public PropertyData[] properties;
    public RuleData[] rules;
}

[System.Serializable]
public class PropertyData
{
    public PropertyType propertyType;
    public float value;
    public float minValue;
    public float maxValue;
}

[System.Serializable]
public class RuleData
{
    public RuleType ruleType;
    public string rule;
}
