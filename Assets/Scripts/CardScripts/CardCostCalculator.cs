using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardCostCalculator
{
    private static float baseCost = 1;
    private static bool matchFound = false;
    private static bool isBeneficial = false;
    private static bool isHarmful = false;
    private static bool isLethal = false;
    private static bool targetsEnemy = false;
    private static bool targetsFriendly = false;
    private static bool targetsAny = false;
    private static bool targetsAll = false;
    private static bool targetsPlayer = false;
    private static bool targetsCreature = false;

    private static float conditionalMultiplier = 0f;
    private static float effectModifier = 0f;
    private static float numberOfTargetsMultiplier = 0f;
    private static float targetTypeMultiplier = 0f;
    private static float numberOfRepititionsMultiplier = 1f;

    public static int CalculateCost(List<string> keywords, string cardType, int power = 0, int durability = 0)
    {
        float costMultiplier = 1f;
        float effectCost = 0f;
        bool firstNumeralFound = false;
        int result;

        baseCost = CalculateBaseCost(cardType);
        baseCost += CalculatePowerAndDurabilityCost(power, durability);

        for (int i = 0; i < keywords.Count; i++)
        {
            costMultiplier += CalculateCostMultiplier(keywords[i], firstNumeralFound, out bool firstNumeralFoundResult);
            firstNumeralFound = firstNumeralFoundResult;
        }

        effectCost = CalculateEffectCost();
        baseCost += effectCost;

        if (baseCost < 1)
        {
            baseCost = 1;
        }
        
        result = Mathf.CeilToInt(baseCost * costMultiplier);
        result = AdjustForHighCosts(result);
        return result;
    }

    private static float CalculateBaseCost(string cardType)
    {
        string cardTypeLowercase = cardType.ToLower();

        switch (cardTypeLowercase)
        {
            case "creature":
                return 1;
            case "spell":
                return 0.75f;
            case "weapon":
                return 0.5f;
            case "artifact":
                return 1.5f;
            default:
                Debug.Log("Card type is invalid.");
                return 100;
        }
    }

    private static float CalculatePowerAndDurabilityCost(int power, int durability)
    {
        float result = 0f;

        result = ((power + (durability / 2f)) / 2f) - 1;

        return result;
    }

    private static float CalculateCostMultiplier(string keyword, bool firstNumeralFound, out bool firstNumeralFoundResult)
    {
        bool placeholder = firstNumeralFound;
        float result = 0;

        result = KeywordCosts(keyword);
        if (!matchFound)
        {
            ConditionalCosts(keyword);
        }
        if (!matchFound)
        {
            EffectCosts(keyword);
        }
        if (!matchFound)
        {
            NumeralCosts(keyword, firstNumeralFound, out placeholder);
        }
        if (!matchFound)
        {
            TargetCosts(keyword);
        }
        if (!matchFound)
        {
            TargetTypeCosts(keyword);
        }

        firstNumeralFoundResult = placeholder;
        matchFound = false;
        return result;
    }

    private static float CalculateEffectCost()
    {
        float result = 0f;
        float effectCostMultiplier = 1f;

        AdjustTargetTypeMultiplier();
        effectCostMultiplier = DetermineIfEffectIsHelpful();
        result = effectModifier * effectCostMultiplier;

        result *= conditionalMultiplier * numberOfTargetsMultiplier * targetTypeMultiplier * numberOfRepititionsMultiplier;

        //Debug.Log("Conditional: " + conditionalMultiplier + " NumberOfTargets: " + numberOfTargetsMultiplier + " targetType: " + targetTypeMultiplier );

        return result;
    }

    private static void AdjustTargetTypeMultiplier()
    {
        if (targetsFriendly && !targetsEnemy && !targetsCreature)
        {
            targetTypeMultiplier *= 0.5f;
        }
    }

    private static float DetermineIfEffectIsHelpful()
    {
        float result = 10000f;
        float fullCost = 1f;
        float reducedCost = 0.5f;
        float slightlyDiscountedCost = -0.15f;
        float significantlyIncreasedCost = 2f;
        float discountCost = -0.25f;

        bool affectsEntireBoard = targetsAny && targetsAll;
        bool winCondition = isLethal && targetsPlayer;
        bool loseCondition = winCondition && targetsFriendly && !targetsEnemy;

        if (loseCondition)
        {
            //If the effect would lose the player the game outright, make the effect discount the total card cost
            result = discountCost;
        }
        else if (winCondition)
        {
            //If the effect would win the player the game outright, significantly increase the cost
            result = significantlyIncreasedCost;
        }
        else if (isBeneficial)
        {
            if (isHarmful)
            {
                //If it's both beneficial and harmful, check to see if it affects the entire board 
                if (affectsEntireBoard)
                {
                    //if it affects the entire board, cost is reduced
                    result = reducedCost;
                }
                else
                {
                    //if it doesn't affect the entire board, cost is unaffected
                    result = fullCost;
                }
            }
            else if (targetsFriendly)
            {
                if (!targetsEnemy)
                {
                    //If it's only beneficial and only targets friendly, cost is unaffected
                    result = fullCost;
                }
                else
                {
                    //If it's only beneficial and targets both friendly and enemy, check to see if it affects all targets
                    if(affectsEntireBoard)
                    {
                        //if it affects the entire board, cost is reduced
                        result = reducedCost;
                    }
                    else
                    {
                        //if it doesn't affect the entire board, cost is unaffected
                        result = fullCost;
                    }
                }
            }
            else if (targetsEnemy)
            {
                //If it's only beneficial and only targets enemy, cost becomes a slight discount
                result = slightlyDiscountedCost;
            }
        }
        else if (isHarmful)
        {
            if (targetsFriendly)
            {
                if (!targetsEnemy)
                {
                    //If it's only harmful and only targets friendly, cost becomes a slight discount
                    result = slightlyDiscountedCost;
                }
                else
                {
                    //If it's only harmful and targets both friendly and enemy, check to see if it affects the entire board
                    if (affectsEntireBoard)
                    {
                        //if it affects the entire board, cost is reduced
                        result = reducedCost;
                    }
                    else
                    {
                        //if it doesn't affect the entire board, cost is unaffected
                        result = fullCost;
                    }
                }
            }
            else if (targetsEnemy)
            {
                //If it's only harmful and only targets enemy, cost is unaffected
                result = fullCost;
            }
        }

        if (isLethal)
        {
            result *= 3f;
        }

        return result;
    }

    private static float KeywordCosts(string keyword)
    {
        string keywordLowercase = keyword.ToLower();

        switch (keywordLowercase)
        {
            case "piercing":
                matchFound = true;
                return 1;
            case "shielded":
                matchFound = true;
                return 1;
            case "indestructible":
                matchFound = true;
                return 2;
            case "fast":
                matchFound = true;
                return 0.5f;
            default:
                return 0;
        }
    }

    private static void ConditionalCosts(string keyword)
    {
        string keywordLowercase = keyword.ToLower();

        switch (keywordLowercase)
        {
            case "begin turn":
                matchFound = true;
                conditionalMultiplier = 2f;
                break;
            case "end turn":
                matchFound = true;
                conditionalMultiplier = 2.5f;
                break;
            case "on summon":
                matchFound = true;
                conditionalMultiplier = 1f;
                break;
            case "on death":
                matchFound = true;
                conditionalMultiplier = 0.75f;
                break;
            default:
                break;
        }
    }

    private static void EffectCosts(string keyword)
    {
        string keywordLowercase = keyword.ToLower();

        switch (keywordLowercase)
        {
            case "deal damage":
                matchFound = true;
                isHarmful = true;
                isBeneficial = false;
                isLethal = false;
                effectModifier = 0.6f;
                break;
            case "destroy":
                matchFound = true;
                isHarmful = true;
                isBeneficial = false;
                isLethal = true;
                effectModifier = 2f;
                break;
            case "draw card":
                matchFound = true;
                isHarmful = false;
                isBeneficial = true;
                isLethal = false;
                effectModifier = 1.5f;
                break;
            case "return to hand":
                matchFound = true;
                isHarmful = true;
                isBeneficial = true;
                isLethal = false;
                effectModifier = 1.5f;
                break;
            default:
                break;
        }
    }

    private static void NumeralCosts(string keyword, bool firstNumeralFound, out bool firstNumeralFoundResult)
    {
        string keywordLowercase = keyword.ToLower();
        bool isNumeral = int.TryParse(keywordLowercase, out int result);

        if (!firstNumeralFound)
        {
            if (isNumeral || keywordLowercase == "all")
            {
                matchFound = true;
                firstNumeralFoundResult = true;
                numberOfRepititionsMultiplier = 1f;
                if (keywordLowercase == "all")
                {
                    targetsAll = true;
                    numberOfTargetsMultiplier = 3f;
                }
                else
                {
                    targetsAll = false;
                    numberOfTargetsMultiplier = result / 2f;
                }
            }
            else
            {
                firstNumeralFoundResult = firstNumeralFound;
            }
        }
        else
        {
            if (isNumeral || keywordLowercase == "all")
            {
                matchFound = true;
                firstNumeralFoundResult = true;
                if (keywordLowercase == "all")
                {
                    isLethal = true;
                    numberOfRepititionsMultiplier = 5f;
                }
                else
                {
                    if (isLethal)
                    {
                        isLethal = true;
                    }
                    else
                    {
                        isLethal = false;
                    }
                    numberOfRepititionsMultiplier = result;
                }
            }
            else
            {
                firstNumeralFoundResult = firstNumeralFound;
            }
        }
    }

    private static void TargetCosts(string keyword)
    {
        string keywordLowercase = keyword.ToLower();

        switch (keywordLowercase)
        {
            case "any":
                matchFound = true;
                targetsEnemy = true;
                targetsFriendly = true;
                targetsAny = true;
                break;
            case "enemy":
                matchFound = true;
                targetsEnemy = true;
                targetsFriendly = false;
                targetsAny = false;
                break;
            case "friendly":
                matchFound = true;
                targetsEnemy = false;
                targetsFriendly = true;
                targetsAny = false;
                break;
            default:
                break;
        }
    }

    private static void TargetTypeCosts(string keyword)
    {
        string keywordLowercase = keyword.ToLower();

        switch (keywordLowercase)
        {
            case "creature":
                targetTypeMultiplier = 1;
                targetsPlayer = false;
                targetsCreature = true;
                break;
            case "spell":
                targetTypeMultiplier = 0.75f;
                targetsPlayer = false;
                targetsCreature = false;
                break;
            case "weapon":
                targetTypeMultiplier = 0.25f;
                targetsPlayer = false;
                targetsCreature = false;
                break;
            case "artifact":
                targetTypeMultiplier = 1.25f;
                targetsPlayer = false;
                targetsCreature = false;
                break;
            case "player":
                targetTypeMultiplier = 2f;
                targetsPlayer = true;
                targetsCreature = false;
                break;
            default:
                break;  
        }
    }

    private static int AdjustForHighCosts(int originalCost)
    {
        float result = 0f;
        float mediumCostThreshold = 6f;
        float highCostThreshold = 10f;

        if (originalCost > mediumCostThreshold)
        {
            result = ((originalCost - mediumCostThreshold) * 0.75f) + mediumCostThreshold;

            if (result > highCostThreshold)
            {
                result = ((result - highCostThreshold) / 2f) + highCostThreshold;
            }
        }
        else
        {
            result = originalCost;
        }

        return Mathf.CeilToInt(result);
    }
}
