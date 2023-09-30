using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergySource : MonoBehaviour
{
    [HideInInspector]
    public Transform energyTransform;
    public float pulseTime = 2.0f;

    Vector3 size;

    void Start()
    {
        energyTransform = GetComponent<Transform>();
        size = energyTransform.localScale;
    }

    void Update()
    {
        var amplitude = Mathf.PingPong(Time.time, pulseTime);
        amplitude = amplitude / pulseTime * 0.5f + 0.5f;
        energyTransform.localScale = size * amplitude;
    }
}
