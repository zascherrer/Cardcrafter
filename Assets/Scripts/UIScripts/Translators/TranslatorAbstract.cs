using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TranslatorAbstract 
{
    public static List<string> words;

    public static List<string> Translate(List<string> conditionalWords)
    {
        words = DuplicateList(conditionalWords);
        List<string> result = new List<string>();

        for (int i = 0; i < conditionalWords.Count; i++)
        {
            Debug.Log("Not Implemented");
        }

        result = DuplicateList(words);
        return result;
    }

    public static List<string> DuplicateList(List<string> wordsToDuplicate)
    {
        List<string> duplicate = new List<string>();

        for (int i = 0; i < wordsToDuplicate.Count; i++)
        {
            duplicate.Add(wordsToDuplicate[i]);
        }

        return duplicate;
    }
}
