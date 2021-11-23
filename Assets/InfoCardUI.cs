using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InfoCardUI : MonoBehaviour
{
    // UI Elements
    UIDocument UIDocument;

    // character card
    VisualElement characterCardContainer;
    Label characterCardName;
    VisualElement characterCardPortrait;

    Label characterCardHealth;
    Label characterCardMana;

    VisualElement characterCardHealthBarMissingHealth;
    VisualElement characterCardManaBarMissingMana;

    Label characterCardStrengthAmount;
    Label characterCardIntelligenceAmount;
    Label characterCardAgilityAmount;
    Label characterCardStaminaAmount;
    Label characterCardArmorAmount;
    Label characterCardRangeAmount;

    // animate card left right on show/hide
    bool isCharacterCardAnimationOn;
    float characterCardLeftPercent = -30f;

    float characterCardShowValue = 0f;
    float characterCardHideValue = -30f;

    IVisualElementScheduledItem characterCardScheduler;

    #region Singleton
    public static InfoCardUI instance;
    void Awake()
    {
        // singleton
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of InfoCardUI found");
            return;
        }
        instance = this;

        #endregion

        // getting ui elements
        UIDocument = GetComponent<UIDocument>();
        var root = UIDocument.rootVisualElement;

        // character card
        characterCardContainer = root.Q<VisualElement>("characterCardContainer");
        characterCardContainer.style.left = Length.Percent(characterCardHideValue);

        characterCardName = root.Q<Label>("characterCardName");
        characterCardPortrait = root.Q<VisualElement>("characterCardPortrait");

        characterCardHealth = root.Q<Label>("characterCardHealth");
        characterCardMana = root.Q<Label>("characterCardMana");

        characterCardHealthBarMissingHealth = root.Q<VisualElement>("characterCardHealthBarMissingHealth");
        characterCardManaBarMissingMana = root.Q<VisualElement>("characterCardManaBarMissingMana");

        characterCardStrengthAmount = root.Q<Label>("characterCardStrengthAmount");
        characterCardIntelligenceAmount = root.Q<Label>("characterCardIntelligenceAmount");
        characterCardAgilityAmount = root.Q<Label>("characterCardAgilityAmount");
        characterCardStaminaAmount = root.Q<Label>("characterCardStaminaAmount");
        characterCardArmorAmount = root.Q<Label>("characterCardArmorAmount");
        characterCardRangeAmount = root.Q<Label>("characterCardRangeAmount");
    }

    public void ShowCharacterCard(CharacterStats chStats)
    {
        PopulateCharacterCard(chStats);

        // skip showing it, if it is already shown;
        if (characterCardContainer.style.left.value.value == characterCardShowValue)
            return;

        if (isCharacterCardAnimationOn)
            return;

        // 'animate' it to come up
        characterCardContainer.style.left = Length.Percent(characterCardLeftPercent);
        isCharacterCardAnimationOn = true;
        characterCardScheduler = characterCardContainer.schedule.Execute(() => AnimateCharacterCardRight()).Every(5); // ms
    }

    public void HideCharacterCard()
    {
        // skip hidding it, if it is already hidden;
        if (characterCardContainer.style.left.value.value == characterCardHideValue)
            return;

        if (isCharacterCardAnimationOn)
            return;

        isCharacterCardAnimationOn = true;
        characterCardScheduler = characterCardContainer.schedule.Execute(() => AnimateCharacterCardLeft()).Every(5); // ms
    }

    void AnimateCharacterCardRight()
    {
        if (characterCardLeftPercent <= characterCardShowValue)
        {
            characterCardContainer.style.left = Length.Percent(characterCardLeftPercent);
            characterCardLeftPercent++;
            return;
        }
        // TODO: idk how to destroy scheduler...
        isCharacterCardAnimationOn = false;

        characterCardScheduler.Pause();
    }

    void AnimateCharacterCardLeft()
    {
        if (characterCardLeftPercent >= characterCardHideValue)
        {
            characterCardContainer.style.left = Length.Percent(characterCardLeftPercent);
            characterCardLeftPercent--;
            return;
        }

        // TODO: idk how to destroy scheduler...
        isCharacterCardAnimationOn = false;

        characterCardScheduler.Pause();
    }


    void PopulateCharacterCard(CharacterStats chStats)
    {
        characterCardName.text = chStats.character.characterName;
        characterCardPortrait.style.backgroundImage = chStats.character.portrait.texture;

        characterCardHealth.text = chStats.currentHealth + "/" + chStats.maxHealth.GetValue();
        characterCardMana.text = chStats.currentMana + "/" + chStats.maxMana.GetValue();

        // (float) casts are not redundant, without them it does not work
        float missingHealthPerc = ((float)chStats.maxHealth.GetValue() - (float)chStats.currentHealth) / (float)chStats.maxHealth.GetValue();
        missingHealthPerc = Mathf.Clamp(missingHealthPerc, 0, 1);
        float missingManaPerc = ((float)chStats.maxMana.GetValue() - (float)chStats.currentMana) / (float)chStats.maxMana.GetValue();

        characterCardHealthBarMissingHealth.style.width = Length.Percent(missingHealthPerc * 100);
        characterCardManaBarMissingMana.style.width = Length.Percent(missingManaPerc * 100);

        characterCardStrengthAmount.text = "" + chStats.strength.GetValue();
        characterCardIntelligenceAmount.text = "" + chStats.intelligence.GetValue();
        characterCardAgilityAmount.text = "" + chStats.agility.GetValue();
        characterCardStaminaAmount.text = "" + chStats.stamina.GetValue();
        characterCardArmorAmount.text = "" + chStats.armor.GetValue();
        characterCardRangeAmount.text = "" + chStats.movementRange.GetValue();
    }

}
