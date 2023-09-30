using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using CommonUsages = UnityEngine.XR.CommonUsages;

public class PickMenuAnther : MonoBehaviour
{
    public InputActionProperty rightActivate;
    [HideInInspector]
    public AnthertorySlot anthertorySlot;
    [HideInInspector]
    public Transform antherHolder;

    bool grabArea = false;
    bool grabbing = false;
    Transform holdPosition;
    GameObject holdingAnther;
    Transform antherModelHolder;
    GameObject anthermodel;

    UnityEngine.XR.InputDevice RightControllerDevice;
    Vector3 RightControllerVelocity;

    private void Start()
    {
        holdPosition = GameObject.FindGameObjectWithTag("RightHand").transform;
        RightControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    private void Update()
    {
        UpdateVelocityInput();

        if (rightActivate.action.ReadValue<float>() > 0.6f && grabArea && !grabbing)
        {
            grabbing = true;

            
            PickGrabbedAnther();
        }

        if (rightActivate.action.ReadValue<float>() < 0.1f && grabbing)
        {
            grabbing = false;

            if (holdingAnther)
            {
                Anthertory.instance.ThrowingAnther();
                holdingAnther.GetComponent<Rigidbody>().isKinematic = false;
                holdingAnther.GetComponent<Rigidbody>().velocity = RightControllerVelocity * 3;
                holdingAnther = null;
            }
            
        }

        if (holdingAnther && grabbing)
        {
            holdingAnther.transform.position = holdPosition.position;
            holdingAnther.transform.rotation = holdPosition.rotation;
        }
    }


    void UpdateVelocityInput()
    {
        RightControllerDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out RightControllerVelocity);
    }


    void PickGrabbedAnther()
    {
        holdingAnther = Instantiate(anthertorySlot.curSlot.anther.plantPrefab, null);
        
        //Create anther model to hold
        Transform t = holdingAnther.transform;
        antherModelHolder = holdingAnther.transform.GetChild(0).gameObject.transform;
        anthermodel = Instantiate(anthertorySlot.curSlot.anther.antherModel, antherModelHolder);
        anthermodel.transform.localScale = Vector3.one;
        anthermodel.GetComponent<MeshRenderer>().sharedMaterial = anthertorySlot.curSlot.anther.antherMaterial;

        //Set antherdata in dropplant
        holdingAnther.GetComponent<Plant>().antherData = Instantiate(anthertorySlot.curSlot.anther);

        Anthertory.instance.RemoveGrabbedAnther(anthertorySlot.index);
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "RightHand") grabArea = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "RightHand") grabArea = true;
    }
}
