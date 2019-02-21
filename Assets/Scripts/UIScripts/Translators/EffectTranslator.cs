using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EffectTranslator
{
    public static List<string> words;
    public static bool shouldPluralize = false;
    public static bool pluralizable = false;
    private static string lastWords = null;

    public static List<string> Translate(List<string> effectWords)
    {
        words = DuplicateList(effectWords);
        List<string> result = new List<string>();

        for (int i = 0; i < effectWords.Count; i++)
        {
            words[i] = TranslateEffects(words[i]);
            words[i] = TranslateNumerals(words[i]);
        }

        words = AppendLastWords(words);
        result = words;
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

    private static string TranslateEffects(string originalWord)
    {
        string word = originalWord.ToLower();
        string result = "";

        switch (word)
        {
            case "deal damage":
                result = "deal";
                lastWords = "damage to";
                pluralizable = false;
                break;
            case "destroy":
                result = "destroy";
                lastWords = null;
                pluralizable = false;
                break;
            case "draw card":
                result = "draw";
                lastWords = "card";
                pluralizable = true;
                break;
            case "return to hand":
                result = "unsummon";
                lastWords = null;
                pluralizable = false;
                break;
            default:
                result = originalWord;
                break;
        }

        return result;
    }

    private static string TranslateNumerals(string originalWord)
    {
        string word = originalWord.ToLower();
        string result = "";
        bool isNumber = int.TryParse(word, out int throwaway);

        switch (word)
        {
            case "all":
                result = "maximum";
                shouldPluralize = true;
                break;
            case "1":
                result = "1";
                shouldPluralize = false;
                break;
            default:
                if (isNumber)
                {
                    shouldPluralize = true;
                }
                result = originalWord;
                break;
        }

        return result;
    }

    private static List<string> AppendLastWords(List<string> words)
    {
        List<string> result = words;

        if (lastWords != null)
        {
            if (shouldPluralize && pluralizable)
            {
                lastWords += "s";
            }

            result.Add(lastWords);
        }

        return result;
    }
}
