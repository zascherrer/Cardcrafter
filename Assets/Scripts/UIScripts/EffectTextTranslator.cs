using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EffectTextTranslator
{
    private static bool shouldCapitalize = false;
    private static bool shouldPluralize = true;
    private static bool sentenceIsOver = false;
    private static bool hasConditional = true;
    private static string secondNumeral = "";
    private static List<string> words = new List<string>();
    private static string lastWords = "";
    private static string[] lastWordsArray;

    public static List<string> TranslateEffectText(List<string> wordsFromSource)
    {
        words = DuplicateList(wordsFromSource);
        bool firstNumeralFound = false;

        for (int i = 0; i < words.Count; i++)
        {
            CheckForCapitalization(i);

            words[i] = TranslateConditionals(words[i]);
            words[i] = TranslateEffects(words[i]);
            words[i] = TranslateNumerals(words[i], firstNumeralFound, out bool firstNumeralFoundResult);
            words[i] = TranslateTargets(words[i]);
            words[i] = TranslateTargetTypes(words[i]);

            firstNumeralFound = firstNumeralFoundResult;
        }
        RemoveEmptySlots();
        FixAllTarget();
        AppendLastWords();
        AddPeriod();
        return words;
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

    private static void FixAllTarget()
    {
        for (int i = 1; i < words.Count; i++)
        {
            if (words[i] == "target" && words[i - 1].ToLower() == "all")
            {
                words[i] = "";
                RemoveEmptySlots();
            }
        }
    }

    private static string TranslateConditionals(string originalWord)
    {
        string word = originalWord.ToLower();

        switch (word)
        {
            case "begin turn":
                hasConditional = true;
                return "At the beginning of the turn,";
            case "end turn":
                hasConditional = true;
                return "At the end of the turn,";
            case "on summon":
                hasConditional = false;
                return "";
            case "on death":
                hasConditional = true;
                return "When this card goes to the graveyard,";
            default:
                return originalWord;
        }
    }

    private static string TranslateEffects(string originalWord)
    {
        string word = originalWord.ToLower();
        string result = "";

        switch (word)
        {
            case "deal damage":
                result = "";
                lastWords = "takes X damage";
                break;
            case "destroy":
                result = "Destroy";
                lastWords = "";
                break;
            case "draw card":
                result = "";
                lastWords = "draws X cards";
                break;
            case "return to hand":
                result = "Return";
                lastWords = "to its owner's hand";
                break;
            default:
                result = originalWord;
                break;
        }

        if (!shouldCapitalize)
        {
            result = result.ToLower();
        }
        else
        {
            shouldCapitalize = false;
        }

        return result;
    }

    private static string TranslateNumerals(string originalWord, bool firstNumeralFound, out bool firstNumeralFoundOut)
    {
        string word = originalWord.ToLower();
        string result = "";
        bool isNumeral = int.TryParse(word, out int throwaway);

        if ((isNumeral || word == "all") && !firstNumeralFound)
        {
            firstNumeralFoundOut = true;
            switch (word)
            {
                case "1":
                    result = originalWord;
                    shouldPluralize = false;
                    break;
                case "all":
                    result = originalWord;
                    shouldPluralize = true;
                    break;
                default:
                    result = originalWord;
                    if (isNumeral)
                    {
                        shouldPluralize = true;
                    }
                    break;
            }
        }
        else if ((isNumeral || word == "all") && firstNumeralFound)
        {
            firstNumeralFoundOut = firstNumeralFound;
            switch (word)
            {
                case "1":
                    secondNumeral = originalWord;
                    result = "";
                    break;
                case "all":
                    secondNumeral = "maximum";
                    result = "";
                    break;
                default:
                    secondNumeral = originalWord;
                    result = "";
                    break;
            }
        }
        else
        {
            firstNumeralFoundOut = firstNumeralFound;
            result = originalWord;
        }
        

        return result;
    }

    private static string TranslateTargets(string originalWord)
    {
        string word = originalWord.ToLower();

        switch (word)
        {
            case "any":
                return "target";
            case "friendly":
                return "friendly";
            case "enemy":
                return "enemy";
            default:
                return originalWord;
        }
    }

    private static string TranslateTargetTypes(string originalWord)
    {
        string word = originalWord.ToLower();
        string result = "";
        bool isTargetType = false;

        switch (word)
        {
            case "creature":
                result = "creature";
                isTargetType = true;
                break;
            case "spell":
                result = "spell";
                isTargetType = true;
                break;
            case "player":
                result = "player";
                isTargetType = true;
                break;
            case "artifact":
                result = "artifact";
                isTargetType = true;
                break;
            case "weapon":
                result = "weapon";
                isTargetType = true;
                break;
            default:
                result = originalWord;
                break;
        }

        if(shouldPluralize && isTargetType)
        {
            result += "s";
        }

        if (isTargetType)
        {
            sentenceIsOver = true;
        }

        return result;
    }

    private static void CheckForCapitalization(int index)
    {
        if (index > 0)
        {
            if (words[index - 1] == "" && hasConditional == false)
            {
                shouldCapitalize = true;
            }
            else
            {
                shouldCapitalize = false;
            }
        }
        else
        {
            shouldCapitalize = true;
        }
    }

    private static void RemoveEmptySlots()
    {
        if (sentenceIsOver)
        {
            List<string> newWords = new List<string>();

            for (int i = 0; i < words.Count; i++)
            {
                if (words[i] != "")
                {
                    newWords.Add(words[i]);
                }
            }

            words = newWords;
        }
    }

    private static void AppendLastWords()
    {
        if (lastWords != "" && sentenceIsOver)
        {
            lastWordsArray = lastWords.Split(' ');
            lastWords = TranslateLastWords(lastWordsArray);

            lastWords += ".";
            words.Add(lastWords);
        }
    }

    private static string TranslateLastWords(string[] wordArrayOriginal)
    {
        string result = "";
        string[] wordArray = InsertSecondNumeral(wordArrayOriginal);

        for (int i = 0; i < wordArray.Length; i++)
        {
            if (i == wordArray.Length - 1)
            {
                switch (wordArray[i].ToLower())
                {
                    default:
                        result += wordArray[i];
                        break;
                }
            }
            else
            {
                switch (wordArray[i].ToLower())
                {
                    case "takes":
                        result += Pluralize("takes", "take") + " ";
                        break;
                    case "its":
                        result += Pluralize("its", "their") + " ";
                        break;
                    case "draws":
                        result += Pluralize("draws", "draw") + " ";
                        break;
                    default:
                        result += wordArray[i] + " ";
                        break;
                }
            }
        }

        return result;
    }

    private static string[] InsertSecondNumeral(string[] wordArrayOriginal)
    {
        string[] result = new string[wordArrayOriginal.Length];

        for (int i = 0; i < wordArrayOriginal.Length; i++)
        {
            if (wordArrayOriginal[i] == "X")
            {
                result[i] = secondNumeral;
            }
            else
            {
                result[i] = wordArrayOriginal[i];
            }
        }

        return result;
    }

    private static string Pluralize(string singular, string plural)
    {
        if (shouldPluralize)
        {
            return plural;
        }
        else
        {
            return singular;
        }
    }

    private static void AddPeriod()
    {
        if (words.Count > 0 && sentenceIsOver && lastWords == "")
        {
            words[words.Count - 1] += ".";
        }
    }
}
