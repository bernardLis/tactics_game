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
    VisualElement characterCardPortraitSkull;

    Label characterCardHealth;
    Label characterCardMana;

    VisualElement characterCardHealthBarInteractionResult;
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
    string hideTileInfoTweenID = "hideTileInfoTweenID";

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
        characterCardPortraitSkull = root.Q<VisualElement>("characterCardPortraitSkull");

        characterCardHealth = root.Q<Label>("characterCardHealth");
        characterCardMana = root.Q<Label>("characterCardMana");

        characterCardHealthBarInteractionResult = root.Q<VisualElement>("characterCardHealthBarInteractionResult");
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

        DOTween.Pause(hideTileInfoTweenID);

        DOTween.To(() => tileInfoCard.style.left.value.value, x => tileInfoCard.style.left = Length.Percent(x), cardShowValue, 0.5f)
               .SetEase(Ease.InOutSine);
    }

    public void HideTileInfo()
    {
        DOTween.To(() => tileInfoCard.style.left.value.value, x => tileInfoCard.style.left = Length.Percent(x), cardHideValue, 0.5f)
               .SetEase(Ease.InOutSine)
               .OnComplete(() => DisplayNone(tileInfoCard))
               .SetId(hideTileInfoTweenID);
    }


    /* character card */
    public void ShowCharacterCard(CharacterStats chStats)
    {
        PopulateCharacterCard(chStats);

        // clean-up
        DOTween.Pause(missingBarTweenID);

        characterCardPortraitSkull.style.display = DisplayStyle.None;
        characterCardHealthBarMissingHealth.style.backgroundColor = missingBarColor;
        characterCardHealthBarInteractionResult.style.display = DisplayStyle.None;

        // show the card
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
        float currentHealth = (float)chStats.currentHealth;
        float healthAfterInteraction = (float)chStats.currentHealth - val;
        healthAfterInteraction = Mathf.Clamp(healthAfterInteraction, 0, chStats.currentHealth);

        // text
        characterCardHealth.text = healthAfterInteraction + "/" + chStats.maxHealth.GetValue();

        // bar
        float result = (currentHealth - healthAfterInteraction) / currentHealth;
        result = Mathf.Clamp(result, 0, 1);

        // TODO: there is a better way to do it
        if(result <= 0)
            result = currentHealth / (float)chStats.maxHealth.GetValue();
        
        characterCardHealthBarInteractionResult.style.display = DisplayStyle.Flex;
        characterCardHealthBarInteractionResult.style.width = Length.Percent(result * 100);

        // death
        if(healthAfterInteraction <= 0)
            characterCardPortraitSkull.style.display = DisplayStyle.Flex;

        // "animate it"
        AnimateInteractionResult();
    }

    public void ShowHeal(CharacterStats chStats, int val)
    {
        // if there is nothing to heal, don't show the result
        if (chStats.maxHealth.GetValue() >= chStats.currentHealth)
            return;

        float currentHealth = (float)chStats.currentHealth;
        float healthAfterInteraction = (float)chStats.currentHealth + val;

        // text
        characterCardHealth.text = healthAfterInteraction + "/" + chStats.maxHealth.GetValue();

        // bar
        float result = (currentHealth - healthAfterInteraction) / currentHealth;
        result = Mathf.Clamp(result, 0, 1);
        characterCardHealthBarInteractionResult.style.display = DisplayStyle.Flex;
        characterCardHealthBarInteractionResult.style.width = Length.Percent(result * 100);

        // "animate it"
        AnimateInteractionResult();
    }

    void AnimateInteractionResult()
    {
        characterCardHealthBarInteractionResult.style.backgroundColor = missingBarColor;

        DOTween.ToAlpha(() => characterCardHealthBarInteractionResult.style.backgroundColor.value,
                         x => characterCardHealthBarInteractionResult.style.backgroundColor = x,
                         0f, 0.8f).SetLoops(-1, LoopType.Yoyo)
                         .SetId(missingBarTweenID);
    }

    void DisplayNone(VisualElement el) { el.style.display = DisplayStyle.None; }

}
