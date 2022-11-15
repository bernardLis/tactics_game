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

    public Ability BasicDefend;

    // UI Elements
    VisualElement _root;
    VisualElement _container;
    VisualElement _characterCardContainer;
    CharacterCard _characterCardVisual;

    VisualElement _characterAbilitiesContainer;
    VisualElement _basicActionContainer;
    List<AbilitySlotVisual> _basicActionSlots = new();
    List<AbilitySlotVisual> _abilitySlots = new();
    List<AbilityButton> _allButtons = new();

    // local
    CharacterStats _selectedPlayerStats;

    // animate ui up/down on show/hide
    float _UIShowValue = 0;
    float _UIHideValue = -200;

    // buttons management
    Queue<IEnumerator> _buttonClickQueue = new();
    bool _wasClickEnqueued;

    string _hideCharacterUIID = "hideCharacterUIID";

    protected override void Awake()
    {
        base.Awake();

        // getting ui elements
        _root = GetComponent<UIDocument>().rootVisualElement;

        _container = _root.Q<VisualElement>("characterUIContainer");
        _characterCardContainer = _root.Q<VisualElement>("characterUICharacterCard");
    }

    void Start()
    {
        _battleInputController = BattleInputController.Instance;
        _battleCharacterController = BattleCharacterController.Instance;

        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;
        MovePointController.OnMove += MovePointController_OnMove;
        BattleCharacterController.OnCharacterStateChanged += BattleCharacterController_OnCharacterStateChange;

        AddAbilitySlots();

        //https://answers.unity.com/questions/1590871/how-to-stack-coroutines-and-call-each-one-till-all.html
        StartCoroutine(CoroutineCoordinator());
    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
        MovePointController.OnMove -= MovePointController_OnMove;
        BattleCharacterController.OnCharacterStateChanged -= BattleCharacterController_OnCharacterStateChange;
    }

    async void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state == BattleState.Won || state == BattleState.Lost)
            await HideCharacterUI();

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
    }
    async void HandleCharacterStateNone()
    {
        if (_selectedPlayerStats == null)
            return;
        _selectedPlayerStats.OnAbilityAdded -= OnAbilityAdded;
        _selectedPlayerStats.OnCharacterDeath -= OnCharacterDeath;
        _selectedPlayerStats = null;

        await HideCharacterUI();
    }

    async void HandleCharacterSelected()
    {
        _selectedPlayerStats = _battleCharacterController.SelectedCharacter.GetComponent<PlayerStats>();
        _selectedPlayerStats.OnAbilityAdded += OnAbilityAdded;
        _selectedPlayerStats.OnCharacterDeath += OnCharacterDeath;

        await ShowCharacterUI();
    }

    void AddAbilitySlots()
    {
        VisualElement basicActionContainer = _root.Q<VisualElement>("basicActionContainer");
        basicActionContainer.Clear();
        for (int i = 0; i < 2; i++)
        {
            AbilitySlotVisual abilitySlot = new();
            basicActionContainer.Add(abilitySlot);
            _basicActionSlots.Add(abilitySlot);
        }

        VisualElement abilityContainer = _root.Q<VisualElement>("abilityContainer");
        abilityContainer.Clear();
        for (int i = 0; i < 2; i++)
        {
            AbilitySlotVisual abilitySlot = new();
            abilityContainer.Add(abilitySlot);
            _abilitySlots.Add(abilitySlot);
        }

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
        await DOTween.To(() => _container.style.bottom.value.value, x => _container.style.bottom = x,
                         _UIShowValue, 0.5f)
                    .SetEase(Ease.InOutSine).AsyncWaitForCompletion();

        EnableSkillButtons();
    }

    async void OnAbilityAdded(Ability ability)
    {
        // if character is pushed into the place of it will throw an error without this if
        if (_battleCharacterController.CharacterState == CharacterState.Selected
            && _battleCharacterController.SelectedCharacter == _selectedPlayerStats.gameObject)
        {
            await HideCharacterUI();
            await ShowCharacterUI();
        }
    }

    async void OnCharacterDeath(GameObject obj)
    {
        await HideCharacterUI();
    }

    public async Task HideCharacterUI()
    {
        await DOTween.To(() => _container.style.bottom.value.value, x => _container.style.bottom = x,
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

        _characterCardVisual.HealthBar.HideInteractionResult();
    }

    public void HideManaChange()
    {
        if (_selectedPlayerStats == null)
            return;

        _characterCardVisual.ManaBar.HideInteractionResult();
    }

    void HandleAbilityButtons()
    {
        _allButtons.Clear();
        foreach (AbilitySlotVisual slot in _basicActionSlots)
            slot.RemoveButton();
        foreach (AbilitySlotVisual slot in _abilitySlots)
            slot.RemoveButton();

        // TODO: I think that the idea with buttons remembering their input key is not so good
        // but I don't have any other ideas... maybe in the future I will come up with something
        // it's for simulating button clicks with the keyboard;
        string[] buttons = { "Q", "W", "E", "R" };

        AbilityButton basicAttack = new(_selectedPlayerStats.BasicAttack, "A");
        basicAttack.RegisterCallback<ClickEvent>(ev => AbilityButtonClicked(basicAttack));
        _allButtons.Add(basicAttack);

        AbilityButton basicDefend = new(BasicDefend, "S");
        basicDefend.RegisterCallback<ClickEvent>(ev => AbilityButtonClicked(basicDefend));
        _allButtons.Add(basicDefend);

        _basicActionSlots[0].AddButton(basicAttack);
        _basicActionSlots[1].AddButton(basicDefend);

        for (int i = 0; i < _selectedPlayerStats.Abilities.Count; i++)
        {
            AbilityButton button = new(_selectedPlayerStats.Abilities[i], buttons[i]);
            button.RegisterCallback<ClickEvent>(ev => AbilityButtonClicked(button));
            _abilitySlots[i].AddButton(button);
            _allButtons.Add(button);
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
        foreach (AbilityButton button in _allButtons)
            button.SetEnabled(false);
    }

    public void EnableSkillButtons()
    {
        if (_container.style.bottom.value.value < _UIShowValue)
            return;
        foreach (AbilityButton button in _allButtons)
        {
            if (button.Ability.ManaCost > _selectedPlayerStats.CurrentMana)
                continue;
            if (button.Ability.WeaponType != _selectedPlayerStats.Character.Weapon.WeaponType && button.Ability.WeaponType != WeaponType.Any)
                continue;

            button.SetEnabled(true);
        }
    }

    // TODO: Hey future Bernard, I know you are looking at this and thinking: "damn... mixing coroutines and async await sucks, what was I thinking"
    // I, past Bernard would like to tell you that I have tried to make something better and I left coroutines and async await not because I don't like you but
    // because I am not skilled enough to rewrite everything to use only async await. 
    IEnumerator HandleButtonClick(Ability ability)
    {
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
    public void SimulateAbilityButtonClicked(InputAction.CallbackContext ctx)
    {
        foreach (AbilityButton button in _allButtons)
        {
            if (button.Key == ctx.control.name.ToUpper())
            {
                // https://forum.unity.com/threads/trigger-button-click-from-code.1124356/
                using (var e = new NavigationSubmitEvent() { target = button })
                    button.SendEvent(e);

                AbilityButtonClicked(button);
            }
        }
    }
}
