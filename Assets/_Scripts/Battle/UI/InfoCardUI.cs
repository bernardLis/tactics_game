using UnityEngine;
using UnityEngine.UIElements;
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
    CharacterCardVisual _characterCardVisual;

    // interaction summary
    VisualElement _interactionSummary;

    Label _attackLabel;
    Label _attackDamageValue;
    Label _attackHitValue;

    VisualElement _retaliationSummary;
    Label _retaliationDamageValue;
    Label _retaliationHitValue;

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

        // interaction summary
        _interactionSummary = root.Q<VisualElement>("interactionSummary");

        _attackLabel = root.Q<Label>("attackLabel");
        _attackDamageValue = root.Q<Label>("attackDamageValue");
        _attackHitValue = root.Q<Label>("attackHitValue");

        _retaliationSummary = root.Q<VisualElement>("retaliationSummary");
        _retaliationDamageValue = root.Q<Label>("retaliationDamageValue");
        _retaliationHitValue = root.Q<Label>("retaliationHitValue");
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
        _characterCard.Clear();

        _characterCardVisual = new(stats);
        _characterCard.Add(_characterCardVisual);

        // show the card
        DOTween.To(() => _characterCard.style.left.value.value, x => _characterCard.style.left = Length.Percent(x), _cardShowValue, 0.5f)
               .SetEase(Ease.InOutSine);
    }

    public void HideCharacterCard()
    {
        DOTween.To(() => _characterCard.style.left.value.value, x => _characterCard.style.left = Length.Percent(x), _cardHideValue, 0.5f)
               .SetEase(Ease.InOutSine);
    }

    public void ShowInteractionSummary(CharacterStats attacker, CharacterStats defender, Ability ability)
    {
        DOTween.To(() => _interactionSummary.style.left.value.value, x => _interactionSummary.style.left = Length.Percent(x), _cardShowValue, 0.5f)
               .SetEase(Ease.InOutSine);

        _characterUI.HideHealthChange();

        int attackValue = ability.CalculateInteractionResult(attacker, defender);
        _attackDamageValue.text = "" + attackValue;

        // different labels and UI for heal / attack
        if (ability.AbilityType == AbilityType.Attack)
        {
            // self dmg
            if (attacker.gameObject == defender.gameObject)
                _characterUI.ShowHealthChange(ability.CalculateInteractionResult(attacker, attacker));

            ShowHealthChange(defender, attackValue);

            _attackLabel.text = "Attack";

            float hitChance = (1 - defender.GetDodgeChance(attacker.gameObject)) * 100;
            hitChance = Mathf.Clamp(hitChance, 0, 100);

            _attackHitValue.text = hitChance + "%";
        }

        if (ability.AbilityType == AbilityType.Heal)
        {
            if (attacker.gameObject == defender.gameObject)
                _characterUI.ShowHealthChange(attackValue);

            ShowHealthChange(defender, attackValue);

            _attackLabel.text = "Heal";
            _attackHitValue.text = 100 + "%";
        }

        if (ability.AbilityType == AbilityType.Buff)
        {
            if (attacker.gameObject == defender.gameObject)
                _characterUI.ShowHealthChange(attackValue);

            ShowHealthChange(defender, attackValue);

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

        int relatiationResult = retaliationAbility.CalculateInteractionResult(defender, attacker); // correct defender, attacker
        _retaliationDamageValue.text = "" + relatiationResult;

        float retaliationChance = (1 - attacker.GetDodgeChance(attacker.gameObject)) * 100;
        retaliationChance = Mathf.Clamp(retaliationChance, 0, 100);
        _retaliationHitValue.text = retaliationChance + "%";

        _characterUI.ShowHealthChange(relatiationResult);
    }

    public void HideInteractionSummary()
    {
        DOTween.To(() => _interactionSummary.style.left.value.value, x => _interactionSummary.style.left = Length.Percent(x), _cardHideValue, 0.5f)
               .SetEase(Ease.InOutSine);
    }

    void ShowHealthChange(CharacterStats stats, int val)
    {
        _characterCardVisual.HealthBar.DisplayInteractionResult(stats.MaxHealth.GetValue(),
                                                                stats.CurrentHealth,
                                                                val);
    }

    // TODO: hook-up to enemies when they are taking the turn
    void ShowManaChange(CharacterStats stats, int val)
    {
        _characterCardVisual.ManaBar.DisplayInteractionResult(stats.MaxHealth.GetValue(),
                                                                stats.CurrentHealth,
                                                                val);
    }

    void DisplayNone(VisualElement el) { el.style.display = DisplayStyle.None; }
}
