using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Inspectable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject displayCard;

    private void Awake()
    {
        displayCard = FindObjectOfType<DisplayCard>().gameObject;
    }

    private void Start()
    {
        if (displayCard.activeInHierarchy)
        {
            displayCard.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (displayCard)
        {
            displayCard.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (displayCard)
        {
            displayCard.SetActive(false);
        }
    }
}
