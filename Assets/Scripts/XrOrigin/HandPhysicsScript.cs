using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HandPhysicsScript : MonoBehaviour
{
    private Collider[] handColliders;

    // Start is called before the first frame update
    void Start()
    {
        handColliders = GetComponentsInChildren<Collider>();
    }

   public void EnableHandColliderDelay(float delay)
    {
        Invoke("EnableHandCollider", delay);
    }

    public void EnableHandCollider()
    {
        foreach(var item in handColliders)
        {
            item.enabled = true;
        }
    }

    public void DisbleHandCollider()
    {
        foreach (var item in handColliders)
        {
            item.enabled = false;
        }
    }

}
