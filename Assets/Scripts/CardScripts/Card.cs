using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public int cost;
    public new string name;
    public string description;
    public string type;
    public bool staysOnField;
    public bool isInGraveyard;
    public bool isFast;
    public Sprite artwork;
    public Sprite artworkBackground;

    void EnterGraveyard()
    {
        isInGraveyard = true;
        staysOnField = true;
    }

    public void RemoveFromField()
    {
        EnterGraveyard();
    }
}
