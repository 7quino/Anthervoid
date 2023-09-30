using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunCycle : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float time;
    public float timeOffset;
    public float fullDayLenght;
    //Default starttime sen morgon, 0.5f = noon
    public float startTime = 0.4f;
    private float timeRate;
    public Vector3 noon;
    public Vector3 rotationOffset;

    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;

    private void Start()
    {
        timeRate = 1.0f / fullDayLenght;
        time = startTime;
    }

    private void Update()
    {
        time += timeRate * Time.deltaTime;

        if (time >= 1.0f)
        {
            time = 0.0f;
        }

        //light rotation
        sun.transform.eulerAngles = ((time - timeOffset) * noon * 4.0f) + rotationOffset;

        //light intensity, räkna ut värdet på animationskurvan vid en given tid
        sun.intensity = sunIntensity.Evaluate(time);

        //Change colors
        sun.color = sunColor.Evaluate(time);

        //enable disable
        if (sun.intensity == 0 && sun.gameObject.activeInHierarchy)
        {
            sun.gameObject.SetActive(false);
        }
        else if (sun.intensity > 0 && !sun.gameObject.activeInHierarchy)
        {
            sun.gameObject.SetActive(true);
        }
    }
}
