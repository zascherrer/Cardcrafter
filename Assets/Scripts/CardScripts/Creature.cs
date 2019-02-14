using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : Card
{
    public int power;
    public int durability;

    void CheckForDeath()
    {
        if (durability <= 0)
        {
            staysOnField = false;
        }
    }
}
