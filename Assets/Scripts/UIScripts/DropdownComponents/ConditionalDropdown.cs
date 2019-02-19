using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConditionalDropdown : DropdownAbstract
{
    // Start is called before the first frame update
    void Start()
    {
        OnStart();
    }

    public override void CheckValueOfDropdown()
    {
        string currentValue = dropdown.options[dropdown.value].text;

        switch (currentValue.ToLower())
        {
            case "when...":
                WhenLogicTree();
                break;
            default:
                DeleteExtraDropdowns();
                break;
        }

        AddEffectDropdownIfNecessary();
    }

    private void WhenLogicTree()
    {
        if (numberOfExtraDropdownsAdded == 0)
        {
            AddExtraDropdown(1, DropdownManager.TypesOfDropdown.TARGET);
            AddExtraDropdown(2, DropdownManager.TypesOfDropdown.TARGET_TYPE);
            AddExtraDropdown(3, DropdownManager.TypesOfDropdown.EFFECT);
        }
        else
        {
            Debug.Log("Something went wrong in WhenLogicTree()");
        }
    }

    private void AddEffectDropdownIfNecessary()
    {
        AddNextDropdownAndComponent<EffectDropdown>(true, DropdownManager.TypesOfDropdown.EFFECT);
    }
}
