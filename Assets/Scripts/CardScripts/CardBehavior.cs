﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardBehavior : MonoBehaviour
{
    public Card card;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!card.staysOnField)
        {
            card.RemoveFromField();
        }
    }
}
