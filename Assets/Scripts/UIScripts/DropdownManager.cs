using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class DropdownManager : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    private TMP_Dropdown effectDropdown;
    private DropdownOptions dropdownOptions;
    public Dictionary<string, DropdownOptions> dropdownOptionsDictionary;
    public UIManager UIManager;
    public GameObject dropdownTemplate;
    public GameObject slotTemplate;
    public bool extraNumeralDropdownNeeded = false;
    public bool extraNumeralDropdownExists = false;
    public UnityEvent effectDropdownOnValueChange;
    public UnityEvent anyDropdownOnValueChange;

    private int indexOfLastChild;

    public enum TypesOfDropdown
    {
        DEFAULT,
        KEYWORD,
        CONDITIONAL,
        EFFECT,
        TARGET,
        TARGET_TYPE,
        NUMERICAL
    }

    private TypesOfDropdown nextTypeOfDropdown;
    public TypesOfDropdown originalTypeOfDropdown;
    //public string defaultDropdownType;

    // Start is called before the first frame update
    void Start()
    {
        anyDropdownOnValueChange.AddListener(delegate
        {
            Debug.Log("Invoked.");
        });
        FindOrAddLastDropdown();
    }

    public void FindOrAddLastDropdown()
    {
        AssignUIManager();
        AssignDropdownOptionsDictionary();

        //indexOfLastChild = this.transform.childCount - 1;
        indexOfLastChild = -1;

        if (indexOfLastChild >= 0)
        {
            indexOfLastChild = 0;
            FillDropdown(indexOfLastChild);
        }
        else
        {
            indexOfLastChild = 0;

            if (dropdownTemplate)
            {
                AddAndFillDropdown(indexOfLastChild);
            }
            else
            {
                Debug.Log("typeOfDropdownToAdd not assigned");
            }
        }
    }

    private void AddDropdown()
    {
        GameObject newDropdown = Instantiate(dropdownTemplate);

        newDropdown.transform.SetParent(this.transform);
    }

    private void AssignDropdown(int childIndex)
    {
        if (childIndex >= this.transform.childCount)
        {
            childIndex = this.transform.childCount - 1;
        }
        dropdown = this.transform.GetChild(childIndex).GetComponent<TMP_Dropdown>();
    }

    private void AddOptionsToDropdown()
    {
        dropdown.AddOptions(dropdownOptions.options);
    }

    private void FillDropdown(int childIndex)
    {
        AssignDropdown(childIndex);
        AssignNextTypeOfDropdown();
        AssignDropdownOptions();
        AddOptionsToDropdown();
        
        if (nextTypeOfDropdown != TypesOfDropdown.DEFAULT)
        {
            AddListenerToDropdown();
        }
        else
        {
            Destroy(dropdown.gameObject);
            effectDropdownOnValueChange.Invoke();
        }
    }

    private void FillDropdown(int childIndex, bool externalCall = false)
    {
        AssignDropdown(this.transform.childCount - 1);
        if (externalCall)
        {
            nextTypeOfDropdown = TypesOfDropdown.NUMERICAL;
        }
        else
        {
            AssignNextTypeOfDropdown();
        }
        AssignDropdownOptions();
        AddOptionsToDropdown();

        if (nextTypeOfDropdown != TypesOfDropdown.DEFAULT)
        {
            AddListenerToDropdown();
        }
        else
        {
            Destroy(dropdown.gameObject);
        }
    }

    public void AddAndFillDropdown(int childIndex)
    {
        if (dropdown)
        {
            dropdown.onValueChanged.RemoveAllListeners();
            AddAnyDropdownOnValueChangeListener();

            if (nextTypeOfDropdown == TypesOfDropdown.EFFECT)
            {
                AddListenerToEffectDropdown();
            }
        }
        AddDropdown();
        FillDropdown(childIndex);
    }

    public void AddAndFillDropdown(int childIndex, bool externalCall = false)
    {
        if (!externalCall)
        {
            AddAndFillDropdown(childIndex);
        }
        else
        {
            if (dropdown)
            {
                dropdown.onValueChanged.RemoveAllListeners();
                AddAnyDropdownOnValueChangeListener();
            }
            AddDropdown();
            FillDropdown(childIndex, externalCall);
        }
        
    }

    private void AddListenerToDropdown()
    {
        dropdown.onValueChanged.AddListener(delegate
        {
            OnDropdownValueChanged();
        });
        AddAnyDropdownOnValueChangeListener();
    }

    private void AddListenerToEffectDropdown()
    {
        int effectDropdownSiblingIndex = dropdown.transform.GetSiblingIndex();
        effectDropdown = this.transform.GetChild(effectDropdownSiblingIndex).GetComponent<TMP_Dropdown>();
        string currentEffect = effectDropdown.options[effectDropdown.value].text;

        DetermineIfExtraNumeralDropdownNeeded(currentEffect);

        dropdown.onValueChanged.AddListener(delegate
        {
            string currentEffectDuplicate = effectDropdown.options[effectDropdown.value].text;

            DetermineIfExtraNumeralDropdownNeeded(currentEffectDuplicate);

            effectDropdownOnValueChange.Invoke();
        });
    }

    private void DetermineIfExtraNumeralDropdownNeeded(string currentEffect)
    {
        switch (currentEffect.ToLower())
        {
            case "deal damage":
                extraNumeralDropdownNeeded = true;
                break;
            case "draw card":
                extraNumeralDropdownNeeded = true;
                break;
            default:
                extraNumeralDropdownNeeded = false;
                break;
        }
    }

    private void AddAnyDropdownOnValueChangeListener()
    {
        dropdown.onValueChanged.AddListener(delegate
        {
            anyDropdownOnValueChange.Invoke();
        });
    }

    private void OnDropdownValueChanged()
    {
        indexOfLastChild++;
        AddAndFillDropdown(indexOfLastChild);
    }

    private void AssignDropdownOptions()
    {
        switch (nextTypeOfDropdown)
        {
            case TypesOfDropdown.KEYWORD:
                dropdownOptions = dropdownOptionsDictionary["keyword"];
                break;
            case TypesOfDropdown.CONDITIONAL:
                dropdownOptions = dropdownOptionsDictionary["conditional"];
                break;
            case TypesOfDropdown.EFFECT:
                dropdownOptions = dropdownOptionsDictionary["effect"];
                break;
            case TypesOfDropdown.TARGET:
                dropdownOptions = dropdownOptionsDictionary["target"];
                break;
            case TypesOfDropdown.TARGET_TYPE:
                dropdownOptions = dropdownOptionsDictionary["target_type"];
                break;
            case TypesOfDropdown.NUMERICAL:
                dropdownOptions = dropdownOptionsDictionary["numerical"];
                break;
            default:
                Debug.Log("typesOfDropdown is invalid or default");
                break;
        }
    }

    private void AssignUIManager()
    {
        if (UIManager == null)
        {
            UIManager = FindObjectOfType<UIManager>();
        }
    }

    private void AssignDropdownOptionsDictionary()
    {
        if (dropdownOptionsDictionary == null)
        {
            FillDropdownOptionsDictionary();
        }
    }

    private void FillDropdownOptionsDictionary()
    {
        List<DropdownOptions> dropdownOptionsList = UIManager.dropdownOptions;
        if (dropdownOptionsDictionary == null)
        {
            dropdownOptionsDictionary = new Dictionary<string, DropdownOptions>();
        }
        
        dropdownOptionsDictionary.Add("keyword", dropdownOptionsList[0]);
        dropdownOptionsDictionary.Add("conditional", dropdownOptionsList[1]);
        dropdownOptionsDictionary.Add("effect", dropdownOptionsList[2]);
        dropdownOptionsDictionary.Add("target", dropdownOptionsList[3]);
        dropdownOptionsDictionary.Add("target_type", dropdownOptionsList[4]);
        dropdownOptionsDictionary.Add("numerical", dropdownOptionsList[5]);
    }

    private void AssignNextTypeOfDropdown()
    {
        switch (originalTypeOfDropdown)
        {
            case TypesOfDropdown.KEYWORD:
                nextTypeOfDropdown = TypesOfDropdown.KEYWORD;
                break;
            case TypesOfDropdown.CONDITIONAL:
                AssignNextTypeOfDropdownConditional();
                break;
            case TypesOfDropdown.EFFECT:
                break;
            case TypesOfDropdown.TARGET:
                break;
            case TypesOfDropdown.TARGET_TYPE:
                break;
            case TypesOfDropdown.NUMERICAL:
                nextTypeOfDropdown = TypesOfDropdown.NUMERICAL;
                break;
            default:
                Debug.Log("originalTypeOfDropdown is set to default or is otherwise invalid");
                break;
        }
    }

    private void AssignNextTypeOfDropdown(bool externalCall = false)
    {
        if (externalCall)
        {
            switch (originalTypeOfDropdown)
            {
                case TypesOfDropdown.KEYWORD:
                    nextTypeOfDropdown = TypesOfDropdown.KEYWORD;
                    break;
                case TypesOfDropdown.CONDITIONAL:
                    AssignNextTypeOfDropdownConditional();
                    break;
                case TypesOfDropdown.EFFECT:
                    break;
                case TypesOfDropdown.TARGET:
                    break;
                case TypesOfDropdown.TARGET_TYPE:
                    break;
                case TypesOfDropdown.NUMERICAL:
                    nextTypeOfDropdown = TypesOfDropdown.NUMERICAL;
                    break;
                default:
                    Debug.Log("originalTypeOfDropdown is set to default or is otherwise invalid");
                    break;
            }
        }
        else
        {
            Debug.Log("The wrong form of AssignNextTypeOfDropdown was called.");
        }
    }

    private void AssignNextTypeOfDropdownConditional()
    {
        if (indexOfLastChild <= 0)
        {
            nextTypeOfDropdown = TypesOfDropdown.CONDITIONAL;
        }
        else
        {
            switch (nextTypeOfDropdown)
            {
                case TypesOfDropdown.KEYWORD:
                    nextTypeOfDropdown = TypesOfDropdown.DEFAULT;
                    break;
                case TypesOfDropdown.CONDITIONAL:
                    nextTypeOfDropdown = TypesOfDropdown.EFFECT;
                    break;
                case TypesOfDropdown.EFFECT:
                    nextTypeOfDropdown = TypesOfDropdown.NUMERICAL;
                    break;
                case TypesOfDropdown.TARGET:
                    nextTypeOfDropdown = TypesOfDropdown.TARGET_TYPE;
                    break;
                case TypesOfDropdown.TARGET_TYPE:
                    nextTypeOfDropdown = TypesOfDropdown.DEFAULT;
                    AddExtraNumeralDropdown();
                    break;
                case TypesOfDropdown.NUMERICAL:
                    nextTypeOfDropdown = TypesOfDropdown.TARGET;
                    break;
                default:
                    nextTypeOfDropdown = TypesOfDropdown.CONDITIONAL;
                    break;
            }
        }
    }

    private void AddExtraNumeralDropdown()
    {
        effectDropdownOnValueChange.AddListener(delegate
        {
            if (extraNumeralDropdownNeeded)
            {
                AddExtraDropdown();
            }
            else
            {
                Debug.Log("Removing extra dropdown");
                if (this.transform.childCount > 5)
                {
                    Destroy(this.transform.GetChild(this.transform.childCount - 1).gameObject);
                    extraNumeralDropdownExists = false;
                }
                else
                {
                    Debug.Log("Extra dropdown does not exist");
                }
            }
        });

        Debug.Log("Last Dropdown listener has been added.");
        //GameObject newSlot = Instantiate(slotTemplate, dropdown.transform.parent.parent);

        //newSlot.transform.SetSiblingIndex(slotTemplate.transform.GetSiblingIndex() + 1);
    }

    private void AddExtraDropdownIfNeeded()
    {
        effectDropdown.onValueChanged.Invoke(0);
    }

    private void AddExtraDropdown()
    {
        if (this.transform.childCount <= 6 && !extraNumeralDropdownExists)
        {
            Debug.Log("Adding extra dropdown");
            AddAndFillDropdown(indexOfLastChild, true);
            dropdown.onValueChanged.RemoveAllListeners();
            AddAnyDropdownOnValueChangeListener();
            extraNumeralDropdownExists = true;
        }
        else
        {
            Debug.Log("Attempted to add an extra dropdown and found that one was not needed.");
        }
    }

    //private void DestroyAllChildren(Transform parentObject)
    //{
    //    for (int i = 0; i < parentObject.childCount; i++)
    //    {
    //        Destroy(parentObject.GetChild(i).gameObject);
    //    }
    //}
}
