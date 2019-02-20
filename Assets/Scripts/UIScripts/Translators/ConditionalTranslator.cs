using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConditionalTranslator
{
    public static List<string> words;
    public static bool shouldPluralize = false;

    public static List<string> Translate(List<string> conditionalWords)
    {
        words = DuplicateList(conditionalWords);
        List<string> result = new List<string>();

        for (int i = 0; i < conditionalWords.Count; i++)
        {
            words[i] = TranslateConditionals(words[i]);
            words[i] = TranslateTargets(words[i]);
            words[i] = TranslateTargetTypes(words[i]);
            words[i] = TranslateEffects(words[i]);
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

    private static string TranslateConditionals(string originalWord)
    {
        string word = originalWord.ToLower();

        switch (word)
        {
            case "begin turn":
                return "At the beginning of the turn,";
            case "end turn":
                return "At the end of the turn,";
            case "on summon":
                return "When played,";
            case "on death":
                return "When this card goes to the graveyard,";
            case "when...":
                return "When";
            default:
                return originalWord;
        }
    }

    private static string TranslateTargets(string originalWord)
    {
        string word = originalWord.ToLower();

        switch (word)
        {
            case "any":
                return "any";
            case "friendly":
                return "a friendly";
            case "enemy":
                return "an enemy";
            default:
                return originalWord;
        }
    }

    private static string TranslateTargetTypes(string originalWord)
    {
        string word = originalWord.ToLower();
        string result = "";

        switch (word)
        {
            case "creature":
                result = "creature";
                break;
            case "spell":
                result = "spell";
                break;
            case "player":
                result = "player";
                break;
            case "artifact":
                result = "artifact";
                break;
            case "weapon":
                result = "weapon";
                break;
            default:
                result = originalWord;
                break;
        }

        return result;
    }

    private static string TranslateEffects(string originalWord)
    {
        string word = originalWord.ToLower();
        string result = "";

        switch (word)
        {
            case "deal damage":
                result = "takes damage,";
                break;
            case "destroy":
                result = "is destroyed,";
                break;
            case "draw card":
                result = "draws a card,";
                break;
            case "return to hand":
                result = "is returned to its owner's hand,";
                break;
            default:
                result = originalWord;
                break;
        }

        return result;
    }

}
