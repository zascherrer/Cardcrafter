using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayCard : MonoBehaviour
{
    private AttributeMenu attributeMenu;
    private Transform attributeMenuViewportTransform;
    private Transform keywordSlot;
    public Transform effectSlot;
    private Transform powerAndDurabilitySlot;

    private TMP_Text description;
    private TMP_Text cardType;
    private TMP_Text power;
    private TMP_Text durability;
    private TMP_Text mana;
    private TMP_Text cardTitle;

    private TMP_Dropdown cardTypeDropdown;
    private TMP_InputField cardTitleInput;

    private List<TMP_Dropdown> keywordDropdowns = new List<TMP_Dropdown>();
    public List<TMP_Dropdown> effectDropdowns = new List<TMP_Dropdown>();
    private List<TMP_Dropdown> powerAndDurabilityDropdowns = new List<TMP_Dropdown>();

    private int cardCost;
    private List<string> keywordStrings = new List<string>();
    private List<string> effectStrings = new List<string>();

    private DropdownManager effectSlotManager;

    // Start is called before the first frame update
    void Start()
    {
        AssignAttributeMenu();
        AssignDisplayCardAttributes();
        AssignListenersToPowerAndDurabilityDropdowns();
    }

    void AssignListenersToPowerAndDurabilityDropdowns()
    {
        for (int i = 0; i < powerAndDurabilityDropdowns.Count; i++)
        {
            powerAndDurabilityDropdowns[i].onValueChanged.AddListener(delegate
            {
                OnDropdownsChanged();
            });
        }
    }

    public void OnDropdownsChanged()
    {
        AssignAttributeMenu();
        TransmitDataFromAttributeMenu();
    }

    private void AssignAttributeMenu()
    {
        attributeMenu = FindObjectOfType<AttributeMenu>();

        if (attributeMenu)
        {
            AssignAttributeMenuAttributes();
        }
        else
        {
            Debug.Log("AttributeMenu not found.");
        }
    }

    private void AssignAttributeMenuAttributes()
    {
        attributeMenuViewportTransform = attributeMenu.GetComponentInChildren<AttributeMenuPanel>().transform.GetChild(0).GetChild(0);
        keywordSlot = attributeMenuViewportTransform.GetComponentInChildren<KeywordSlot>().transform;
        effectSlot = attributeMenuViewportTransform.GetComponentInChildren<EffectSlot>().transform;
        powerAndDurabilitySlot = attributeMenuViewportTransform.GetComponentInChildren<PowerAndDurabilitySlot>().transform;

        AssignCardTypeDropdown();
        AssignDropdowns(keywordDropdowns, keywordSlot);
        AssignDropdowns(effectDropdowns, effectSlot);
        AssignDropdowns(powerAndDurabilityDropdowns, powerAndDurabilitySlot);
        AssignCardTitleInput();
        AssignEffectSlotManager(effectSlot);
    }

    private void AssignEffectSlotManager(Transform effectSlot)
    {
        effectSlotManager = effectSlot.GetComponent<DropdownManager>();
    }

    private void AssignCardTypeDropdown()
    {
        cardTypeDropdown = attributeMenu.GetComponentInChildren<CardTypeDropdown>().transform.GetComponentInChildren<TMP_Dropdown>();

        if (cardTypeDropdown)
        {
            //Debug.Log("cardTypeDropdown found!");
        }
        else
        {
            Debug.Log("cardTypeDropdown not found.");
        }
    }

    private void AssignCardTitleInput()
    {
        cardTitleInput = attributeMenu.GetComponentInChildren<CardTitleInput>().gameObject.GetComponent<TMP_InputField>();

        if (cardTitleInput)
        {
            //Debug.Log("cardTitleInput found!");
        }
        else
        {
            Debug.Log("cardTitleInput not found.");
        }
    }

    public void AssignDropdowns(List<TMP_Dropdown> dropdowns, Transform parentSlot)
    {
        dropdowns.Clear();

        for (int i = 0; i < parentSlot.childCount; i++)
        {
            if (parentSlot.GetChild(i).GetComponent<TMP_Dropdown>())
            {
                dropdowns.Add(parentSlot.GetChild(i).GetComponent<TMP_Dropdown>());
            }
        }
    }

    private void AssignDisplayCardAttributes()
    {
        AssignDescription();
        AssignCardType();
        AssignPower();
        AssignDurability();
        AssignCardTitle();
        AssignMana();
    }

    private void AssignDescription()
    {
        description = this.transform.GetComponentInChildren<DescriptionBox>().transform.GetComponentInChildren<TMP_Text>();

        if (description)
        {
            ClearText(description);
        }
        else
        {
            Debug.Log("Description not found.");
        }
    }

    private void AssignCardType()
    {
        cardType = this.transform.GetComponentInChildren<CardTypePanel>().transform.GetComponentInChildren<TMP_Text>();

        if (cardType)
        {
            ClearText(cardType);
        }
        else
        {
            Debug.Log("CardType not found.");
        }
    }

    private void AssignCardTitle()
    {
        cardTitle = this.transform.GetComponentInChildren<CardTitle>().transform.GetComponent<TMP_Text>();

        if (cardTitle)
        {
            cardTitle.text = "Enter text here...";
            AddCardTitleListener();
        }
        else
        {
            Debug.Log("CardTitle not found.");
        }
    }

    private void AssignMana()
    {
        mana = this.transform.GetComponentInChildren<ManaOrb>().transform.GetComponentInChildren<TMP_Text>();

        if (mana)
        {
            ClearText(mana);
        }
        else
        {
            Debug.Log("Mana Orb not found.");
        }
    }

    private void ClearText(TMP_Text text)
    {
        text.text = "";
    }

    private void TransmitDataFromAttributeMenu()
    {
        //Debug.Log(effectSlot.childCount);

        AssignTextFromDropdown(cardType, cardTypeDropdown);
        UpdateDescription();
        UpdatePowerAndDurability();
        AssignCardCost();
    }

    private void AssignTextFromDropdown(TMP_Text text, TMP_Dropdown dropdown)
    {
        text.text = dropdown.options[dropdown.value].text;
    }

    private void UpdateDescription()
    {
        description.text = "";

        TransmitTextFromDropdownsToDescription(keywordDropdowns, keywordSlot);
        TransmitTextFromDropdownsToDescription(effectDropdowns, effectSlot, true);
    }

    private void TransmitTextFromDropdownsToDescription(List<TMP_Dropdown> dropdowns, Transform slot, bool isEffect = false)
    {
        if (slot.childCount > dropdowns.Count)
        {
            AssignDropdowns(dropdowns, slot);
        }

        if (dropdowns.Count > 0)
        {
            string textToAdd = ConcatenateTextFromDropdownList(dropdowns, isEffect);
            AddTextToDescription(textToAdd);
        }
        else
        {
            Debug.Log("keywordDropdowns is empty.");
        }
    }

    private string ConcatenateTextFromDropdownList(List<TMP_Dropdown> dropdowns)
    {
        List<string> words = new List<string>();
        string result = " ";

        for (int i = 0; i < dropdowns.Count; i++)
        {
            string wordToAdd = dropdowns[i].options[dropdowns[i].value].text;
            words.Add(wordToAdd);
        }

        keywordStrings = words;

        for (int i = 0; i < dropdowns.Count; i++)
        {
            if (i == dropdowns.Count - 1 || i == dropdowns.Count - 2)
            {
                result += dropdowns[i].options[dropdowns[i].value].text + " ";
            }
            else
            {
                result += dropdowns[i].options[dropdowns[i].value].text + ", ";
            }
        }

        return result;
    }

    private string ConcatenateTextFromDropdownList(List<TMP_Dropdown> dropdowns, bool isEffect)
    {
        if (!isEffect)
        {
            return ConcatenateTextFromDropdownList(dropdowns);
        }
        else
        {
            List<string> words = new List<string>();
            List<string> wordsTranslated = new List<string>();
            string result = " ";

            for (int i = 0; i < dropdowns.Count; i++)
            {
                string wordToAdd = dropdowns[i].options[dropdowns[i].value].text;
                Debug.Log(wordToAdd);
                words.Add(wordToAdd);
            }

            effectStrings = words;
            wordsTranslated = EffectTextTranslator.TranslateEffectText(words);

            for (int i = 0; i < wordsTranslated.Count; i++)
            {
                result += wordsTranslated[i] + " ";
            }
            
            return result;
        }
    }

    private void AddTextToDescription(string textToAdd)
    {
        if (description.text == "" || description.text == " ")
        {
            description.text = textToAdd;
        }
        else
        {
            description.text += "\n\n";
            description.text += textToAdd;
        }
    }

    private void AssignPower()
    {
        power = this.transform.GetComponentInChildren<PowerOrb>().transform.GetComponentInChildren<TMP_Text>();

        if (power)
        {
            ClearText(power);
        }
        else
        {
            Debug.Log("Power text not found.");
        }
    }

    private void AssignDurability()
    {
        durability = this.transform.GetComponentInChildren<DurabilityOrb>().transform.GetComponentInChildren<TMP_Text>();

        if (durability)
        {
            ClearText(durability);
        }
        else
        {
            Debug.Log("Durability text not found.");
        }
    }

    private void UpdatePowerAndDurability()
    {
        if (power)
        {
            AssignTextFromDropdown(power, powerAndDurabilityDropdowns[0]);
            AssignTextFromDropdown(durability, powerAndDurabilityDropdowns[1]);
        }
        else if (durability)
        {
            AssignTextFromDropdown(durability, powerAndDurabilityDropdowns[0]);
        }
        else
        {
            Debug.Log("Power and durability are not assigned.");
        }
    }

    private void AddCardTitleListener()
    {
        cardTitleInput.onSelect.AddListener(delegate
        {
            string originalTitle = cardTitle.text;

            cardTitle.text = "";
            cardTitleInput.text = originalTitle;
        });

        cardTitleInput.onDeselect.AddListener(delegate
        {
            cardTitle.text = cardTitleInput.textComponent.text;
            cardTitleInput.text = "";
        });
    }

    private void AssignCardCost()
    {
        List<string> combinedKeywords = CombineLists(keywordStrings, effectStrings);
        cardCost = 0;

        if (combinedKeywords.Count > 1)
        {
            Debug.Log(combinedKeywords.Count);
        }

        if(powerAndDurabilityDropdowns.Count == 1)
        {
            int.TryParse(powerAndDurabilityDropdowns[0].options[powerAndDurabilityDropdowns[0].value].text, out int durability);
            cardCost = CardCostCalculator.CalculateCost(combinedKeywords, cardType.text, 0, durability);
        }
        else if (powerAndDurabilityDropdowns.Count == 2)
        {
            int.TryParse(powerAndDurabilityDropdowns[0].options[powerAndDurabilityDropdowns[0].value].text, out int power);
            int.TryParse(powerAndDurabilityDropdowns[1].options[powerAndDurabilityDropdowns[1].value].text, out int durability);

            cardCost = CardCostCalculator.CalculateCost(combinedKeywords, cardType.text, power, durability);
        }
        else
        {
            Debug.Log("Something's wrong with the power and durability dropdowns list.");

            cardCost = CardCostCalculator.CalculateCost(combinedKeywords, cardType.text, 100, 100);
        }

        mana.text = cardCost.ToString();
    }

    private List<string> CombineLists(List<string> list1, List<string> list2)
    {
        List<string> result = new List<string>();
        result = list1;

        for (int i = 0; i < list2.Count; i++)
        {
            result.Add(list2[i]);
        }

        return result;
    }

    private void AddExtraNumeralDropdown()
    {
        effectSlotManager.AddAndFillDropdown(effectSlotManager.transform.childCount - 1, true);
    }
}
