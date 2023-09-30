using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PickPlantAnther : MonoBehaviour
{
    public AntherData antherData;
    public GameObject flowerOffset;
    
    List<GameObject> petalObjects = new List<GameObject>();
    List<SkinnedMeshRenderer> petalRenderers = new List<SkinnedMeshRenderer>();
    GameObject antherObject;
    MeshRenderer antherRenderer;
    GameObject ovuleObject;
    MeshRenderer ovuleRenderer;
    MeshRenderer receptacleRenderer;

    Material[] hoverMaterials = new Material[2];
    Material petallMaterial;
    [SerializeField]
    Material[] antherMaterials;
    [SerializeField]
    Material[] ovuleMaterials;
    Material reseptacleMaterial;

    bool isPicked = false;

    public void SetUpComponent()
    {
        hoverMaterials[0] = antherData.antherMaterial;
        hoverMaterials[1] = antherData.antherMaterial;

        //Set up variables
        foreach (Transform child in flowerOffset.transform)
        {
            switch (child.gameObject.name)
            {
                case "Petal":
                    {
                        petalObjects.Add(child.gameObject);
                        petalRenderers.Add(child.gameObject.GetComponent<SkinnedMeshRenderer>());
                        if (!petallMaterial) petallMaterial = child.gameObject.GetComponent<SkinnedMeshRenderer>().material;
                        break;
                    }
                case "Anther":
                    {
                        antherObject = child.gameObject;
                        antherRenderer = child.gameObject.GetComponent<MeshRenderer>();
                        antherMaterials = antherRenderer.materials;
                        break;
                    }
                case "Ovule":
                    {
                        ovuleObject = child.gameObject;
                        ovuleRenderer = child.gameObject.GetComponent<MeshRenderer>();
                        ovuleMaterials = ovuleRenderer.materials;
                        break;
                    }
                case "Receptacle":
                    {
                        receptacleRenderer = child.gameObject.GetComponent<MeshRenderer>();
                        reseptacleMaterial = receptacleRenderer.material;
                        break;
                    }
            }
        }

        //Generate components for interaction
        foreach (GameObject petal in petalObjects)
        {
            petal.layer = 13;
            petal.AddComponent<BoxCollider>();
            Rigidbody rb = petal.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            XRSimpleInteractable petalInteractor = petal.AddComponent<XRSimpleInteractable>();
            petalInteractor.hoverEntered.AddListener((function) => OnHoverEnterFlower());
            petalInteractor.hoverExited.AddListener((function) => OnHoverExitFlower());
            petalRenderers.Add(petal.GetComponent<SkinnedMeshRenderer>());
        }
    }

    public void OnHoverEnterFlower()
    {
        if (isPicked) return;

        foreach (SkinnedMeshRenderer renderer in petalRenderers)
        {
            renderer.material = hoverMaterials[0];
        }
        antherRenderer.materials = hoverMaterials;
        ovuleRenderer.materials = hoverMaterials;
        receptacleRenderer.material = hoverMaterials[0];
        ActivateHandRay.instance.onActivateAction.AddListener(OnSelectFlower);
    }

    public void OnHoverExitFlower()
    {
        if (isPicked) return;

        foreach (SkinnedMeshRenderer renderer in petalRenderers)
        {
            renderer.material = petallMaterial;
        }
        antherRenderer.materials = antherMaterials;
        ovuleRenderer.materials = ovuleMaterials;
        receptacleRenderer.material = reseptacleMaterial;
        ActivateHandRay.instance.onActivateAction.RemoveListener(OnSelectFlower);
    }

    public void OnSelectFlower()
    {
        if (!isPicked)
        {
            isPicked = true;
            Anthertory.instance.AddItem(antherData);
        }
        
        foreach (SkinnedMeshRenderer renderer in petalRenderers)
        {
            renderer.material = petallMaterial;
        }
        receptacleRenderer.material = reseptacleMaterial;
        ActivateHandRay.instance.onActivateAction.RemoveListener(OnSelectFlower);
        antherMaterials[0] = antherData.transparentMaterial;
        ovuleMaterials[1] = antherData.transparentMaterial;
        antherRenderer.materials = antherMaterials;
        ovuleRenderer.materials = ovuleMaterials;
    }
}



