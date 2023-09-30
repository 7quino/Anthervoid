using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingPlanet : MonoBehaviour
{
    [HideInInspector]
    public Renderer planetRenderer;
    [HideInInspector]
    public AudioSource audioSource;

    public GameObject collidingPlanet;
    public GameObject explosionStone;
    public GameObject explosionPlanet;
    //public Gradient emissiveColor;
    public AudioClip explosionClip;
    public AudioClip metereorClip;

    //public float glowStrenght;
    //public float glowStrenghtHigh;
    //public float glowStrenghtLow;
    public float glowTime;
    public float explosionAmount;
    public float explosionTime;

    bool hasExploded = false;

    private void Start()
    {
        planetRenderer = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();

        explosionPlanet.SetActive(false);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == collidingPlanet && !hasExploded)
        {
            hasExploded = true;
            StartCoroutine(PlayAudio());
            StartCoroutine(GlowUpDown());
            StartCoroutine(Explosion());
            StartCoroutine(ExplosionPlanet());
        }
    }

    IEnumerator PlayAudio()
    {
        audioSource.clip = explosionClip;
        audioSource.PlayOneShot(explosionClip);

        yield return new WaitForSeconds(explosionClip.length);
        //yield return new WaitForSeconds(1);

        audioSource.clip = metereorClip;
        audioSource.PlayOneShot(metereorClip);
    }

    IEnumerator ExplosionPlanet()
    {
        for (int i = 0; i < 6; i++)
        {
            explosionPlanet.SetActive (true);
            yield return new WaitForSeconds(Random.Range(0.05f, 0.3f));
            explosionPlanet.SetActive(false);
            yield return new WaitForSeconds(Random.Range(0.05f, 0.3f));
        }
    }


    IEnumerator GlowUpDown()
    {
        float timeElapsed = Time.deltaTime;
        float timeEnd = glowTime + Time.deltaTime;
        float curGlowTime;
        Vector3 size = transform.localScale;
        Vector3 curSize;

        while (timeElapsed < timeEnd)
        {
            curGlowTime = timeElapsed / timeEnd;
            //planetRenderer.material.SetColor("_EmissionColor", emissiveColor.Evaluate(curGlowTime) * glowStrenght);

            curSize = Vector3.Lerp(size, new Vector3(0, 0, 0), timeElapsed / timeEnd);
            transform.localScale = curSize;

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        planetRenderer.material.SetColor("_EmissionColor", Color.black * 0);

        /*
        while (timeElapsed < timeEnd)
        {
            curGlowTime = timeElapsed / timeEnd;
            glowStrenght = Mathf.Lerp(glowStrenghtLow, glowStrenghtHigh, timeElapsed / timeEnd);
            planetRenderer.material.SetColor("_EmissionColor", emissiveColor.Evaluate(curGlowTime) * glowStrenght);

            timeElapsed += Time.deltaTime;
            yield return null;
        }


        yield return new WaitForSeconds(1);

        timeElapsed = Time.deltaTime;
        timeEnd = glowTime + Time.deltaTime;
        while (timeElapsed < timeEnd)
        {
            curGlowTime = timeElapsed / timeEnd;
            glowStrenght = Mathf.Lerp(glowStrenghtHigh, 0, timeElapsed / timeEnd);
            planetRenderer.material.SetColor("_EmissionColor", emissiveColor.Evaluate(curGlowTime) * glowStrenght);

            timeElapsed += Time.deltaTime;
            yield return null;
        }
        */
    }

    IEnumerator Explosion()
    {
        for (int i = 0; i < explosionAmount; i++)
        {
            Vector3 randomDir = new Vector3(Random.Range(-100.0f, 100.0f), Random.Range(10f, 50.0f), Random.Range(-100.0f, 100.0f));
            var explodingStone = Instantiate(explosionStone, transform.position, Quaternion.identity);
            float size = Random.Range(2, 6);
            explodingStone.transform.localScale = new Vector3(size, size, size);
            explodingStone.GetComponent<Rigidbody>().AddForce(randomDir, ForceMode.Impulse);

            yield return new WaitForSeconds(explosionTime);
        }
    }
}
