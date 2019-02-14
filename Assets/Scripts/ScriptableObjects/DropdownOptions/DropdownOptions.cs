using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "NewDropdownOptions", menuName = "DropdownOptions")]
public class DropdownOptions : ScriptableObject
{
    public List<string> options;
}
