using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using DG.Tweening;

public class InfoCardUI : MonoBehaviour
{
    // global
    CharacterUI characterUI;

    // UI Elements
    UIDocument UIDocument;

    // tile info
    VisualElement tileInfoCard;
    Label tileInfoText;

    // character card
    VisualElement characterCard;
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

    VisualElement characterCardModifierContainer;

    // interaction summary
    VisualElement interactionSummary;

    Label attackLabel;
    Label attackDamageValue;
    Label attackHitValue;

    VisualElement retaliationSummary;
    Label retaliationDamageValue;
    Label retaliationHitValue;

    // tweeen
    public Color damageBarColor;
    public Color healBarColor;

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

        characterCardModifierContainer = root.Q<VisualElement>("characterCardModifierContainer");

        // interaction summary
        interactionSummary = root.Q<VisualElement>("interactionSummary");

        attackLabel = root.Q<Label>("attackLabel");
        attackDamageValue = root.Q<Label>("attackDamageValue");
        attackHitValue = root.Q<Label>("attackHitValue");

        retaliationSummary = root.Q<VisualElement>("retaliationSummary");
        retaliationDamageValue = root.Q<Label>("retaliationDamageValue");
        retaliationHitValue = root.Q<Label>("retaliationHitValue");
    }

    void Start()
    {
        characterUI = CharacterUI.instance;
    }

    /* tile info */
    public void ShowTileInfo(string _info)
    {
        tileInfoText.text = _info;
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
    public void ShowCharacterCard(CharacterStats _chStats)
    {
        PopulateCharacterCard(_chStats);

        // clean-up
        DOTween.Pause(missingBarTweenID);

        characterCardPortraitSkull.style.display = DisplayStyle.None;
        characterCardHealthBarMissingHealth.style.backgroundColor = damageBarColor;
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

    void PopulateCharacterCard(CharacterStats _stats)
    {
        characterCardName.text = _stats.character.characterName;
        characterCardPortrait.style.backgroundImage = _stats.character.portrait.texture;

        characterCardHealth.text = _stats.currentHealth + "/" + _stats.maxHealth.GetValue();
        characterCardMana.text = _stats.currentMana + "/" + _stats.maxMana.GetValue();

        // (float) casts are not redundant, without them it does not work
        float missingHealthPerc = ((float)_stats.maxHealth.GetValue() - (float)_stats.currentHealth)
                                  / (float)_stats.maxHealth.GetValue();
        missingHealthPerc = Mathf.Clamp(missingHealthPerc, 0, 1);
        float missingManaPerc = ((float)_stats.maxMana.GetValue() - (float)_stats.currentMana)
                                / (float)_stats.maxMana.GetValue();

        characterCardHealthBarMissingHealth.style.width = Length.Percent(missingHealthPerc * 100);
        characterCardManaBarMissingMana.style.width = Length.Percent(missingManaPerc * 100);

        characterCardStrengthAmount.text = "" + _stats.strength.GetValue();
        characterCardIntelligenceAmount.text = "" + _stats.intelligence.GetValue();
        characterCardAgilityAmount.text = "" + _stats.agility.GetValue();
        characterCardStaminaAmount.text = "" + _stats.stamina.GetValue();
        characterCardArmorAmount.text = "" + _stats.armor.GetValue();
        characterCardRangeAmount.text = "" + _stats.movementRange.GetValue();

        HandleStatCheck(_stats.strength, characterCardStrengthAmount);
        HandleStatCheck(_stats.intelligence, characterCardIntelligenceAmount);
        HandleStatCheck(_stats.agility, characterCardAgilityAmount);
        HandleStatCheck(_stats.stamina, characterCardStaminaAmount);
        HandleStatCheck(_stats.armor, characterCardArmorAmount);
        HandleStatCheck(_stats.movementRange, characterCardRangeAmount);

        characterCardModifierContainer.Clear();
        HandleStatModifiers(_stats);
        HandleStatuses(_stats);
    }

    // TODO: common to infoCardUI and characterUI
    void HandleStatCheck(Stat _stat, Label _label)
    {
        _label.style.color = Color.white;
        if (_stat.GetValue() > _stat.baseValue)
            _label.style.color = Color.green;
        if (_stat.GetValue() < _stat.baseValue)
            _label.style.color = Color.red;
    }

    void HandleStatuses(CharacterStats _stats)
    {
        if (_stats.statuses.Count == 0)
            return;

        foreach (Status s in _stats.statuses)
        {
            VisualElement mElement = new VisualElement();
            mElement.style.backgroundImage = s.icon.texture;
            mElement.AddToClassList("modifierIconContainer");
            characterCardModifierContainer.Add(mElement);
        }
    }

    void HandleStatModifiers(CharacterStats _stats)
    {
        foreach (Stat s in _stats.stats)
        {
            List<StatModifier> modifiers = s.GetActiveModifiers();
            if (modifiers.Count == 0)
                continue;

            foreach (StatModifier m in modifiers)
            {
                VisualElement mElement = new VisualElement();
                mElement.style.backgroundImage = m.icon.texture;
                mElement.AddToClassList("modifierIconContainer");
                characterCardModifierContainer.Add(mElement);
            }
        }
    }

    public void ShowInteractionSummary(CharacterStats _attacker, CharacterStats _defender, Ability _ability)
    {
        DOTween.To(() => interactionSummary.style.left.value.value, x => interactionSummary.style.left = Length.Percent(x), cardShowValue, 0.5f)
               .SetEase(Ease.InOutSine);

        // Clean-up
        characterUI.HideDamage();

        int attackValue = CalculateInteractionResult(_attacker, _defender, _ability);
        attackDamageValue.text = "" + attackValue;

        // different labels and UI for heal / attack
        if (_ability.aType == AbilityType.Attack)
        {
            if (_attacker.gameObject == _defender.gameObject)
                characterUI.ShowDamage(CalculateInteractionResult(_attacker, _attacker, _ability));

            ShowDamage(_defender, attackValue);

            attackLabel.text = "Attack";

            float hitChance = (1 - _defender.GetDodgeChance(_attacker.gameObject)) * 100;
            hitChance = Mathf.Clamp(hitChance, 0, 100);

            attackHitValue.text = hitChance + "%";

        }

        if (_ability.aType == AbilityType.Heal)
        {
            if (_attacker.gameObject == _defender.gameObject)
                characterUI.ShowHeal(attackValue);

            ShowHeal(_defender, attackValue);

            attackLabel.text = "Heal";
            attackHitValue.text = 100 + "%";
        }

        if (_ability.aType == AbilityType.Buff)
        {
            if (_attacker.gameObject == _defender.gameObject)
                characterUI.ShowHeal(attackValue);

            ShowHeal(_defender, attackValue);

            attackLabel.text = "Buff";

            if (_ability.statModifier != null)
                attackDamageValue.text = "" + _ability.statModifier.value; // TODO: lazy way
            attackHitValue.text = 100 + "%";
        }


        // retaliation only on attack
        if (_ability.aType != AbilityType.Attack)
        {
            DisplayNone(retaliationSummary);
            return;
        }

        // retaliation only if there is an ability that character can retaliate with
        Ability retaliationAbility = _defender.GetRetaliationAbility();
        if (retaliationAbility == null)
        {
            DisplayNone(retaliationSummary);
            return;
        }

        bool willRetaliate = _defender.WillRetaliate(_attacker.gameObject);
        bool canRetaliate = retaliationAbility.CanHit(_defender.gameObject, _attacker.gameObject);
        if (!willRetaliate || !canRetaliate)
        {
            DisplayNone(retaliationSummary);
            return;
        }

        // show change in attackers health after they get retaliated on
        retaliationSummary.style.display = DisplayStyle.Flex;

        int relatiationResult = CalculateInteractionResult(_defender, _attacker, retaliationAbility);
        retaliationDamageValue.text = "" + relatiationResult;

        float retaliationChance = (1 - _attacker.GetDodgeChance(_attacker.gameObject)) * 100;
        retaliationChance = Mathf.Clamp(retaliationChance, 0, 100);
        retaliationHitValue.text = retaliationChance + "%";

        characterUI.ShowDamage(relatiationResult);
    }
    public void HideInteractionSummary()
    {
        DOTween.To(() => interactionSummary.style.left.value.value, x => interactionSummary.style.left = Length.Percent(x), cardHideValue, 0.5f)
               .SetEase(Ease.InOutSine);
    }

    void ShowDamage(CharacterStats _chStats, int _val)
    {
        float currentHealth = (float)_chStats.currentHealth;
        float healthAfterInteraction = (float)_chStats.currentHealth - _val;
        healthAfterInteraction = Mathf.Clamp(healthAfterInteraction, 0, _chStats.currentHealth);

        // text
        characterCardHealth.text = healthAfterInteraction + "/" + _chStats.maxHealth.GetValue();

        // bar
        float result = _val / (float)_chStats.maxHealth.GetValue();
        if (healthAfterInteraction == 0)
            result = currentHealth / (float)_chStats.maxHealth.GetValue();

        characterCardHealthBarInteractionResult.style.display = DisplayStyle.Flex;
        // reset right
        characterCardHealthBarInteractionResult.style.right = Length.Percent(0);
        characterCardHealthBarInteractionResult.style.width = Length.Percent(result * 100);

        // death
        if (healthAfterInteraction <= 0)
            characterCardPortraitSkull.style.display = DisplayStyle.Flex;

        // "animate it"
        AnimateInteractionResult(damageBarColor);
    }

    void ShowHeal(CharacterStats _chStats, int _val)
    {
        // if there is nothing to heal, don't show the result
        if (_chStats.currentHealth >= _chStats.maxHealth.GetValue())
            return;

        float healthAfterInteraction = (float)_chStats.currentHealth + _val;
        healthAfterInteraction = Mathf.Clamp(healthAfterInteraction, 0, _chStats.maxHealth.GetValue());

        // text
        characterCardHealth.text = healthAfterInteraction + "/" + _chStats.maxHealth.GetValue();

        // bar
        float result = _val / (float)_chStats.maxHealth.GetValue();
        result = Mathf.Clamp(result, 0, 1);

        characterCardHealthBarInteractionResult.style.display = DisplayStyle.Flex;
        // move it left, to show that it is health gain not loss.
        characterCardHealthBarInteractionResult.style.right = Length.Percent(result * 100);
        characterCardHealthBarInteractionResult.style.width = Length.Percent(result * 100);

        // "animate it"
        AnimateInteractionResult(healBarColor);
    }

    void AnimateInteractionResult(Color _color)
    {
        characterCardHealthBarInteractionResult.style.backgroundColor = _color;

        DOTween.ToAlpha(() => characterCardHealthBarInteractionResult.style.backgroundColor.value,
                         x => characterCardHealthBarInteractionResult.style.backgroundColor = x,
                         0f, 0.8f).SetLoops(-1, LoopType.Yoyo)
                         .SetId(missingBarTweenID);
    }

    int CalculateInteractionResult(CharacterStats _attacker, CharacterStats _defender, Ability _ability)
    {
        int result = 0;

        // TODO: differentiate between abilities that calculate value from int/str
        if (_ability.aType == AbilityType.Attack)
            result = _ability.value + _attacker.strength.GetValue() - _defender.armor.GetValue();

        if (_ability.aType == AbilityType.Heal)
            result = _ability.value + _attacker.intelligence.GetValue();


        return result;
    }

    void DisplayNone(VisualElement el) { el.style.display = DisplayStyle.None; }

}
