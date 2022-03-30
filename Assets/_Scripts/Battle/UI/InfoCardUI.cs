using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using DG.Tweening;

public class InfoCardUI : MonoBehaviour
{
    // global
    CharacterUI _characterUI;

    // tile info
    VisualElement _tileInfoCard;
    Label _tileInfoText;

    // character card
    VisualElement _characterCard;
    Label _characterCardName;
    VisualElement _characterCardPortrait;
    VisualElement _characterCardPortraitSkull;

    Label _characterCardHealth;
    Label _characterCardMana;

    VisualElement _characterCardHealthBarInteractionResult;
    VisualElement _characterCardHealthBarMissingHealth;
    VisualElement _characterCardManaBarMissingMana;

    Label _characterCardStrength;
    Label _characterCardIntelligence;
    Label _characterCardAgility;
    Label _characterCardStamina;
    Label _characterCardArmor;
    Label _characterCardRange;

    VisualElement _characterCardModifierContainer;

    // character card
    CharacterCardVisual _characterCardVisual;

    // interaction summary
    VisualElement _interactionSummary;

    Label _attackLabel;
    Label _attackDamageValue;
    Label _attackHitValue;

    VisualElement _retaliationSummary;
    Label _retaliationDamageValue;
    Label _retaliationHitValue;

    // tweeen
    Color _damageBarColor;
    Color _healBarColor;

    string _missingHealthTweenID = "missingBarTweenID";

    // animate card left right on show/hide
    float _cardShowValue = 0f;
    float _cardHideValue = -100f;
    string _hideTileInfoTweenID = "hideTileInfoTweenID";

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
        var root = GetComponent<UIDocument>().rootVisualElement;

        // tile info
        _tileInfoCard = root.Q<VisualElement>("tileInfoCard");
        _tileInfoText = root.Q<Label>("tileInfoText");

        // character card
        _characterCard = root.Q<VisualElement>("characterCard");
        _characterCard.style.left = Length.Percent(_cardHideValue);

        _characterCardName = root.Q<Label>("characterCardName");
        _characterCardPortrait = root.Q<VisualElement>("characterCardPortrait");
        _characterCardPortraitSkull = root.Q<VisualElement>("characterCardPortraitSkull");

        _characterCardHealth = root.Q<Label>("characterCardHealth");
        _characterCardMana = root.Q<Label>("characterCardMana");

        _characterCardHealthBarInteractionResult = root.Q<VisualElement>("characterCardHealthBarInteractionResult");
        _characterCardHealthBarMissingHealth = root.Q<VisualElement>("characterCardHealthBarMissingHealth");
        _characterCardManaBarMissingMana = root.Q<VisualElement>("characterCardManaBarMissingMana");

        _characterCardStrength = root.Q<Label>("characterCardStrengthAmount");
        _characterCardIntelligence = root.Q<Label>("characterCardIntelligenceAmount");
        _characterCardAgility = root.Q<Label>("characterCardAgilityAmount");
        _characterCardStamina = root.Q<Label>("characterCardStaminaAmount");
        _characterCardArmor = root.Q<Label>("characterCardArmorAmount");
        _characterCardRange = root.Q<Label>("characterCardRangeAmount");

        _characterCardModifierContainer = root.Q<VisualElement>("characterCardModifierContainer");

        // interaction summary
        _interactionSummary = root.Q<VisualElement>("interactionSummary");

        _attackLabel = root.Q<Label>("attackLabel");
        _attackDamageValue = root.Q<Label>("attackDamageValue");
        _attackHitValue = root.Q<Label>("attackHitValue");

        _retaliationSummary = root.Q<VisualElement>("retaliationSummary");
        _retaliationDamageValue = root.Q<Label>("retaliationDamageValue");
        _retaliationHitValue = root.Q<Label>("retaliationHitValue");

