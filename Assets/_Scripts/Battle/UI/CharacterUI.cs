using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using DG.Tweening;

public class CharacterUI : MonoBehaviour
{
    // global utility
    BattleInputController _battleInputController;
    BattleCharacterController _battleCharacterController;

    // UI Elements
    VisualElement _characterUIContainer;
    VisualElement _characterUI;

    VisualElement _characterUICharacterCard;

    VisualElement _characterUITooltipContainer;
    Label _characterUITooltipAbilityName;
    Label _characterUITooltipAbilityDescription;
    Label _characterUITooltipAbilityManaCost;
    Label _characterUITooltipModifierDescription;
    Label _characterUITooltipStatusDescription;

    Label _characterName;
    VisualElement _characterPortrait;
    VisualElement _characterPortraitSkull;

    Label _characterHealth;
    VisualElement _characterHealthBarMissingHealth;
    VisualElement _characterHealthBarRetaliationResult;

    Label _characterMana;
    VisualElement _characterManaBarMissingMana;
    VisualElement _characterManaBarInteractionResult;

    Label _characterStrength;
    Label _characterIntelligence;
    Label _characterAgility;
    Label _characterStamina;
    Label _characterArmor;
    Label _characterRange;

    VisualElement _modifierContainer;

    Button _characterAButton;
    Button _characterSButton;
    Button _characterDButton;

    Button _characterQButton;
    Button _characterWButton;
    Button _characterEButton;
    Button _characterRButton;
    List<Button> _abilityButtons;

    VisualElement _characterASkillIcon;
    VisualElement _characterSSkillIcon;

    VisualElement _characterQSkillIcon;
    VisualElement _characterWSkillIcon;
    VisualElement _characterESkillIcon;
    VisualElement _characterRSkillIcon;
    List<VisualElement> _abilityIcons;

    // local
    CharacterStats _selectedPlayerStats;

    // animate ui up/down on show/hide
    float _characterUIShowValue = 0f;
    float _characterUIHideValue = 20f;

    // buttons management
    Queue<IEnumerator> _buttonClickQueue = new();
    bool _wasClickEnqueued;



    // showing retaliation result
    Color _damageBarColor;
    Color _healBarColor;

    string _hideCharacterUIID = "hideCharacterUIID";
    string _manaUseTweenID = "manaUseTweenID";
    string _healthLostTweenID = "characterHealthBarTweenID";

    public static CharacterUI instance;
    void Awake()
    {
        #region Singleton
        // singleton
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of CharacterUI found");
            return;
        }
        instance = this;

        #endregion

        // getting ui elements
        var root = GetComponent<UIDocument>().rootVisualElement;

        _characterUIContainer = root.Q<VisualElement>("characterUIContainer");
        _characterUI = root.Q<VisualElement>("characterUI");
        // otherwise it is null
        _characterUI.style.top = Length.Percent(_characterUIHideValue);
        _characterUICharacterCard = root.Q<VisualElement>("characterUICharacterCard");

        _characterUITooltipContainer = root.Q<VisualElement>("characterUITooltipContainer");
        _characterUITooltipAbilityName = root.Q<Label>("characterUITooltipAbilityName");
        _characterUITooltipAbilityDescription = root.Q<Label>("characterUITooltipAbilityDescription");
        _characterUITooltipAbilityManaCost = root.Q<Label>("characterUITooltipAbilityManaCost");
        _characterUITooltipModifierDescription = root.Q<Label>("characterUITooltipModifierDescription");
        _characterUITooltipStatusDescription = root.Q<Label>("characterUITooltipStatusDescription");

        _characterName = root.Q<Label>("characterName");
        _characterPortrait = root.Q<VisualElement>("characterPortrait");
        _characterPortraitSkull = root.Q<VisualElement>("characterPortraitSkull");

        _characterHealth = root.Q<Label>("characterHealth");
        _characterHealthBarMissingHealth = root.Q<VisualElement>("characterHealthBarMissingHealth");
        _characterHealthBarRetaliationResult = root.Q<VisualElement>("characterHealthBarRetaliationResult");

