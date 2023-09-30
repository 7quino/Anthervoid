using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandOnInput : MonoBehaviour
{
    public InputActionProperty triggerAction;
    public InputActionProperty gripAction;
    [HideInInspector]
    public Animator handAnimator;

    void Start()
    {
        handAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        float triggerValue = triggerAction.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger", triggerValue);

        float gripValue = gripAction.action.ReadValue<float>();
        handAnimator.SetFloat("Grip", gripValue);
    }
}
