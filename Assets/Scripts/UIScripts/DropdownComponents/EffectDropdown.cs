using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EffectDropdown : DropdownAbstract
{
    void Start()
    {
        OnStart();
    }

    public override void CheckValueOfDropdown()
    {
        string currentValue = dropdown.options[dropdown.value].text;

        switch (currentValue.ToLower())
        {
            case "deal damage":
                AddNumeralDropdown();
                break;
            case "draw card":
                AddNumeralDropdown();
                break;
            default:
                DeleteExtraDropdowns();
                break;
        }

        AddNumeralDropdownIfNecessary();
    }

    private void AddNumeralDropdown()
    {
        if (numberOfExtraDropdownsAdded == 0)
        {
            AddExtraDropdown(1, DropdownManager.TypesOfDropdown.NUMERICAL);
        }
        else
        {
            Debug.Log("Something went wrong in AddNumeralDropdown()");
        }
    }

    private void AddNumeralDropdownIfNecessary()
    {
        AddNextDropdownAndComponent<NumeralDropdown>(true, DropdownManager.TypesOfDropdown.NUMERICAL);
    }
}