        _characterMana = root.Q<Label>("characterMana");
        _characterManaBarMissingMana = root.Q<VisualElement>("characterManaBarMissingMana");
        _characterManaBarInteractionResult = root.Q<VisualElement>("characterManaBarInteractionResult");

        _characterStrength = root.Q<Label>("characterStrengthAmount");
        _characterIntelligence = root.Q<Label>("characterIntelligenceAmount");
        _characterAgility = root.Q<Label>("characterAgilityAmount");
        _characterStamina = root.Q<Label>("characterStaminaAmount");
        _characterArmor = root.Q<Label>("characterArmorAmount");
        _characterRange = root.Q<Label>("characterRangeAmount");

        _modifierContainer = root.Q<VisualElement>("modifierContainer");

        _characterAButton = root.Q<Button>("characterAButton");
        _characterSButton = root.Q<Button>("characterSButton");
        _characterDButton = root.Q<Button>("characterDButton");

        _characterQButton = root.Q<Button>("characterQButton");
        _characterWButton = root.Q<Button>("characterWButton");
        _characterEButton = root.Q<Button>("characterEButton");
        _characterRButton = root.Q<Button>("characterRButton");

        // TODO: this could be probably improved
        _abilityButtons = new();
        _abilityButtons.Add(_characterQButton);
        _abilityButtons.Add(_characterWButton);
        _abilityButtons.Add(_characterEButton);
        _abilityButtons.Add(_characterRButton);

        // register interaction callbacks (buttons)
        _characterAButton.clickable.clicked += AButtonClicked;
        _characterSButton.clickable.clicked += SButtonClicked;
        _characterDButton.clickable.clicked += DButtonClicked;

        _characterQButton.clickable.clicked += QButtonClicked;
        _characterWButton.clickable.clicked += WButtonClicked;
        _characterEButton.clickable.clicked += EButtonClicked;
        _characterRButton.clickable.clicked += RButtonClicked;

        _characterASkillIcon = root.Q<VisualElement>("characterASkillIcon");
        _characterSSkillIcon = root.Q<VisualElement>("characterSSkillIcon");

        _characterQSkillIcon = root.Q<VisualElement>("characterQSkillIcon");
        _characterWSkillIcon = root.Q<VisualElement>("characterWSkillIcon");
        _characterESkillIcon = root.Q<VisualElement>("characterESkillIcon");
        _characterRSkillIcon = root.Q<VisualElement>("characterRSkillIcon");

        // TODO: this could be probably improved
        _abilityIcons = new();
        _abilityIcons.Add(_characterQSkillIcon);
        _abilityIcons.Add(_characterWSkillIcon);
        _abilityIcons.Add(_characterESkillIcon);
        _abilityIcons.Add(_characterRSkillIcon);

