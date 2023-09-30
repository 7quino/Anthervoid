using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class AnthertoryRotate : MonoBehaviour
{
    public InputActionProperty rotateAction;
    public float rotationSpeed = 50.0f;
    
    void Update()
    {
        Vector2 rotate = rotateAction.action.ReadValue<Vector2>();
        if (rotate.x != 0)
        {
            transform.Rotate(0, (-rotate.x) * rotationSpeed * Time.deltaTime, 0);
        }
    }
}