        _damageBarColor = Helpers.GetColor("gray");
        _healBarColor = Helpers.GetColor("healthGainGreen");
    }

    void Start()
    {
        _characterUI = CharacterUI.instance;
    }

    /* tile info */
    public void ShowTileInfo(string info)
    {
        _tileInfoText.text = info;
        _tileInfoCard.style.display = DisplayStyle.Flex;

        DOTween.Pause(_hideTileInfoTweenID);

        DOTween.To(() => _tileInfoCard.style.left.value.value, x => _tileInfoCard.style.left = Length.Percent(x), _cardShowValue, 0.5f)
               .SetEase(Ease.InOutSine);
    }

    public void HideTileInfo()
    {
        DOTween.To(() => _tileInfoCard.style.left.value.value, x => _tileInfoCard.style.left = Length.Percent(x), _cardHideValue, 0.5f)
               .SetEase(Ease.InOutSine)
               .OnComplete(() => DisplayNone(_tileInfoCard))
               .SetId(_hideTileInfoTweenID);
    }

    /* character card */
    public void ShowCharacterCard(CharacterStats stats)
    {
        //PopulateCharacterCard(stats);
        _characterCard.Clear();

        _characterCardVisual = new(stats.Character);
        _characterCard.Add(_characterCardVisual);

        _characterCardVisual.HealthBar.DisplayMissingAmount(stats.MaxHealth.GetValue(), stats.CurrentHealth);
        _characterCardVisual.ManaBar.DisplayMissingAmount(stats.MaxMana.GetValue(), stats.CurrentMana);

        // clean-up
        DOTween.Pause(_missingHealthTweenID);

        _characterCardPortraitSkull.style.display = DisplayStyle.None;
        _characterCardHealthBarMissingHealth.style.backgroundColor = _damageBarColor;
        _characterCardHealthBarInteractionResult.style.display = DisplayStyle.None;

        // show the card
        DOTween.To(() => _characterCard.style.left.value.value, x => _characterCard.style.left = Length.Percent(x), _cardShowValue, 0.5f)
               .SetEase(Ease.InOutSine);
    }

    public void HideCharacterCard()
    {
        DOTween.To(() => _characterCard.style.left.value.value, x => _characterCard.style.left = Length.Percent(x), _cardHideValue, 0.5f)
               .SetEase(Ease.InOutSine);
    }

    void PopulateCharacterCard(CharacterStats stats)
    {
        BattleUIHelpers.HandleStatCheck(stats.Strength, _characterCardStrength);
        BattleUIHelpers.HandleStatCheck(stats.Intelligence, _characterCardIntelligence);
        BattleUIHelpers.HandleStatCheck(stats.Agility, _characterCardAgility);
        BattleUIHelpers.HandleStatCheck(stats.Stamina, _characterCardStamina);
        BattleUIHelpers.HandleStatCheck(stats.Armor, _characterCardArmor);
        BattleUIHelpers.HandleStatCheck(stats.MovementRange, _characterCardRange);

        _characterCardModifierContainer.Clear();
        List<VisualElement> elements = new(BattleUIHelpers.HandleStatModifiers(stats));
        elements.AddRange(BattleUIHelpers.HandleStatuses(stats));
        foreach (VisualElement el in elements)
            _characterCardModifierContainer.Add(el);
    }

    public void ShowInteractionSummary(CharacterStats attacker, CharacterStats defender, Ability ability)
    {
        DOTween.To(() => _interactionSummary.style.left.value.value, x => _interactionSummary.style.left = Length.Percent(x), _cardShowValue, 0.5f)
               .SetEase(Ease.InOutSine);

        // Clean-up
        _characterUI.HideDamage();

        int attackValue = CalculateInteractionResult(attacker, defender, ability);
        _attackDamageValue.text = "" + attackValue;

        // different labels and UI for heal / attack
        if (ability.AbilityType == AbilityType.Attack)
        {
            // self dmg
            if (attacker.gameObject == defender.gameObject)
                _characterUI.ShowDamage(CalculateInteractionResult(attacker, attacker, ability));

            ShowDamage(defender, attackValue);

            _attackLabel.text = "Attack";

            float hitChance = (1 - defender.GetDodgeChance(attacker.gameObject)) * 100;
            hitChance = Mathf.Clamp(hitChance, 0, 100);

            _attackHitValue.text = hitChance + "%";

        }

        if (ability.AbilityType == AbilityType.Heal)
        {
            if (attacker.gameObject == defender.gameObject)
                _characterUI.ShowHeal(attackValue);

            ShowHeal(defender, attackValue);

            _attackLabel.text = "Heal";
            _attackHitValue.text = 100 + "%";
        }

        if (ability.AbilityType == AbilityType.Buff)
        {
            if (attacker.gameObject == defender.gameObject)
                _characterUI.ShowHeal(attackValue);

            ShowHeal(defender, attackValue);

            _attackLabel.text = "Buff";

            if (ability.StatModifier != null)
                _attackDamageValue.text = ability.StatModifier.Value.ToString();
            _attackHitValue.text = 100 + "%";
        }

        // retaliation only on attack
        if (ability.AbilityType != AbilityType.Attack)
        {
            DisplayNone(_retaliationSummary);
            return;
        }

        // retaliation only if there is an ability that character can retaliate with
        Ability retaliationAbility = defender.GetRetaliationAbility();
        if (retaliationAbility == null)
        {
            DisplayNone(_retaliationSummary);
            return;
        }

        bool willRetaliate = defender.WillRetaliate(attacker.gameObject);
        bool canRetaliate = retaliationAbility.CanHit(defender.gameObject, attacker.gameObject);
        if (!willRetaliate || !canRetaliate)
        {
            DisplayNone(_retaliationSummary);
            return;
        }

        // show change in attackers health after they get retaliated on
        _retaliationSummary.style.display = DisplayStyle.Flex;

        int relatiationResult = CalculateInteractionResult(defender, attacker, retaliationAbility);
        _retaliationDamageValue.text = "" + relatiationResult;

        float retaliationChance = (1 - attacker.GetDodgeChance(attacker.gameObject)) * 100;
        retaliationChance = Mathf.Clamp(retaliationChance, 0, 100);
        _retaliationHitValue.text = retaliationChance + "%";

        _characterUI.ShowDamage(relatiationResult);
    }
    public void HideInteractionSummary()
    {
        DOTween.To(() => _interactionSummary.style.left.value.value, x => _interactionSummary.style.left = Length.Percent(x), _cardHideValue, 0.5f)
               .SetEase(Ease.InOutSine);
    }

    void ShowDamage(CharacterStats stats, int val)
    {

        Debug.Log($"show damage is called value: {val} ");

        _characterCardVisual.HealthBar.DisplayInteractionResult(stats.MaxHealth.GetValue(),
                                                                stats.CurrentHealth,
                                                                val);
        /*
                // text
                _characterCardHealth.text = healthAfterInteraction + "/" + stats.MaxHealth.GetValue();

                // bar
                float result = val / (float)stats.MaxHealth.GetValue();
                if (healthAfterInteraction == 0)
                    result = currentHealth / (float)stats.MaxHealth.GetValue();

                _characterCardHealthBarInteractionResult.style.display = DisplayStyle.Flex;
                // reset right
                _characterCardHealthBarInteractionResult.style.right = Length.Percent(0);
                _characterCardHealthBarInteractionResult.style.width = Length.Percent(result * 100);

                // death
                if (healthAfterInteraction <= 0)
                    _characterCardPortraitSkull.style.display = DisplayStyle.Flex;

                // "animate it"
                AnimateInteractionResult(_damageBarColor);
                */
    }

    void ShowHeal(CharacterStats stats, int val)
    {
        // if there is nothing to heal, don't show the result
        if (stats.CurrentHealth >= stats.MaxHealth.GetValue())
            return;

        float healthAfterInteraction = (float)stats.CurrentHealth + val;
        healthAfterInteraction = Mathf.Clamp(healthAfterInteraction, 0, stats.MaxHealth.GetValue());

        // text
        _characterCardHealth.text = healthAfterInteraction + "/" + stats.MaxHealth.GetValue();

        // bar
        float result = val / (float)stats.MaxHealth.GetValue();
        result = Mathf.Clamp(result, 0, 1);

        _characterCardHealthBarInteractionResult.style.display = DisplayStyle.Flex;
        // move it left, to show that it is health gain not loss.
        _characterCardHealthBarInteractionResult.style.right = Length.Percent(result * 100);
        _characterCardHealthBarInteractionResult.style.width = Length.Percent(result * 100);

        // "animate it"
        AnimateInteractionResult(_healBarColor);
    }

    void AnimateInteractionResult(Color color)
    {
        _characterCardHealthBarInteractionResult.style.backgroundColor = color;

        DOTween.ToAlpha(() => _characterCardHealthBarInteractionResult.style.backgroundColor.value,
                         x => _characterCardHealthBarInteractionResult.style.backgroundColor = x,
                         0f, 0.8f).SetLoops(-1, LoopType.Yoyo)
                         .SetId(_missingHealthTweenID);
    }

    int CalculateInteractionResult(CharacterStats attacker, CharacterStats defender, Ability ability)
    {
        int result = 0;

        // TODO: differentiate between abilities that calculate value from int/str
        // maybe ability should count that? 
        if (ability.AbilityType == AbilityType.Attack)
            result = ability.BasePower + attacker.Strength.GetValue() - defender.Armor.GetValue();

        if (ability.AbilityType == AbilityType.Heal)
            result = ability.BasePower + attacker.Intelligence.GetValue();


        return result;
    }

    void DisplayNone(VisualElement el) { el.style.display = DisplayStyle.None; }

}
