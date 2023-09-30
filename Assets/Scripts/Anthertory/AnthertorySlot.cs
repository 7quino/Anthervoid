using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AnthertorySlot: MonoBehaviour
{
    public int index;

    private GameObject antherSlots;
    public GameObject antherHolder;
    public TextMeshProUGUI quantityText;

    [SerializeField]
    bool selected = false;
    public float minRotation;
    public float maxRotation;
    public float checkRate = 0.5f;
    float nextCheck = 0;

    [Header("Start data")]
    public AntherData startAntherData;
    public int quantity;
    public AntherSlot curSlot;
   
    private void Awake()
    {
        antherSlots = GameObject.Find("AntherSlots");
    }

    void Update()
    {
        if (Time.time >= nextCheck)
        {
            nextCheck = Time.time + checkRate;
            UpdateStatus();
        }
    }

    void UpdateStatus()
    {
        float rotation = antherSlots.transform.localEulerAngles.y;

        if (!selected && rotation > minRotation && rotation < maxRotation)
        {
            selected = true;
            StartCoroutine(ChangeScalePos(1, 2, 0, 0.04f));
            Anthertory.instance.SelectItem(index);
        }
        else if (selected && rotation < minRotation || selected && rotation > maxRotation)
        {
            selected = false;
            StartCoroutine(ChangeScalePos(2, 1, 0.04f, 0));
        }
    }

    public IEnumerator ChangeScalePos(float startSize, float endSize, float startPos, float endPos)
    {
        float timeElapsed = Time.deltaTime;
        float timeEnd = 0.75f + Time.deltaTime;

        while (timeElapsed < timeEnd)
        {
            antherHolder.transform.localScale = Vector3.Lerp(Vector3.one * startSize, Vector3.one * endSize, timeElapsed / timeEnd);
            antherHolder.transform.localPosition = Vector3.Lerp(new Vector3( 0, startPos, 0.06f), new Vector3( 0, endPos, 0.06f), timeElapsed / timeEnd);

            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void SetNew(AntherSlot slot)
    {
        curSlot = slot;
        antherSlots.SetActive(true);

        Transform antherHolderTrasform = antherHolder.transform;
        foreach (Transform child in antherHolderTrasform)
        {
            if (child.tag == "Anther") Destroy(child.gameObject);
        }

        GameObject dropAnther = Instantiate(slot.anther.antherPrefab, antherHolder.transform);
        dropAnther.GetComponent<PickMenuAnther>().anthertorySlot = this;
        dropAnther.GetComponent<PickMenuAnther>().antherHolder = antherHolder.transform;
        SetQuantity(slot);
    }

    public void SetQuantity(AntherSlot slot)
    {
        quantityText.text = slot.quantity > 0 ? slot.quantity.ToString() : string.Empty;
    }

    public void Clear()
    {
        Transform antherHolderTrasform = antherHolder.transform;
        foreach (Transform child in antherHolderTrasform)
        {
            if (child.tag == "Anther") child.gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
        }

        //antherSlots.SetActive(false);
    }
}
