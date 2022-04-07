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

    CharacterCardVisual _characterCardVisual;

    VisualElement _characterUITooltipContainer;
    Label _characterUITooltipAbilityName;
    Label _characterUITooltipAbilityDescription;
    Label _characterUITooltipAbilityManaCost;
    Label _characterUITooltipModifierDescription;
    Label _characterUITooltipStatusDescription;

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

    string _hideCharacterUIID = "hideCharacterUIID";

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
        _characterCardVisual = new(playerStats);
        _characterUICharacterCard.Add(_characterCardVisual);
        _characterCardVisual.HealthBar.DisplayMissingAmount(playerStats.MaxHealth.GetValue(), playerStats.CurrentHealth);
        _characterCardVisual.ManaBar.DisplayMissingAmount(playerStats.MaxMana.GetValue(), playerStats.CurrentMana);

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

    public void ShowHealthChange(int val)
    {
        if (_selectedPlayerStats == null)
            return;

        _characterCardVisual.HealthBar.DisplayInteractionResult(_selectedPlayerStats.MaxHealth.GetValue(),
                                                                _selectedPlayerStats.CurrentHealth,
                                                                val);
    }

    public void ShowManaChange(int val)
    {
        if (_selectedPlayerStats == null)
            return;

        _characterCardVisual.ManaBar.DisplayInteractionResult(_selectedPlayerStats.MaxMana.GetValue(),
                                                                _selectedPlayerStats.CurrentMana,
                                                                val);
    }

    public void HideHealthChange()
    {
        if (_selectedPlayerStats == null)
            return;

        _characterCardVisual.HealthBar.HideInteractionResult(_selectedPlayerStats.MaxHealth.GetValue(),
                                                            _selectedPlayerStats.CurrentHealth);
    }

    public void HideManaChange()
    {
        if (_selectedPlayerStats == null)
            return;

        _characterCardVisual.ManaBar.HideInteractionResult(_selectedPlayerStats.MaxMana.GetValue(),
                                                            _selectedPlayerStats.CurrentMana);
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
