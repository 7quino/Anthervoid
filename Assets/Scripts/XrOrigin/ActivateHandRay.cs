using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ActivateHandRay : MonoBehaviour
{
    public XRRayInteractor rightRay;
    public XRRayInteractor leftRay;

    public XRInteractorLineVisual leftLine;
    public XRInteractorLineVisual rightLine;

    public InputActionProperty leftActivate;
    public InputActionProperty rightActivate;

    bool holdingAnther = false;
    bool inventoryIsOpen = false;
    float rightLineLength;
    float rightRayInteractionLength;

    public UnityEvent onActivateAction;
    bool activatePerformed = false;

    public static ActivateHandRay instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        rightLineLength = rightLine.lineLength;
        rightRayInteractionLength = rightRay.maxRaycastDistance;

        Anthertory.instance.onOpenInventory.AddListener(InventoryRay);
        Anthertory.instance.onCloseInventory.AddListener(NormalRay);
        Anthertory.instance.onGrabbingAnther.AddListener(HoldingAnter);
        Anthertory.instance.onThrowingAnther.AddListener(NotHoldingAnther);
    }

    void Update()
    {
        //Activate buttonn pressed event
        if (rightActivate.action.ReadValue<float>() > 0.1f && !activatePerformed) StartCoroutine(ActivateAction());

        //Hand ray hit
        bool isRightRayHovering = rightRay.TryGetHitInfo(out Vector3 rightPos, out Vector3 rightNormal, out int rightNumber, out bool rightValid);
        bool isLeftRayHovering = leftRay.TryGetHitInfo(out Vector3 leftPos, out Vector3 leftNormal, out int leftNumber, out bool leftValid);

        if (!inventoryIsOpen && !holdingAnther)
        {
            leftLine.enabled = (!isRightRayHovering && leftActivate.action.ReadValue<float>() > 0.1f);
            rightLine.enabled = (!isLeftRayHovering && rightActivate.action.ReadValue<float>() > 0.1f);
        }
    }

    IEnumerator ActivateAction()
    {
        activatePerformed = true;
        onActivateAction.Invoke();
        yield return new WaitForSeconds(0.5f);
        activatePerformed = false;
    }

    void InventoryRay()
    {
        inventoryIsOpen = true;
        leftLine.enabled = false;


        rightLine.enabled = true;
        rightLine.lineLength = 0.5f;
        rightRay.maxRaycastDistance = 0.7f;
    }

    void NormalRay()
    {
        inventoryIsOpen = false;
        rightLine.lineLength = rightLineLength;
        rightRay.maxRaycastDistance = rightRayInteractionLength;
    }

    void HoldingAnter()
    {
        holdingAnther = true;
        leftLine.enabled = false;
        rightLine.enabled = false;
    }

    void NotHoldingAnther()
    {
        holdingAnther = false;
        if (inventoryIsOpen) rightLine.enabled = true;
    }
}
