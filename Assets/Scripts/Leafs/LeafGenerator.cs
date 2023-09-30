using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafGenerator : MonoBehaviour
{
    public LeafData baseData;
    [HideInInspector]
    public LeafData leafData;
    Material[] leafMaterial = new Material[1];

    public void CreateBaseData()
    {
        leafData = Instantiate(baseData);
        leafData.leafIndex = Random.Range(0, baseData.leafObjects.Length);
        leafData.size = Random.Range(1, leafData.maxSize);
    }

    public void SetMaterialProperties(float gradient)
    {
        leafMaterial[0] = Instantiate(leafData.leafMaterial);
        leafMaterial[0].SetFloat("_ColorPosition", gradient + Random.Range(-0.1f,0.1f));
    }

    public GameObject CreateLeaf(Transform parentObject)
    {
        GameObject leaf = new GameObject();
        leaf.transform.parent = parentObject.transform;
        leaf.name = "LeafMesh";

        SkinnedMeshRenderer meshRenderer = leaf.AddComponent<SkinnedMeshRenderer>();
        meshRenderer.sharedMesh = leafData.leafObjects[leafData.leafIndex].GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;

        //Material[] leafMaterial = new Material[] {leafData.leafMaterialInstance};
        meshRenderer.materials = leafMaterial;

        leaf.transform.Rotate(0, Random.Range(-leafData.randomAngle, leafData.randomAngle), 0);
        leaf.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(0, Random.Range(0, 100));

        return leaf;
    }
}