        _damageBarColor = Helpers.GetColor("gray");
        _healBarColor = Helpers.GetColor("healthGainGreen");
    }

    void Start()
    {
        _battleInputController = BattleInputController.instance;
        _battleCharacterController = BattleCharacterController.instance;

        //https://answers.unity.com/questions/1590871/how-to-stack-coroutines-and-call-each-one-till-all.html
        StartCoroutine(CoroutineCoordinator());
    }

    // allow clicks only when not moving and character is selected & did not finish its turn
    // first 2 abilities should always be 
    void AButtonClicked()
    {
        if (!_battleCharacterController.CanSelectAbility())
            return;

        // TODO: hardcoded ability indexes
        _buttonClickQueue.Enqueue(HandleButtonClick(_selectedPlayerStats.BasicAbilities[0]));
    }

    void SButtonClicked()
    {
        if (!_battleCharacterController.CanSelectAbility())
            return;

        // TODO: hardcoded ability indexes
        _buttonClickQueue.Enqueue(HandleButtonClick(_selectedPlayerStats.BasicAbilities[1]));
    }

    void DButtonClicked()
    {
        if (!_battleCharacterController.CanSelectAbility())
            return;

        // TODO: hardcoded ability indexes
        OpenInventory();
    }

    void QButtonClicked()
    {
        if (!_battleCharacterController.CanSelectAbility())
            return;

        // TODO: hardcoded ability indexes
        if (_selectedPlayerStats.Abilities[0] == null)
            return;

        _buttonClickQueue.Enqueue(HandleButtonClick(_selectedPlayerStats.Abilities[0]));
    }

    void WButtonClicked()
    {
        if (!_battleCharacterController.CanSelectAbility())
            return;

        // TODO: hardcoded ability indexes
        if (_selectedPlayerStats.Abilities[1] == null)
            return;

        _buttonClickQueue.Enqueue(HandleButtonClick(_selectedPlayerStats.Abilities[1]));
    }

    void EButtonClicked()
    {
        if (!_battleCharacterController.CanSelectAbility())
            return;

        // TODO: hardcoded ability indexes
        if (_selectedPlayerStats.Abilities[2] == null)
            return;

        _buttonClickQueue.Enqueue(HandleButtonClick(_selectedPlayerStats.Abilities[2]));
    }

    void RButtonClicked()
    {
        if (!_battleCharacterController.CanSelectAbility())
            return;

        // TODO: hardcoded ability indexes
        if (_selectedPlayerStats.Abilities[3] == null)
            return;

        _buttonClickQueue.Enqueue(HandleButtonClick(_selectedPlayerStats.Abilities[3]));
    }

    // TODO: Hey future Bernard, I know you are looking at this and thinking: "damn... mixing coroutines and async await sucks, what was I thinking"
    // I, past Bernard would like to tell you that I tried hard to make it work and I left coroutines and async await not because I don't like you but
    // because I am not skilled enough to rewrite everything to use only async await. 
    IEnumerator HandleButtonClick(Ability ability)
    {
        _battleCharacterController.SelectedCharacter.GetComponent<FaceDirectionUI>().HideUI();

        ShowAbilityTooltip(ability);

        Task task = ability.HighlightTargetable(_battleCharacterController.SelectedCharacter); // for some reason this works, but it has to be written exactly like that with Task task = ;
        yield return new WaitUntil(() => task.IsCompleted);

        _battleCharacterController.SetSelectedAbility(ability);
    }

    void OpenInventory()
    {
        _battleCharacterController.SelectedCharacter.GetComponent<FaceDirectionUI>().HideUI();
        InventoryUI.instance.EnableInventoryUI();
        // then player selects item and I can queue ability with handle button click... sounds a bit convoluted
        // I probably need to disable game controlls and enable inventory controls.
    }

    public void UseItem(Item item)
    {
        item.Ability.Initialize(_battleCharacterController.SelectedCharacter);
        _buttonClickQueue.Enqueue(HandleButtonClick(item.Ability));
    }

    void ShowAbilityTooltip(Ability ability)
    {
        _characterUITooltipContainer.style.display = DisplayStyle.Flex;

        _characterUITooltipAbilityName.text = Helpers.ParseScriptableObjectCloneName(ability.name);
        _characterUITooltipAbilityDescription.text = ability.Description;
        _characterUITooltipAbilityManaCost.text = "Mana cost: " + ability.ManaCost;

        _characterUITooltipModifierDescription.style.display = DisplayStyle.Flex;
        // display modifier & status description
        if (ability.StatModifier != null)
            _characterUITooltipModifierDescription.text = ability.StatModifier.GetDescription();
        else
            _characterUITooltipModifierDescription.style.display = DisplayStyle.None;

        _characterUITooltipStatusDescription.style.display = DisplayStyle.Flex;
        if (ability.Status != null)
            _characterUITooltipStatusDescription.text = ability.Status.GetDescription();
        else
            _characterUITooltipStatusDescription.style.display = DisplayStyle.None;

    }

    public void HideAbilityTooltip()
    {
        _characterUITooltipContainer.style.display = DisplayStyle.None;
    }

    public void ShowCharacterUI(CharacterStats playerStats)
    {
        _characterUIContainer.style.display = DisplayStyle.Flex;

        InfoCardUI.instance.HideCharacterCard();

        // current character is not in the scene, keep that in mind. It's a static scriptable object.
        _selectedPlayerStats = playerStats;

        _characterUICharacterCard.Clear();
        CharacterCardVisual visual = new(playerStats.Character);
        _characterUICharacterCard.Add(visual);
        visual.HealthBar.DisplayMissingAmount(playerStats.MaxHealth.GetValue(), playerStats.CurrentHealth);
        visual.ManaBar.DisplayMissingAmount(playerStats.MaxMana.GetValue(), playerStats.CurrentMana);

        HandleAbilityButtons();
        DisableSkillButtons();
        EnableSkillButtons();

        DOTween.Kill(_hideCharacterUIID);
        DOTween.To(() => _characterUI.style.top.value.value, x => _characterUI.style.top = Length.Percent(x), _characterUIShowValue, 0.5f)
            .SetEase(Ease.InOutSine);
    }

    public void HideCharacterUI()
    {
        HideAbilityTooltip();
        _selectedPlayerStats = null;
        DOTween.To(() => _characterUI.style.top.value.value, x => _characterUI.style.top = Length.Percent(x), _characterUIHideValue, 0.5f)
            .SetEase(Ease.InOutSine)
            .SetId(_hideCharacterUIID)
            .OnComplete(() => _characterUIContainer.style.display = DisplayStyle.None);
    }

    void SetCharacterHealthMana(CharacterStats playerStats)
    {
        _characterHealth.text = playerStats.CurrentHealth + "/" + playerStats.MaxHealth.GetValue();
        _characterMana.text = playerStats.CurrentMana + "/" + playerStats.MaxMana.GetValue();

        // (float) casts are NOT redundant, without them it does not work
        float missingHealthPerc = ((float)playerStats.MaxHealth.GetValue() - (float)playerStats.CurrentHealth) / (float)playerStats.MaxHealth.GetValue();
        float missingManaPerc = ((float)playerStats.MaxMana.GetValue() - (float)playerStats.CurrentMana) / (float)playerStats.MaxMana.GetValue();

        _characterHealthBarMissingHealth.style.width = Length.Percent(missingHealthPerc * 100);
        _characterManaBarMissingMana.style.width = Length.Percent(missingManaPerc * 100);
    }

    void SetCharacteristics(CharacterStats stats)
    {
        _characterStrength.text = "" + stats.Strength.GetValue();
        _characterIntelligence.text = "" + stats.Intelligence.GetValue();
        _characterAgility.text = "" + stats.Agility.GetValue();
        _characterStamina.text = "" + stats.Stamina.GetValue();
        _characterArmor.text = "" + stats.Armor.GetValue();
        _characterRange.text = "" + stats.MovementRange.GetValue();

        BattleUIHelpers.HandleStatCheck(stats.Strength, _characterStrength);
        BattleUIHelpers.HandleStatCheck(stats.Intelligence, _characterIntelligence);
        BattleUIHelpers.HandleStatCheck(stats.Agility, _characterAgility);
        BattleUIHelpers.HandleStatCheck(stats.Stamina, _characterStamina);
        BattleUIHelpers.HandleStatCheck(stats.Armor, _characterArmor);
        BattleUIHelpers.HandleStatCheck(stats.MovementRange, _characterRange);

        _modifierContainer.Clear();
        List<VisualElement> elements = new(BattleUIHelpers.HandleStatModifiers(stats));
        elements.AddRange(BattleUIHelpers.HandleStatuses(stats));
        foreach (VisualElement el in elements)
            _modifierContainer.Add(el);
    }

    void HandleAbilityButtons()
    {
        // TODO: hardcoded ability indexes
        _characterASkillIcon.style.backgroundImage = _selectedPlayerStats.BasicAbilities[0].Icon.texture;
        _characterSSkillIcon.style.backgroundImage = _selectedPlayerStats.BasicAbilities[1].Icon.texture;

        for (int i = 0; i < _abilityButtons.Count; i++)
        {
            if (_selectedPlayerStats.Abilities.Count <= i)
            {
                _abilityButtons[i].style.display = DisplayStyle.None;
                continue;
            }

            // show buttons for each ability
            _abilityButtons[i].style.display = DisplayStyle.Flex;
            _abilityIcons[i].style.backgroundImage = _selectedPlayerStats.Abilities[i].Icon.texture;
        }
    }

    public void DisableSkillButtons()
    {
        _characterAButton.SetEnabled(false);
        _characterSButton.SetEnabled(false);
        _characterDButton.SetEnabled(false);

        _characterQButton.SetEnabled(false);
        _characterWButton.SetEnabled(false);
        _characterEButton.SetEnabled(false);
        _characterRButton.SetEnabled(false);
    }

    public void EnableSkillButtons()
    {
        if (_selectedPlayerStats == null)
            return;

        // costless actions
        _characterAButton.SetEnabled(true);
        _characterSButton.SetEnabled(true);
        _characterDButton.SetEnabled(true);

        // enable buttons if they are populated
        // && player has enough mana to cast ability;
        // && weapon type matches
        for (int i = 0; i < _abilityButtons.Count; i++)
        {
            if (_abilityButtons[i].style.display == DisplayStyle.None)
                continue;
            if (_selectedPlayerStats.Abilities[i].ManaCost >= _selectedPlayerStats.CurrentMana)
                continue;
            if (_selectedPlayerStats.Abilities[i].WeaponType != _selectedPlayerStats.Character.Weapon.WeaponType && _selectedPlayerStats.Abilities[i].WeaponType != WeaponType.Any)
                continue;

            _abilityButtons[i].SetEnabled(true);
        }
    }

    // called by infocardUI
    public void ShowManaUse(int val)
    {
        if (_selectedPlayerStats == null)
            return;

        float currentMana = (float)_selectedPlayerStats.CurrentMana;
        float manaAfterInteraction = (float)_selectedPlayerStats.CurrentMana - val;
        manaAfterInteraction = Mathf.Clamp(manaAfterInteraction, 0, _selectedPlayerStats.CurrentMana);

        // text
        _characterMana.text = manaAfterInteraction + "/" + _selectedPlayerStats.MaxMana.GetValue();

        // bar
        float result = val / (float)_selectedPlayerStats.MaxMana.GetValue();

        if (manaAfterInteraction == 0)
            result = currentMana / (float)_selectedPlayerStats.MaxMana.GetValue();

        _characterManaBarInteractionResult.style.display = DisplayStyle.Flex;
        // reset right
        _characterManaBarInteractionResult.style.right = Length.Percent(0);
        _characterManaBarInteractionResult.style.width = Length.Percent(result * 100);

        // "animate it"
        AnimateManaUse();
    }

    public void HideManaUse()
    {
        DOTween.Pause(_manaUseTweenID);

        _characterManaBarInteractionResult.style.width = Length.Percent(0);
        if (_selectedPlayerStats != null)
            SetCharacterHealthMana(_selectedPlayerStats);

    }

    void AnimateManaUse()
    {
        _characterManaBarInteractionResult.style.backgroundColor = _damageBarColor;

        DOTween.ToAlpha(() => _characterManaBarInteractionResult.style.backgroundColor.value,
                         x => _characterManaBarInteractionResult.style.backgroundColor = x,
                         0f, 0.8f).SetLoops(-1, LoopType.Yoyo)
                         .SetId(_manaUseTweenID);
    }

    // called by infocardUI
    public void ShowDamage(int val)
    {
        if (_selectedPlayerStats == null)
            return;

        _characterPortraitSkull.style.display = DisplayStyle.None;

        float currentHealth = (float)_selectedPlayerStats.CurrentHealth;
        float healthAfterInteraction = (float)_selectedPlayerStats.CurrentHealth - val;
        healthAfterInteraction = Mathf.Clamp(healthAfterInteraction, 0, _selectedPlayerStats.CurrentHealth);

        // text
        _characterHealth.text = healthAfterInteraction + "/" + _selectedPlayerStats.MaxHealth.GetValue();

        // bar
        float result = val / (float)_selectedPlayerStats.MaxHealth.GetValue();

        if (healthAfterInteraction == 0)
            result = currentHealth / (float)_selectedPlayerStats.MaxHealth.GetValue();

        _characterHealthBarRetaliationResult.style.display = DisplayStyle.Flex;
        // reset right
        _characterHealthBarRetaliationResult.style.right = Length.Percent(0);
        _characterHealthBarRetaliationResult.style.width = Length.Percent(result * 100);

        // death
        if (healthAfterInteraction <= 0)
            _characterPortraitSkull.style.display = DisplayStyle.Flex;

        // "animate it"
        AnimateInteractionResult(_damageBarColor);
    }

    public void ShowHeal(int val)
    {
        if (_selectedPlayerStats == null)
            return;

        float currentHealth = (float)_selectedPlayerStats.CurrentHealth;
        float maxHealth = (float)_selectedPlayerStats.MaxHealth.GetValue();

        // if there is nothing to heal, don't show the result
        if (currentHealth >= maxHealth)
            return;

        float healthAfterInteraction = (float)currentHealth + val;
        healthAfterInteraction = Mathf.Clamp(healthAfterInteraction, 0, maxHealth);

        // text
        _characterHealth.text = healthAfterInteraction + "/" + maxHealth;

        // bar
        float result = val / (float)maxHealth;
        result = Mathf.Clamp(result, 0, 1);

        _characterHealthBarRetaliationResult.style.display = DisplayStyle.Flex;
        // move it left, to show that it is health gain not loss.
        _characterHealthBarRetaliationResult.style.right = Length.Percent(result * 100);
        _characterHealthBarRetaliationResult.style.width = Length.Percent(result * 100);

        // "animate it"
        AnimateInteractionResult(_healBarColor);
    }

    public void HideDamage()
    {
        DOTween.Pause(_healthLostTweenID);

        _characterHealthBarRetaliationResult.style.width = Length.Percent(0);
        if (_selectedPlayerStats != null)
            SetCharacterHealthMana(_selectedPlayerStats);
    }

    void AnimateInteractionResult(Color col)
    {
        _characterHealthBarRetaliationResult.style.backgroundColor = col;

        DOTween.ToAlpha(() => _characterHealthBarRetaliationResult.style.backgroundColor.value,
                         x => _characterHealthBarRetaliationResult.style.backgroundColor = x,
                         0f, 0.8f).SetLoops(-1, LoopType.Yoyo)
                         .SetId(_healthLostTweenID);
    }

    // https://answers.unity.com/questions/1590871/how-to-stack-coroutines-and-call-each-one-till-all.html
    IEnumerator CoroutineCoordinator()
    {
        while (true)
        {
            while (_buttonClickQueue.Count > 0)
            {
                // block input when the queue is not 0;
                _wasClickEnqueued = true;
                _battleInputController.SetInputAllowed(false);

                yield return StartCoroutine(_buttonClickQueue.Dequeue());
            }
            // set input allowed to true only once when the queue is emptied;
            if (_wasClickEnqueued)
            {
                _wasClickEnqueued = false;
                _battleInputController.SetInputAllowed(true);
            }

            yield return null;
        }
    }

    // for keyboard input
    public void SimulateAButtonClicked()
    {
        using (var e = new NavigationSubmitEvent() { target = _characterAButton })
            _characterAButton.SendEvent(e);
    }

    public void SimulateSButtonClicked()
    {
        using (var e = new NavigationSubmitEvent() { target = _characterSButton })
            _characterSButton.SendEvent(e);
    }

    public void SimulateDButtonClicked()
    {
        using (var e = new NavigationSubmitEvent() { target = _characterDButton })
            _characterDButton.SendEvent(e);
    }


    public void SimulateQButtonClicked()
    {
        // https://forum.unity.com/threads/trigger-button-click-from-code.1124356/
        using (var e = new NavigationSubmitEvent() { target = _characterQButton })
            _characterQButton.SendEvent(e);
    }

    public void SimulateWButtonClicked()
    {
        using (var e = new NavigationSubmitEvent() { target = _characterWButton })
            _characterWButton.SendEvent(e);
    }

    public void SimulateEButtonClicked()
    {
        using (var e = new NavigationSubmitEvent() { target = _characterEButton })
            _characterEButton.SendEvent(e);
    }

    public void SimulateRButtonClicked()
    {
        using (var e = new NavigationSubmitEvent() { target = _characterRButton })
            _characterRButton.SendEvent(e);
    }

}
