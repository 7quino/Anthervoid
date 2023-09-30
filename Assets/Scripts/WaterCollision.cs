using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class WaterCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Water")
        {
            Destroy(GetComponent<Collider>());
            Destroy(GetComponent<Rigidbody>());
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            gameObject.GetComponent<Plant>().enabled = true;
            Destroy(gameObject.transform.GetChild(0).gameObject, 10);
            Destroy(this, 10);
        } 
    }
}
