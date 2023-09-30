using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SpliceMenu : MonoBehaviour
{
    [Header("Components")]
    public GameObject mainMenu;
    public TextMeshProUGUI title;
    public Slider slider;
    public Toggle stemToogle;
    public Toggle rootToogle;
    public TMP_Dropdown dropdown;

    float sliderValue;

    List<AntherData> antherDataSlots = new List<AntherData>();
    List<string> options = new List<string>();

    AntherData dropdownAnther;
    AntherData curAnther;

    RuleData curAntherRuleData;
    RuleData otherAntherRuleData;

    
    private void Start()
    {
        Anthertory.instance.onCloseInventory.AddListener(OnDisable);
    }


    private void OnDisable()
    {
        this.gameObject.SetActive(false);
    }
    

    void OnEnable()
    {
        if (Anthertory.instance == null) return;

        curAnther = Anthertory.instance.selectedAnther.anther;
        title.text = curAnther.type.ToString();

        dropdown.ClearOptions();
        antherDataSlots.Clear();
        options.Clear();
        options.Add("Choose anther");

        foreach (AnthertorySlot slot in Anthertory.instance.anthertorySlots)
        {
            if (slot != null && slot.curSlot.anther.type != curAnther.type)
            {
                antherDataSlots.Add(slot.curSlot.anther);
                options.Add(slot.curSlot.anther.type.ToString());
            }
        }

        dropdown.AddOptions(options);
    }

    public void OnButtonGo()
    {
        foreach (AntherData antherSlot in antherDataSlots)
        {
            if (antherSlot.type.ToString() == dropdown.options[dropdown.value].text)
            {
                dropdownAnther = antherSlot;
                break;
            }
        }

        if (dropdownAnther == null) return;
        sliderValue = slider.value;
        if (stemToogle.isOn) SpliceRule(0, 2);
        if (rootToogle.isOn) SpliceRule(3, 5);

        Anthertory.instance.selectedAnther.anther.splicergy = 0;
        Anthertory.instance.spliceMeter.fillAmount = 0;

        Anthertory.instance.SplicePerformedMessage();
        this.gameObject.SetActive(false);
    }


    void SpliceRule(int ruleIndexStart, int ruleIndexStop)
    {
        int index;

        //Choose random rule
        RuleType randomRule = (RuleType)UnityEngine.Random.Range(ruleIndexStart, ruleIndexStop);

        //Rule current anther
        foreach (RuleData rule in curAnther.rules) if (randomRule == rule.ruleType) curAntherRuleData = rule;
        index = (int)((float)curAntherRuleData.rule.Length * sliderValue);
        string firstpart = curAntherRuleData.rule.Remove(index, curAntherRuleData.rule.Length - index);

        //Rule selected anther dropdown
        foreach (RuleData rule in dropdownAnther.rules) if (randomRule == rule.ruleType) otherAntherRuleData = rule;
        index = (int)((float)otherAntherRuleData.rule.Length * sliderValue);
        string lastpart = otherAntherRuleData.rule.Remove(0, index);
        
        curAntherRuleData.rule = firstpart + lastpart;
    }

 
    public void OnButtonCancel()
    {
        this.gameObject.SetActive(false);
        mainMenu.SetActive(true);
    }
}
