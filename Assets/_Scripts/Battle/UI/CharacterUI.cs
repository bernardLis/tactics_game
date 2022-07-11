using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine.InputSystem;

public class CharacterUI : Singleton<CharacterUI>
{
    // global utility
    BattleInputController _battleInputController;
    BattleCharacterController _battleCharacterController;

    // UI Elements
    VisualElement _root;
    VisualElement _container;
    VisualElement _characterCardContainer;
    CharacterCardVisual _characterCardVisual;

    VisualElement _abilityTooltipContainer;

    Button _openInventoryButton;
    VisualElement _characterAbilitiesContainer;

    // local
    CharacterStats _selectedPlayerStats;

    // animate ui up/down on show/hide
    float _UIShowValue = 0f;
    float _UIHideValue = -22f;

    // buttons management
    Queue<IEnumerator> _buttonClickQueue = new();
    bool _areButtonEnabled;
    bool _wasClickEnqueued;

    string _hideCharacterUIID = "hideCharacterUIID";

    protected override void Awake()
    {
        base.Awake();

        // getting ui elements
        _root = GetComponent<UIDocument>().rootVisualElement;

        _container = _root.Q<VisualElement>("characterUIContainer");
        _characterCardContainer = _root.Q<VisualElement>("characterUICharacterCard");

        _abilityTooltipContainer = _root.Q<VisualElement>("abilityTooltipContainer");

        _openInventoryButton = _root.Q<Button>("openInventory");
        _openInventoryButton.clickable.clicked += OpenInventoryClicked;

        _characterAbilitiesContainer = _root.Q<VisualElement>("characterAbilities");
        _characterAbilitiesContainer.Clear();
    }

    void Start()
    {
        _battleInputController = BattleInputController.Instance;
        _battleCharacterController = BattleCharacterController.Instance;

        MovePointController.OnMove += MovePointController_OnMove;
        BattleCharacterController.OnCharacterStateChanged += BattleCharacterController_OnCharacterStateChange;

        //https://answers.unity.com/questions/1590871/how-to-stack-coroutines-and-call-each-one-till-all.html
        StartCoroutine(CoroutineCoordinator());
    }

    void OnDestroy()
    {
        MovePointController.OnMove -= MovePointController_OnMove;
        BattleCharacterController.OnCharacterStateChanged -= BattleCharacterController_OnCharacterStateChange;
    }

    void MovePointController_OnMove(Vector3 pos)
    {
        ResolveManaChange();
    }

    void BattleCharacterController_OnCharacterStateChange(CharacterState state)
    {
        if (state == CharacterState.None)
            HandleCharacterStateNone();
        if (state == CharacterState.Selected)
            HandleCharacterSelected();
        if (state == CharacterState.SelectingInteractionTarget)
            return;
        if (state == CharacterState.SelectingFaceDir)
            HandleSelectingFaceDir();
        if (state == CharacterState.ConfirmingInteraction)
            return;
    }
    async void HandleCharacterStateNone()
    {
        _selectedPlayerStats.OnAbilityAdded -= OnAbilityAdded;
        _selectedPlayerStats = null;

        await HideCharacterUI();
    }

    async void HandleCharacterSelected()
    {
        _selectedPlayerStats = _battleCharacterController.SelectedCharacter.GetComponent<PlayerStats>();
        _selectedPlayerStats.OnAbilityAdded += OnAbilityAdded;

        await ShowCharacterUI();
        HideAbilityTooltip();
    }

    void HandleSelectingFaceDir()
    {
        DisableSkillButtons();
    }

    void ResolveManaChange()
    {
        HideManaChange();

        if (_battleCharacterController.SelectedAbility == null)
            return;
        Ability selectedAbility = _battleCharacterController.SelectedAbility;

        // mana use
        if (selectedAbility.ManaCost != 0)
            ShowManaChange(-1 * selectedAbility.ManaCost);
    }

    async Task ShowCharacterUI()
    {
        _characterCardContainer.Clear();
        _characterCardVisual = new(_selectedPlayerStats);
        _characterCardContainer.Add(_characterCardVisual);

        HandleAbilityButtons();
        DisableSkillButtons();

        DOTween.Kill(_hideCharacterUIID);
        await DOTween.To(() => _container.style.bottom.value.value, x => _container.style.bottom = Length.Percent(x),
                         _UIShowValue, 0.5f)
                    .SetEase(Ease.InOutSine).AsyncWaitForCompletion();

        EnableSkillButtons();
    }

    async void OnAbilityAdded(Ability ability)
    {
        await HideCharacterUI();
        await ShowCharacterUI();
    }

