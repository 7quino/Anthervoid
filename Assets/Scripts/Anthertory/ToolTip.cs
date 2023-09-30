using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;




public class ToolTip : MonoBehaviour
{
    public XRRayInteractor rightRay;
    public TextMeshProUGUI tooltipText;
    public RectTransform tooltipBackground;
    public RectTransform tooltipLayoutGroup;


    public RectTransform tooltipOffset;
    Vector3 rayPos;

    public TooltipMessage[] tooltipMesages;
    public GameObject tooltipObject;


    private void Start()
    {
        tooltipText = GetComponentInChildren<TextMeshProUGUI>();
        gameObject.SetActive(false);

        foreach (TooltipMessage tooltipMessage in tooltipMesages) AddEventPointers(tooltipMessage);
    }

    private void LateUpdate()
    {
        bool isRightRayHovering = rightRay.TryGetHitInfo(out Vector3 rightPos, out Vector3 rightNormal, out int rightNumber, out bool rightValid);
        rayPos = rightPos;

        if (isRightRayHovering)
        {
            transform.position = new Vector3( rayPos.x, rayPos.y, tooltipOffset.transform.position.z - 0.02f);
        }


        if (tooltipBackground.sizeDelta.y != tooltipLayoutGroup.sizeDelta.y)
        {
            float x = tooltipBackground.sizeDelta.x;
            float y = tooltipLayoutGroup.sizeDelta.y;
            tooltipBackground.sizeDelta = new Vector2(x, y);
        }
    }


    public void OnToggleClick(Toggle toggle)
    {
        tooltipObject.SetActive(toggle);
    }


    void AddEventPointers(TooltipMessage tooltipMessage)
    {
        for (int i = 0; i < tooltipMessage.uiObjects.Length; i++)
        {
            if (tooltipMessage.uiObjects[i].GetComponent<EventTrigger>() == null)
            {
                tooltipMessage.uiObjects[i].AddComponent<EventTrigger>();
            }
            
            EventTrigger trigger = tooltipMessage.uiObjects[i].GetComponent<EventTrigger>();
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryExit.eventID = EventTriggerType.PointerExit;

            entryEnter.callback.AddListener((function) => HoverEnter(tooltipMessage));
            entryExit.callback.AddListener((function) => HoverExit());
            trigger.triggers.Add(entryEnter);
            trigger.triggers.Add(entryExit);
        }
    }

    /*
    void RemoveEventPointers(TooltipMessage tooltipMessage)
    {
        for (int i = 0; i < tooltipMessage.uiObjects.Length; i++)
        {
            EventTrigger trigger = tooltipMessage.uiObjects[i].GetComponent<EventTrigger>();

            if (trigger != null) trigger.triggers.Clear();
        }
    }
    */

    void HoverEnter(TooltipMessage tooltipMessage)
    {
        if (!this.gameObject.activeSelf) return;

        tooltipObject.SetActive(true);
        tooltipText.text = tooltipMessage.message;
    }

    void HoverExit()
    {
        if (!this.gameObject.activeSelf) return;

        tooltipObject.SetActive(false);
        tooltipText.text = "";
    }
}


[System.Serializable]
public class TooltipMessage
{
    public string uiElement;
    public GameObject[] uiObjects;
    public string message;
}


