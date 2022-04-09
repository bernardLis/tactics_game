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
    VisualElement _container;
    VisualElement _characterCardContainer;
    CharacterCardVisual _characterCardVisual;

    VisualElement _tooltipContainer;
    Label _tooltipAbilityName;
    Label _tooltipAbilityDescription;
    Label _tooltipAbilityManaCost;
    Label _tooltipModifierDescription;
    Label _tooltipStatusDescription;

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
    float _UIShowValue = 0f;
    float _UIHideValue = -20f;//20f;

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

        _container = root.Q<VisualElement>("characterUIContainer");
        _characterCardContainer = root.Q<VisualElement>("characterUICharacterCard");

        _tooltipContainer = root.Q<VisualElement>("characterUITooltipContainer");
        _tooltipAbilityName = root.Q<Label>("characterUITooltipAbilityName");
        _tooltipAbilityDescription = root.Q<Label>("characterUITooltipAbilityDescription");
        _tooltipAbilityManaCost = root.Q<Label>("characterUITooltipAbilityManaCost");
        _tooltipModifierDescription = root.Q<Label>("characterUITooltipModifierDescription");
        _tooltipStatusDescription = root.Q<Label>("characterUITooltipStatusDescription");

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
        _tooltipContainer.style.display = DisplayStyle.Flex;

        _tooltipAbilityName.text = Helpers.ParseScriptableObjectCloneName(ability.name);
        _tooltipAbilityDescription.text = ability.Description;
        _tooltipAbilityManaCost.text = "Mana cost: " + ability.ManaCost;

        _tooltipModifierDescription.style.display = DisplayStyle.Flex;
        // display modifier & status description
        if (ability.StatModifier != null)
            _tooltipModifierDescription.text = ability.StatModifier.GetDescription();
        else
            _tooltipModifierDescription.style.display = DisplayStyle.None;

        _tooltipStatusDescription.style.display = DisplayStyle.Flex;
        if (ability.Status != null)
            _tooltipStatusDescription.text = ability.Status.GetDescription();
        else
            _tooltipStatusDescription.style.display = DisplayStyle.None;
    }

    public void HideAbilityTooltip()
    {
        _tooltipContainer.style.display = DisplayStyle.None;
    }


    public void ShowCharacterUI(CharacterStats playerStats)
    {
        InfoCardUI.instance.HideCharacterCard();

        _selectedPlayerStats = playerStats;

        _characterCardContainer.Clear();
        _characterCardVisual = new(playerStats);
        _characterCardContainer.Add(_characterCardVisual);

        HandleAbilityButtons();
        DisableSkillButtons();
        EnableSkillButtons();

        DOTween.Kill(_hideCharacterUIID);
        DOTween.To(() => _container.style.bottom.value.value, x => _container.style.bottom = Length.Percent(x), 
                         _UIShowValue, 0.5f)
                    .SetEase(Ease.InOutSine);
    }

    public void HideCharacterUI()
    {
        HideAbilityTooltip();
        _selectedPlayerStats = null;
        DOTween.To(() => _container.style.bottom.value.value, x => _container.style.bottom = Length.Percent(x), 
                         _UIHideValue, 0.5f)
                .SetEase(Ease.InOutSine)
                .SetId(_hideCharacterUIID);
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
