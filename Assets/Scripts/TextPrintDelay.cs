using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextPrintDelay : MonoBehaviour
{
    public float delayTime = 0;
    public float printTime;
    public bool printNow = false;

    TextMeshProUGUI textObject;
    string textMessage;
    string textToWright;


    private void Awake()
    {
        textObject = GetComponent<TextMeshProUGUI>();
        textMessage = textObject.text;
        printTime = textMessage.Length * delayTime;
        textObject.text = "";
    }

    private void Update()
    {
        if (printNow)
        {
            printNow = false;
            StartCoroutine(PrintTextDelay());
        }
    }

    public void PrintText()
    {
        StartCoroutine(PrintTextDelay());
    }

    IEnumerator PrintTextDelay()
    {
        for (int i = 0; i < textMessage.Length; i++)
        {
            textToWright += textMessage[i];
            textObject.text = textToWright;

            yield return new WaitForSeconds(delayTime);
        }
    }

    public void ClearText()
    {
        textToWright = "";
        textObject.text = "";
    }
}
