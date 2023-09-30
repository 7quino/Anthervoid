using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using System;


public class Anthertory : MonoBehaviour
{
    public AnthertorySlot[] anthertorySlots;
    public AntherSlot[] slots;

    public GameObject anthertory;
    public GameObject anthertoryCanvas;
    public GameObject mainMenu;
    public GameObject spliceMenu;
    public GameObject toolTip;

    public AntherSlot selectedAnther;
    private int selectedAntherIndex = 0;

    [Header("Selected Item")]
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedItemStatCollected;
    public TextMeshProUGUI selectedItemStatQuantity;
    public TextMeshProUGUI selectedItemGen;
    public AntherPropertyUI[] properties;
    public Image genMeter;
    public Image spliceMeter;
    public Image anthergyMeter;
    public Button spliceButton;
    public Button genButton;
    public GameObject dnaSplicedText;

    public float anthergy;

    //Input actions
    public InputActionProperty xButtonAction;
    public InputActionProperty yButtonAction;
    bool pressed = false;

    //Disable when anthertory is active
    public ActionBasedContinuousTurnProvider turnProvider;


    [Header("Events")]
    public UnityEvent onOpenInventory;
    public UnityEvent onCloseInventory;
    public UnityEvent onGrabbingAnther;
    public UnityEvent onThrowingAnther;

    //Singelton
    public static Anthertory instance;

    private void Awake()
    {
        instance = this;
    }


    private void Start()
    {
        mainMenu.SetActive(true);
        dnaSplicedText.SetActive(false);
        spliceMenu.SetActive(false);
        anthertory.SetActive(false);
        slots = new AntherSlot[anthertorySlots.Length];

        //initialize the slots
        for (int x = 0; x < slots.Length; x++)
        {
            slots[x] = new AntherSlot();
            anthertorySlots[x].index = x;
            slots[x].anther = Instantiate(anthertorySlots[x].startAntherData);
            slots[x].quantity = anthertorySlots[x].quantity;
            slots[x].anther.quantity = slots[x].quantity;

            anthertorySlots[x].SetNew(slots[x]);
        }

        SelectItem(0);
    }


    void Update()
    {
        if (xButtonAction.action.ReadValue<float>() > 0.1f && !pressed)
        {
            pressed = true;
            StartCoroutine(PressDelay());
            Toggle();
        }

        if (yButtonAction.action.ReadValue<float>() > 0.1f && !pressed)
        {
            pressed = true;
            StartCoroutine(PressDelay());
            ToogleUI();
        }

        spliceButton.interactable = spliceMeter.fillAmount == 1.0f ? true : false;
        genButton.interactable = genMeter.fillAmount == 1.0f ? true : false;
    }


    IEnumerator PressDelay()
    {
        yield return new WaitForSeconds(0.2f);
        pressed = false;
    }


    public void Toggle()
    {
        if (anthertory.activeInHierarchy)
        {
            turnProvider.enabled = true;
            anthertory.SetActive(false);
            mainMenu.SetActive(false);
            onCloseInventory.Invoke();
        }
        else
        {
            turnProvider.enabled = false;
            anthertory.SetActive(true);
            mainMenu.SetActive(true);
            if(anthertoryCanvas.activeSelf) onOpenInventory.Invoke();
        }
    }


    public void ToogleUI()
    {
        if (!anthertory.activeInHierarchy) return;

        if (anthertoryCanvas.activeInHierarchy)
        {
            anthertoryCanvas.SetActive(false);
            onCloseInventory.Invoke();
        }
        else
        {
            SelectItem(selectedAntherIndex);
            anthertoryCanvas.SetActive(true);
            onOpenInventory.Invoke();
        }
    }


    public bool IsOpen()
    {
        return anthertoryCanvas.activeInHierarchy;
    }


