using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class InfoCardUI : Singleton<InfoCardUI>
{
    // global
    CharacterUI _characterUI;

    // tile info
    VisualElement _tileInfoCard;
    Label _tileInfoText;

    // character card
    GameObject _displayedCharacter;
    VisualElement _characterCard;
    CharacterCardVisual _characterCardVisual;

    // interaction summary
    VisualElement _interactionSummary;

    Label _attackLabel;
    VisualElement _attackDamageValue;
    Label _attackHitValue;

    VisualElement _retaliationSummary;
    Label _retaliationDamageValue;
    Label _retaliationHitValue;

    // animate card left right on show/hide
    float _cardShowValue = 0f;
    float _cardHideValue = -100f;
    string _hideTileInfoTweenID = "hideTileInfoTweenID";

    protected override void Awake()
    {
        base.Awake();

        // getting ui elements
        var root = GetComponent<UIDocument>().rootVisualElement;

        // tile info
        _tileInfoCard = root.Q<VisualElement>("tileInfoCard");
        _tileInfoText = root.Q<Label>("tileInfoText");

        // character card
        _characterCard = root.Q<VisualElement>("characterCard");
        _characterCard.style.left = Length.Percent(_cardHideValue);

        // interaction summary
        _interactionSummary = root.Q<VisualElement>("interactionSummary");

        _attackLabel = root.Q<Label>("attackLabel");
        _attackDamageValue = root.Q<VisualElement>("attackDamageValue");
        _attackHitValue = root.Q<Label>("attackHitValue");

        _retaliationSummary = root.Q<VisualElement>("retaliationSummary");
        _retaliationDamageValue = root.Q<Label>("retaliationDamageValue");
        _retaliationHitValue = root.Q<Label>("retaliationHitValue");
    }

    void Start()
    {
        _characterUI = CharacterUI.Instance;
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
        if (_displayedCharacter == stats.gameObject)
            return;

        _displayedCharacter = stats.gameObject;
        _characterCard.Clear();

        _characterCardVisual = new(stats);
        _characterCard.Add(_characterCardVisual);

        // show the card
        _characterCard.style.display = DisplayStyle.Flex;
        DOTween.To(() => _characterCard.style.left.value.value, x => _characterCard.style.left = Length.Percent(x), _cardShowValue, 0.5f)
               .SetEase(Ease.InOutSine);
    }

    public void HideCharacterCard()
    {
        _displayedCharacter = null;
        DOTween.To(() => _characterCard.style.left.value.value, x => _characterCard.style.left = Length.Percent(x), _cardHideValue, 0.5f)
               .SetEase(Ease.InOutSine);
    }

    public void ShowInteractionSummary(CharacterStats attacker, CharacterStats defender, Ability ability)
    {
        _interactionSummary.style.display = DisplayStyle.Flex;
        DOTween.To(() => _interactionSummary.style.left.value.value, x => _interactionSummary.style.left = Length.Percent(x), _cardShowValue, 0.5f)
               .SetEase(Ease.InOutSine);

        _characterUI.HideHealthChange();
        DisplayNone(_retaliationSummary); // flexing it when it is necessary
        _attackDamageValue.Clear();

        // different labels and UI for heal / attack
        if (ability.AbilityType == AbilityType.Attack)
            HandleAttackAbilitySummary(attacker, defender, ability);

        if (ability.AbilityType == AbilityType.Push)
            HandlePushAbilitySummary(attacker, defender, ability);

        if (ability.AbilityType == AbilityType.Heal)
            HandleHealAbilitySummary(attacker, defender, ability);

        if (ability.AbilityType == AbilityType.Buff)
            HandleBuffAbilitySummary(attacker, defender, ability);

        if (ability.AbilityType == AbilityType.Create)
            HandleCreateAbilitySummary(attacker, defender, ability);

        if (ability.AbilityType == AbilityType.Utility)
            Debug.Log("Utlity summary not implemented");
    }

    void HandleAttackAbilitySummary(CharacterStats attacker, CharacterStats defender, Ability ability)
    {
        _attackLabel.text = "Attack";

        int attackValue = ability.CalculateInteractionResult(attacker, defender);
        if (defender.IsDamageAbsorbed(ability))
            attackValue = 0;
        Label value = new("" + (-1 * attackValue)); // it looks weird when it is negative.
        _attackDamageValue.Add(value);
        HandleStatusesAbilitySummary(ability);

        // self dmg
        if (attacker.gameObject == defender.gameObject)
            _characterUI.ShowHealthChange(attackValue);

        ShowHealthChange(defender, attackValue);

        float hitChance = (1 - defender.GetDodgeChance(attacker.gameObject, false)) * 100;
        hitChance = Mathf.Clamp(hitChance, 0, 100);

        _attackHitValue.text = hitChance + "%";

        ShowRetaliationSummary(attacker, defender, ability);
    }

    void HandlePushAbilitySummary(CharacterStats attacker, CharacterStats defender, Ability ability)
    {
        _attackLabel.text = "Push";

        int attackValue = ability.CalculateInteractionResult(attacker, defender);
        Label value = new("" + (-1 * attackValue)); // it looks weird when it is negative.
        _attackDamageValue.Add(value);
        HandleStatusesAbilitySummary(ability);

        ShowHealthChange(defender, attackValue);
        _attackHitValue.text = "100%";
    }

    void HandleHealAbilitySummary(CharacterStats attacker, CharacterStats defender, Ability ability)
    {
        _attackLabel.text = "Heal";

        int healValue = ability.CalculateInteractionResult(attacker, defender);
        Label value = new("" + healValue); // it looks weird when it is negative.
        _attackDamageValue.Add(value);
        HandleStatusesAbilitySummary(ability);

        if (attacker.gameObject == defender.gameObject)
            _characterUI.ShowHealthChange(healValue);

        ShowHealthChange(defender, healValue);
        _attackHitValue.text = 100 + "%";
    }

    void HandleBuffAbilitySummary(CharacterStats attacker, CharacterStats defender, Ability ability)
    {
        _attackLabel.text = "Buff";

        HandleStatusesAbilitySummary(ability);

        _attackHitValue.text = 100 + "%";
    }

    void HandleCreateAbilitySummary(CharacterStats attacker, CharacterStats defender, Ability ability)
    {
        _attackLabel.text = "Create";
        CreateAbility a = (CreateAbility)ability;
        if (a.CreatedObject.TryGetComponent(out IUITextDisplayable uiText))
        {
            Label value = new("" + uiText.DisplayText());
            value.style.whiteSpace = WhiteSpace.Normal;
            _attackDamageValue.Add(value);
        }
        _attackHitValue.text = 100 + "%";
    }

    void HandleStatusesAbilitySummary(Ability ability)
    {
        if (ability.StatModifier != null)
        {
            ModifierVisual mElement = new ModifierVisual(ability.StatModifier);
            _attackDamageValue.Add(mElement);
        }

        if (ability.Status != null)
        {
            ModifierVisual mElement = new ModifierVisual(ability.Status);
            _attackDamageValue.Add(mElement);
        }
    }

    void ShowRetaliationSummary(CharacterStats attacker, CharacterStats defender, Ability ability)
    {
        // retaliation only if there is an ability that character can retaliate with
        Ability retaliationAbility = defender.GetRetaliationAbility();
        if (retaliationAbility == null)
            return;

        bool willRetaliate = defender.WillRetaliate(attacker.gameObject);
        bool canRetaliate = retaliationAbility.CanHit(defender.gameObject, attacker.gameObject);
        if (!willRetaliate || !canRetaliate)
            return;

        // show change in attackers health after they get retaliated on
        _retaliationSummary.style.display = DisplayStyle.Flex;

        int relatiationResult = retaliationAbility.CalculateInteractionResult(defender, attacker); // correct defender, attacker
        _retaliationDamageValue.text = "" + (-1 * relatiationResult);

        float retaliationChance = (1 - attacker.GetDodgeChance(defender.gameObject, true)) * 100;
        retaliationChance = Mathf.Clamp(retaliationChance, 0, 100);
        _retaliationHitValue.text = retaliationChance + "%";

        _characterUI.ShowHealthChange(relatiationResult);
    }

    public void HideInteractionSummary()
    {
        _characterCardVisual.HealthBar.HideInteractionResult();
        _characterCardVisual.ManaBar.HideInteractionResult();

        DOTween.To(() => _interactionSummary.style.left.value.value, x => _interactionSummary.style.left = Length.Percent(x), _cardHideValue, 0.5f)
               .SetEase(Ease.InOutSine);
    }

    void ShowHealthChange(CharacterStats stats, int val)
    {
        _characterCardVisual.HealthBar.DisplayInteractionResult(stats.MaxHealth.GetValue(),
                                                                stats.CurrentHealth,
                                                                val);
    }

    public void ShowManaChange(CharacterStats stats, int val)
    {
        _characterCardVisual.ManaBar.DisplayInteractionResult(stats.MaxMana.GetValue(),
                                                                stats.CurrentMana,
                                                                val);
    }

    void DisplayNone(VisualElement el) { el.style.display = DisplayStyle.None; }
}
