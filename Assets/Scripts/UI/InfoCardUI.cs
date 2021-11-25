using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class InfoCardUI : MonoBehaviour
{
    // UI Elements
    UIDocument UIDocument;

    // tile info
    VisualElement tileInfoCard;
    Label tileInfoText;

    // character card
    VisualElement characterCard;
    //VisualElement infoCardContainer;
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

    public Color missingBarColor;
    string missingBarTweenID = "missingBarTweenID";

    // animate card left right on show/hide
    float cardShowValue = 0f;
    float cardHideValue = -100f;

    public static InfoCardUI instance;
    void Awake()
    {
        #region Singleton
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

        // tile info
        tileInfoCard = root.Q<VisualElement>("tileInfoCard");
        tileInfoText = root.Q<Label>("tileInfoText");

        // character card
        characterCard = root.Q<VisualElement>("characterCard");
        characterCard.style.left = Length.Percent(cardHideValue);

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

    /* tile info */
    public void ShowTileInfo(string info)
    {
        tileInfoText.text = info;
        tileInfoCard.style.display = DisplayStyle.Flex;

        DOTween.To(() => tileInfoCard.style.left.value.value, x => tileInfoCard.style.left = Length.Percent(x), cardShowValue, 0.5f)
               .SetEase(Ease.InOutSine);
    }

    public void HideTileInfo()
    {
        tileInfoCard.style.display = DisplayStyle.None;

        DOTween.To(() => tileInfoCard.style.left.value.value, x => tileInfoCard.style.left = Length.Percent(x), cardHideValue, 0.5f)
               .SetEase(Ease.InOutSine);
    }


    /* character card */
    public void ShowCharacterCard(CharacterStats chStats)
    {
        PopulateCharacterCard(chStats);

        DOTween.Pause(missingBarTweenID);
        characterCardHealthBarMissingHealth.style.backgroundColor = missingBarColor;

        DOTween.To(() => characterCard.style.left.value.value, x => characterCard.style.left = Length.Percent(x), cardShowValue, 0.5f)
               .SetEase(Ease.InOutSine);
    }

    public void HideCharacterCard()
    {
        DOTween.To(() => characterCard.style.left.value.value, x => characterCard.style.left = Length.Percent(x), cardHideValue, 0.5f)
               .SetEase(Ease.InOutSine);
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

    public void ShowDamage(CharacterStats chStats, int val)
    {
        float maxHealth = (float)chStats.maxHealth.GetValue();
        float healthAfterInteraction = (float)chStats.currentHealth - val;

        // text
        characterCardHealth.text = healthAfterInteraction + "/" + maxHealth;

        // bar
        float missingHealthPerc = (maxHealth - healthAfterInteraction) / maxHealth;
        missingHealthPerc = Mathf.Clamp(missingHealthPerc, 0, 1);
        characterCardHealthBarMissingHealth.style.width = Length.Percent(missingHealthPerc * 100);

        // "animate it"
        AnimateInteractionResult();
    }

    public void ShowHeal(CharacterStats chStats, int val)
    {
        // if there is nothing to heal, don't show the result
        if (chStats.maxHealth.GetValue() >= chStats.currentHealth)
            return;

        float maxHealth = (float)chStats.maxHealth.GetValue();
        float healthAfterInteraction = (float)chStats.currentHealth + val;

        // text
        characterCardHealth.text = healthAfterInteraction + "/" + maxHealth;

        // bar
        float missingHealthPerc = (maxHealth - healthAfterInteraction) / maxHealth;
        missingHealthPerc = Mathf.Clamp(missingHealthPerc, 0, 1);
        characterCardHealthBarMissingHealth.style.width = Length.Percent(missingHealthPerc * 100);

        // "animate it"
        AnimateInteractionResult();
    }

    void AnimateInteractionResult()
    {
        characterCardHealthBarMissingHealth.style.backgroundColor = missingBarColor;

        DOTween.ToAlpha(() => characterCardHealthBarMissingHealth.style.backgroundColor.value,
                         x => characterCardHealthBarMissingHealth.style.backgroundColor = x,
                         0f, 0.8f).SetLoops(-1, LoopType.Yoyo)
                         .SetId(missingBarTweenID);
    }

}
