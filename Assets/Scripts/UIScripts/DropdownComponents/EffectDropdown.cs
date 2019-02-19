using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EffectDropdown : DropdownAbstract
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
            default:
                DeleteExtraDropdowns();
                break;
        }
    }
}
