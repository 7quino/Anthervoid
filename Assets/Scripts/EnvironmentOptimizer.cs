using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class EnvironmentOptimizer: MonoBehaviour
{
    [HideInInspector]
    public GameObject[] waterPlanes;
    [HideInInspector]
    public List<GameObject> plants = new List<GameObject>();
    public float visibleDistancePlant;
    public float visibleDistanceWater;
    public float checkRate;

    Vector3 playerPos;

    void Start()
    {
        waterPlanes = GameObject.FindGameObjectsWithTag("Water");
        plants.AddRange(GameObject.FindGameObjectsWithTag("Plant"));

        InvokeRepeating("CheckDistance", 0.0f, checkRate);
    }

    void CheckDistance()
    {
        playerPos = transform.position;
        playerPos.y = 0;

        foreach (GameObject water in waterPlanes)
        {
            Vector3 plantCenterPos = water.transform.position;
            plantCenterPos.y = 0;

            if (Vector3.Distance(playerPos, plantCenterPos) > visibleDistanceWater)
                water.SetActive(false);
            else
                water.SetActive(true);
        }

        foreach (GameObject plant in plants)
        {
            Vector3 plantCenterPos = plant.transform.position;
            plantCenterPos.y = 0;

            if (Vector3.Distance(playerPos, plantCenterPos) > visibleDistancePlant)
                plant.SetActive(false);
            else
                plant.SetActive(true);
        }
    }
}