    public void AddItem(AntherData anther)
    {
        int index;

        if (anther.canStack)
        {
            index = GetTtemStack(anther);
            if (slots[index].quantity > 0 && index >= 0)
            {
                anthergy = UpdateMeterAdd(anthergyMeter, 0.05f);
                slots[index].quantity++;
                slots[index].anther.quantity++;
                slots[index].anther.collected++;
                anthertorySlots[index].SetQuantity(slots[index]);

                if (slots[index] == selectedAnther)
                {
                    selectedAnther.anther.genergy = UpdateMeterAdd(genMeter, 0.1f);
                    selectedItemStatCollected.text = selectedAnther.anther.collected.ToString();
                    selectedItemStatQuantity.text = selectedAnther.anther.quantity.ToString();
                }
                else
                {
                    AddEnergy(ref slots[index].anther.genergy, 0.1f);
                }
                return;
            }
        }

        index = GetEmptySlot();
        if (index >= 0)
        {
            anthergy = UpdateMeterAdd(anthergyMeter, 0.05f);
            slots[index].anther = anther;
            slots[index].quantity = 1;
            slots[index].anther.quantity = 1;
            slots[index].anther.collected = 1;
            anthertorySlots[index].SetNew(slots[index]);

            if (slots[index] == selectedAnther)
            {
                selectedAnther.anther.genergy = UpdateMeterAdd(genMeter, 0.1f);
                SelectItem(index);
            }
            else
            {
                AddEnergy(ref slots[index].anther.genergy, 0.1f);
            }
            return;
        }
    }


    void AddEnergy(ref float energyData, float amount)
    {
        energyData += amount;
        if (energyData > 1) energyData = 1;
    }


    private int GetTtemStack(AntherData anther)
    {
        //checking if there is a exixting slot  where the item can be stacked
        for (int index = 0; index < slots.Length; index++)
        {
            if (slots[index].anther.type == anther.type && slots[index].quantity < anther.maxStackAmount)
            {
                return index;
            }
        }
        return -1;
    }


    private int GetEmptySlot()
    {
        for (int index = 0; index < slots.Length; index++)
        {
            if (slots[index].quantity == 0)
            {
                //empty slot
                return index;
            }
        }
        return -1;
    }


    public void SelectItem(int index)
    {
        if (mainMenu.activeSelf && slots[index].quantity == 0)
        {
            mainMenu.SetActive(false);
            return;
        }

        mainMenu.SetActive(true);
        selectedAnther = slots[index];
        selectedAntherIndex = index;

        selectedItemName.text = selectedAnther.anther.type.ToString();
        selectedItemDescription.text = selectedAnther.anther.description;
        selectedItemStatCollected.text = selectedAnther.anther.collected.ToString();
        selectedItemStatQuantity.text = selectedAnther.anther.quantity.ToString();
        selectedItemGen.text = selectedAnther.anther.gen.ToString();
        anthergyMeter.fillAmount = anthergy;
        genMeter.fillAmount = selectedAnther.anther.genergy;
        spliceMeter.fillAmount = selectedAnther.anther.splicergy;

        for (int i = 0; i < properties.Length; i++)
        {
            properties[i].propertyType = selectedAnther.anther.properties[i].propertyType;
            properties[i].propertyText.text = selectedAnther.anther.properties[i].propertyType.ToString();

            float intervall = selectedAnther.anther.properties[i].maxValue - selectedAnther.anther.properties[i].minValue;
            float curValue = selectedAnther.anther.properties[i].value - selectedAnther.anther.properties[i].minValue;

            properties[i].PropertyMeter.fillAmount = curValue / intervall;
        }
    }


    public void OnPlusButton(Button button)
    {
        if (anthergy <= 0.0f) return;

        for (int i = 0; i < properties.Length; i++)
        {
            if (properties[i].plusbutton == button)
            {
                float fillAmount = UpdateMeterAdd(properties[i].PropertyMeter, 0.1f);
                UpdatePropertyValue(properties[i], fillAmount);

                selectedAnther.anther.splicergy = UpdateMeterAdd(spliceMeter, 0.1f);
                anthergy = UpdateMeterReduce(anthergyMeter, 0.05f);
                return;
            }
        }
    }