    public async Task HideCharacterUI()
    {
        HideAbilityTooltip();

        await DOTween.To(() => _container.style.bottom.value.value, x => _container.style.bottom = Length.Percent(x),
                         _UIHideValue, 0.5f)
                .SetEase(Ease.InOutSine)
                .SetId(_hideCharacterUIID).AsyncWaitForCompletion();
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

    void HandleAbilityButtons()
    {
        // TODO: I think that the idea with buttons remembering their input key is not so good
        // but I don't have any other ideas... maybe in the future I will come up with something
        // it's for simulating button clicks with the keyboard;
        string[] buttons = { "Q", "W", "E", "R" };

        AbilityButton basicAttack = new(_selectedPlayerStats.BasicAbilities[0], "A", _root);
        basicAttack.RegisterCallback<ClickEvent>(ev => AbilityButtonClicked(basicAttack));

        AbilityButton basicDefend = new(_selectedPlayerStats.BasicAbilities[1], "S", _root);
        basicDefend.RegisterCallback<ClickEvent>(ev => AbilityButtonClicked(basicDefend));

        _characterAbilitiesContainer.Clear();
        _characterAbilitiesContainer.Add(basicAttack);
        _characterAbilitiesContainer.Add(basicDefend);

        for (int i = 0; i < _selectedPlayerStats.Abilities.Count; i++)
        {
            AbilityButton button = new(_selectedPlayerStats.Abilities[i], buttons[i], _root);
            button.RegisterCallback<ClickEvent>(ev => AbilityButtonClicked(button));

            _characterAbilitiesContainer.Add(button);
        }
    }

    void AbilityButtonClicked(AbilityButton button)
    {
        if (!_battleInputController.IsInputAllowed())
            return;

        if (!_battleCharacterController.CanSelectAbility())
            return;

        if (button.Ability.ManaCost > _selectedPlayerStats.CurrentMana)
            return;

        _buttonClickQueue.Enqueue(HandleButtonClick(button.Ability));
    }

    public void DisableSkillButtons()
    {
        _areButtonEnabled = false;

        foreach (var el in _characterAbilitiesContainer.Children())
            el.SetEnabled(false);
    }

    public void EnableSkillButtons()
    {
        if (_characterAbilitiesContainer.childCount == 0)
            return;

        if (_container.style.bottom.value.value < _UIShowValue)
            return;

        _areButtonEnabled = true;

        foreach (var el in _characterAbilitiesContainer.Children())
        {
            AbilityButton ab = (AbilityButton)el;
            if (ab.Ability.ManaCost > _selectedPlayerStats.CurrentMana)
                continue;
            if (ab.Ability.WeaponType != _selectedPlayerStats.Character.Weapon.WeaponType && ab.Ability.WeaponType != WeaponType.Any)
                continue;

            ab.SetEnabled(true);
        }
    }

    void ShowAbilityTooltip(Ability ability)
    {
        _abilityTooltipContainer.style.display = DisplayStyle.Flex;
        _abilityTooltipContainer.Clear();

        AbilityTooltipVisual visual = new(ability);
        _abilityTooltipContainer.Add(visual);
    }

    public void HideAbilityTooltip()
    {
        _abilityTooltipContainer.style.display = DisplayStyle.None;
    }

    void OpenInventoryClicked()
    {
        if (!_battleCharacterController.CanSelectAbility())
            return;
        if (!_areButtonEnabled)
            return;

        OpenInventory();
    }

    void OpenInventory()
    {
        _battleCharacterController.SelectedCharacter.GetComponent<FaceDirectionUI>().HideUI();
        InventoryUI.Instance.EnableInventoryUI();
    }

    public void UseItem(Item item)
    {
        item.Ability.Initialize(_battleCharacterController.SelectedCharacter);
        _buttonClickQueue.Enqueue(HandleButtonClick(item.Ability));
    }

    // TODO: Hey future Bernard, I know you are looking at this and thinking: "damn... mixing coroutines and async await sucks, what was I thinking"
    // I, past Bernard would like to tell you that I tried hard to make it work and I left coroutines and async await not because I don't like you but
    // because I am not skilled enough to rewrite everything to use only async await. 
    IEnumerator HandleButtonClick(Ability ability)
    {
        _battleCharacterController.SelectedCharacter.GetComponent<FaceDirectionUI>().HideUI();

        ShowAbilityTooltip(ability);

        // when clicked multiple times on the same ability treat it as if clicked select
        if (ability == _battleCharacterController.SelectedAbility)
        {
            MovePointController.Instance.HandleSelectClick();
            yield break;
        }

        // TODO: this eats errors
        _battleInputController.SetInputAllowed(false);
        Task task = ability.HighlightTargetable(_battleCharacterController.SelectedCharacter); // for some reason this works, but it has to be written exactly like that with Task task = ;
        yield return new WaitUntil(() => task.IsCompleted);
        _battleInputController.SetInputAllowed(true);

        _battleCharacterController.SetSelectedAbility(ability);
        _battleCharacterController.GetViableTargets();
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

    /* Keyboard input */
    public void SimulateOpenInventoryClicked()
    {
        using (var e = new NavigationSubmitEvent() { target = _openInventoryButton })
            _openInventoryButton.SendEvent(e);
    }

    public void SimulateAbilityButtonClicked(InputAction.CallbackContext ctx)
    {
        if (_characterAbilitiesContainer.childCount == 0)
            return;

        foreach (var el in _characterAbilitiesContainer.Children())
        {
            AbilityButton ab = (AbilityButton)el;
            if (ab.Key == ctx.control.name.ToUpper())
            {
                // https://forum.unity.com/threads/trigger-button-click-from-code.1124356/
                using (var e = new NavigationSubmitEvent() { target = ab })
                    ab.SendEvent(e);

                AbilityButtonClicked(ab);
            }
        }
    }
}
