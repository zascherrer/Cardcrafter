using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDropdown : DropdownAbstract
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

        AddTargetTypeDropdownIfNecessary();
    }

    private void AddTargetTypeDropdownIfNecessary()
    {
        AddNextDropdownAndComponent<TargetTypeDropdown>(true, DropdownManager.TypesOfDropdown.TARGET_TYPE);
    }
}