    public void OnMinusButton(Button button)
    {
        if (anthergy <= 0.0f) return;

        for (int i = 0; i < properties.Length; i++)
        {
            if (properties[i].minusButton == button)
            {
                float fillAmount = UpdateMeterReduce(properties[i].PropertyMeter, 0.1f);
                UpdatePropertyValue(properties[i], fillAmount);

                selectedAnther.anther.splicergy = UpdateMeterAdd(spliceMeter, 0.1f);
                anthergy = UpdateMeterReduce(anthergyMeter, 0.05f);
                return;
            }
        }
    }


    float UpdateMeterAdd(Image meter, float amount)
    {
        float fillAmount = meter.fillAmount;
        fillAmount += amount;
        if (fillAmount > 1)
            fillAmount = 1;
        meter.fillAmount = fillAmount;
        return fillAmount;
    }


    float UpdateMeterReduce(Image meter, float amount)
    {
        float fillAmount = meter.fillAmount;
        fillAmount -= amount;
        if (fillAmount < 0)
            fillAmount = 0;
        meter.fillAmount = fillAmount;
        return fillAmount;
    }


    void UpdatePropertyValue(AntherPropertyUI antherProperty, float amount)
    {
        foreach (PropertyData property in selectedAnther.anther.properties)
        {
            if (antherProperty.propertyType == property.propertyType)
            {
                float intervall = property.maxValue - property.minValue;
                float value = intervall * amount;
                property.value = value + property.minValue;

                return;
            }
        }
    }


    public void OnGenButton()
    {
        if (genMeter.fillAmount < 1.0f)
            return;

        selectedAnther.anther.gen += 1;
        selectedItemGen.text = selectedAnther.anther.gen.ToString();
        selectedAnther.anther.genergy = 0;
        genMeter.fillAmount = 0.0f;
    }


    public void OnSpliceButton()
    {
        //if (spliceMeter.fillAmount < 1.0f) return;

        mainMenu.SetActive(false);
        spliceMenu.SetActive(true);
    }

    public void RemoveGrabbedAnther(int index)
    {
        onGrabbingAnther.Invoke();
        if (slots[index].quantity == 0) return;

        slots[index].quantity--;
        slots[index].anther.quantity--;

        if (slots[index].quantity == 0)
        {
            anthertorySlots[index].Clear();
            if (slots[index] == selectedAnther) mainMenu.SetActive(false);
        }

        anthertorySlots[index].SetQuantity(slots[index]);
        if (slots[index] == selectedAnther) selectedItemStatQuantity.text = slots[index].quantity.ToString();
    }

    public void OnTooltip(Toggle toggle)
    {
        if (toggle.isOn) toolTip.SetActive(true);
        else toolTip.SetActive(false);
    }


    public bool HasItems(AntherData anther, int quantity)
    {
        int amount = 0;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].anther == anther)
                amount += slots[i].quantity;

            if (amount >= quantity)
                return true;
        }

        return false;
    }


    public void ThrowingAnther()
    {
        onThrowingAnther.Invoke();
    }


    public void SplicePerformedMessage()
    {
        StartCoroutine(ShowSplicedMessage());
    }


    public IEnumerator ShowSplicedMessage()
    {
        dnaSplicedText.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        dnaSplicedText.SetActive(false);
        mainMenu.SetActive(true);
    }
}


//[System.Serializable]
public class AntherSlot
{
    public AntherData anther;
    public int quantity;
}


[System.Serializable]
public class AntherPropertyUI
{
    public PropertyType propertyType;
    public TextMeshProUGUI propertyText;
    public Image PropertyMeter;
    public Button plusbutton;
    public Button minusButton;
}
