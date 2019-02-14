using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public List<DropdownOptions> dropdownOptions;
    public DropdownManager[] dropdownManagers;
    public DisplayCard displayCard;

    // Start is called before the first frame update
    void Start()
    {
        dropdownManagers = FindObjectsOfType<DropdownManager>();
        displayCard = FindObjectOfType<DisplayCard>();

        AddListenersToDropdownManagers();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AddListenersToDropdownManagers()
    {
        for (int i = 0; i < dropdownManagers.Length; i++)
        {
            dropdownManagers[i].anyDropdownOnValueChange.AddListener(delegate
            {
                displayCard.OnDropdownsChanged();
            });
        }
    }
}
