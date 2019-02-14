using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 offset;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    public Transform parentToReturnTo;
    public Transform placeholderParent;
    private GameObject placeholder = null;
    public bool isDraggable = true;

    private void Start()
    {
        canvas = FindObjectOfType<Canvas>();

        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");

        if (isDraggable)
        {
            AddPlaceholder();

            offset = new Vector3(eventData.position.x, eventData.position.y) - this.transform.position;

            parentToReturnTo = this.transform.parent;
            this.transform.SetParent(canvas.transform);

            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDraggable)
        {
            this.transform.position = new Vector3(eventData.position.x, eventData.position.y) - offset;

            MovePlaceholder();
            if (placeholder.transform.parent != placeholderParent)
            {
                placeholder.transform.SetParent(placeholderParent);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("EndDrag");

        if (isDraggable)
        {
            this.transform.SetParent(parentToReturnTo);

            this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
            Destroy(placeholder);

            canvasGroup.blocksRaycasts = true;
        }
    }

    private void AddPlaceholder()
    {
        placeholder = new GameObject();
        placeholder.transform.SetParent(this.transform.parent);
        placeholderParent = this.transform.parent;
        LayoutElement layoutElement = placeholder.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = this.GetComponent<LayoutElement>().preferredHeight;
        layoutElement.preferredWidth = this.GetComponent<LayoutElement>().preferredWidth;
        placeholder.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        placeholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex());
    }

    private void MovePlaceholder()
    {
        int newSiblingIndex = placeholderParent.childCount;

        for (int i = 0; i < placeholderParent.childCount; i++)
        {
            if (placeholderParent.GetChild(i).position.x > this.transform.position.x)
            {
                newSiblingIndex = i;

                if (placeholder.transform.GetSiblingIndex() < newSiblingIndex)
                {
                    newSiblingIndex--;
                }

                break;
            }
        }

        placeholder.transform.SetSiblingIndex(newSiblingIndex);
    }
}
