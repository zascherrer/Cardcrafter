using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumeralDropdown : DropdownAbstract
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
            default:
                DeleteExtraDropdowns();
                break;
        }

        AddTargetDropdownIfNecessary();
    }

    private void AddTargetDropdownIfNecessary()
    {
        AddNextDropdownAndComponent<TargetDropdown>(true, DropdownManager.TypesOfDropdown.TARGET);
    }
}
