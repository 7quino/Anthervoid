using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionStone : MonoBehaviour
{
    [HideInInspector]
    public Renderer stoneRenderer;
    public Gradient emissiveColor;
    public float glowStrenght;
    public float lifetime;

    void Start()
    {
        stoneRenderer = GetComponent<Renderer>();
        StartCoroutine(GlowDown());
        Destroy(gameObject, lifetime);
    }

    IEnumerator GlowDown()
    {
        yield return new WaitForSeconds(1.5f);

        float timeElapsed = Time.deltaTime;
        float timeEnd = lifetime + Time.deltaTime;
        float curGlowTime;
        Vector3 size = transform.localScale;
        Vector3 curSize;

        while (timeElapsed < timeEnd)
        {
            curGlowTime = timeElapsed / timeEnd;
            curSize = Vector3.Lerp(size, new Vector3(0,0,0), timeElapsed / timeEnd);
            transform.localScale = curSize;

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        /*
        while (timeElapsed < timeEnd)
        {
            curGlowTime = timeElapsed / timeEnd;
            glowStrenght = Mathf.Lerp(glowStrenght, 0, timeElapsed / timeEnd);
            stoneRenderer.material.SetColor("_EmissionColor", emissiveColor.Evaluate(curGlowTime) * glowStrenght);

            timeElapsed += Time.deltaTime;
            yield return null;
        }
        */
    }
}
