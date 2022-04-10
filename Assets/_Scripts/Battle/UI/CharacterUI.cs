using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine.InputSystem;

public class CharacterUI : MonoBehaviour
{
    // global utility
    BattleInputController _battleInputController;
    BattleCharacterController _battleCharacterController;

    // UI Elements
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
    float _UIHideValue = -20f;

    // buttons management
    Queue<IEnumerator> _buttonClickQueue = new();
    bool _areButtonEnabled;
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

        _abilityTooltipContainer = root.Q<VisualElement>("abilityTooltipContainer");

        _openInventoryButton = root.Q<Button>("openInventory");
        _openInventoryButton.clickable.clicked += OpenInventoryClicked;

        _characterAbilitiesContainer = root.Q<VisualElement>("characterAbilities");
        _characterAbilitiesContainer.Clear();
    }

    void Start()
    {
        _battleInputController = BattleInputController.instance;
        _battleCharacterController = BattleCharacterController.instance;

        //https://answers.unity.com/questions/1590871/how-to-stack-coroutines-and-call-each-one-till-all.html
        StartCoroutine(CoroutineCoordinator());
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

        AbilityButton basicAttack = new(_selectedPlayerStats.BasicAbilities[0], "A");
        basicAttack.RegisterCallback<ClickEvent>(ev => AbilityButtonClicked(basicAttack));

        AbilityButton basicDefend = new(_selectedPlayerStats.BasicAbilities[1], "S");
        basicDefend.RegisterCallback<ClickEvent>(ev => AbilityButtonClicked(basicDefend));

        _characterAbilitiesContainer.Clear();
        _characterAbilitiesContainer.Add(basicAttack);
        _characterAbilitiesContainer.Add(basicDefend);

        for (int i = 0; i < _selectedPlayerStats.Abilities.Count; i++)
        {
            AbilityButton button = new(_selectedPlayerStats.Abilities[i], buttons[i]);
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

        _areButtonEnabled = true;

        foreach (var el in _characterAbilitiesContainer.Children())
        {
            AbilityButton ab = (AbilityButton)el;
            if (ab.Ability.ManaCost >= _selectedPlayerStats.CurrentMana)
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
        InventoryUI.instance.EnableInventoryUI();
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

        Task task = ability.HighlightTargetable(_battleCharacterController.SelectedCharacter); // for some reason this works, but it has to be written exactly like that with Task task = ;
        yield return new WaitUntil(() => task.IsCompleted);

        _battleCharacterController.SetSelectedAbility(ability);
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
