using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class DropdownAbstract : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public DropdownManager dropdownManager;
    public int numberOfExtraDropdownsAdded = 0;
    public bool nextComponentAdded = false;

    public void OnStart()
    {
        if (AssignDropdownManager())
        {
            if (AssignDropdown())
            {
                AddValueListener();
            }
        }
    }

    public bool AssignDropdownManager()
    {
        dropdownManager = this.GetComponent<DropdownManager>();

        if (dropdownManager)
        {
            Debug.Log("Dropdown Manager found");
            return true;
        }
        else
        {
            Debug.Log("Dropdown Manager is missing");
            return false;
        }
    }

    public bool AssignDropdown()
    {
        dropdown = dropdownManager.dropdown;

        if (dropdown)
        {
            Debug.Log("Dropdown has been found.");
            return true;
        }
        else
        {
            Debug.Log("Dropdown is missing");
            return false;
        }
    }

    public void AddValueListener()
    {
        dropdown.onValueChanged.AddListener(delegate
        {
            CheckValueOfDropdown();
        });
    }

    public virtual void CheckValueOfDropdown()
    {
        string currentValue = dropdown.options[dropdown.value].text;

        switch (currentValue.ToLower())
        {
            default:
                DeleteExtraDropdowns();
                break;
        }
    }

    public void AddExtraDropdown(int numberAfterOriginalDropdown, DropdownManager.TypesOfDropdown nextTypeOfDropdown)
    {
        int childIndex = dropdown.transform.GetSiblingIndex() + numberAfterOriginalDropdown;

        dropdownManager.AddDropdown(childIndex);
        dropdownManager.FillDropdown(childIndex, false, true, nextTypeOfDropdown);
        numberOfExtraDropdownsAdded++;
    }

    public void AddNextDropdownAndComponent<T>(bool addAnotherComponent, DropdownManager.TypesOfDropdown typeOfDropdown) where T : DropdownAbstract
    {
        if (addAnotherComponent && !nextComponentAdded)
        {
            int childIndex = this.transform.childCount;

            dropdownManager.AddDropdown();
            dropdownManager.FillDropdown(childIndex, false, true, typeOfDropdown);
            this.gameObject.AddComponent<T>();

            nextComponentAdded = true;
        }
        else if (addAnotherComponent && nextComponentAdded)
        {
            Debug.Log("Next dropdown component already added.");
        }
        else
        {
            Debug.Log("End of the dropdown chain.");
        }
    }

    public void DeleteExtraDropdowns()
    {
        int siblingIndex = dropdown.transform.GetSiblingIndex();

        for (int i = 1; i <= numberOfExtraDropdownsAdded; i++)
        {
            Destroy(this.transform.GetChild(siblingIndex + i).gameObject);
        }

        numberOfExtraDropdownsAdded = 0;
    }
}
