using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTypeDropdown : DropdownAbstract
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

        EndDropdownCycle();
    }

    private void EndDropdownCycle()
    {
        AddNextDropdownAndComponent<ConditionalDropdown>(false, DropdownManager.TypesOfDropdown.DEFAULT);
    }
}
