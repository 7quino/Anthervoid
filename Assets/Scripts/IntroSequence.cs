using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSequence : MonoBehaviour
{
    public List<TextPrintDelay> texts = new List<TextPrintDelay>();
    public float timeBetweenTexts = 0;

    public GameObject subSun;


    void Start()
    {
        subSun.SetActive(false);

        StartCoroutine(PrintIntroTexts());
    }

    IEnumerator PrintIntroTexts()
    {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < texts.Count; i++)
        {
            texts[i].PrintText();
            yield return new WaitForSeconds(texts[i].printTime);
            yield return new WaitForSeconds(timeBetweenTexts);
            texts[i].ClearText();
        }
    }

    void Update()
    {
        
    }
}
