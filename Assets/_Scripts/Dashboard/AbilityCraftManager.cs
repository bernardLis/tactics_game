using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilityCraftManager : MonoBehaviour
{
    GameManager _gameManager;
    DashboardManager _dashboardManager;
    AbilityGraphManager _abilityGraphManager;

    VisualElement _root;

    VisualElement _abilityCraft;
    Label _craftTooltip;

    AbilityNode _currentAbilityNode;
    TextField _craftAbilityName;

    VisualElement _abilityRangeContainer;
    VisualElement _abilityDamageContainer;
    VisualElement _abilityAOEContainer;

    Label _abilityRange;
    MyButton _rangePlus;
    MyButton _rangeMinus;

    Label _abilityDamage;
    MyButton _damagePlus;
    MyButton _damageMinus;

    Label _abilityAOE;
    MyButton _AOEPlus;
    MyButton _AOEMinus;

    Toggle _abilityStatus;

    Label _abilityManaCost;

    VisualElement _abilityCostContainer;
    SpiceElement _spiceElement;

    VisualElement _abilityButtonsContainer;
    MyButton _craftButton;
    MyButton _discardButton;

    VisualElement _craftAbilityCraftedAbilityContainer;

    int _range;
    int _damage;
    int _aoe;
    bool _isStatusAdded;
    int _manaCost;

    public GameObject[] TestEffects; // HERE:
    GameObject _instantiatedEffect;

    VisualElement _vfxDisplayer;


    public EffectHolder TestEffect;

    void Start()
    {
        _gameManager = GameManager.Instance;

        _dashboardManager = GetComponent<DashboardManager>();
        _dashboardManager.OnAbilitiesOpened += OnAbilitiesClicked;
        _root = _dashboardManager.Root;

        _abilityGraphManager = GetComponent<AbilityGraphManager>();
        _abilityGraphManager.OnCraftNodeAdded += OnCraftNodeAdded;

        _abilityCraft = _root.Q<VisualElement>("abilityCraft");
        GetCraftContainerElements();
        SetupCraftContainer();

        TestEffect.PlayEffect(new Vector3(6.3f, -3.6f), Vector3.one);
    }

    void OnAbilitiesClicked()
    {

    }

    void OnCraftNodeAdded(AbilityNodeVisualElement nodeVisualElement)
    {
        Debug.Log($"added");
        _craftTooltip.text = "Change some values and hit the craft button to get a new ability.";
        _currentAbilityNode = nodeVisualElement.AbilityNode;

        _range = _currentAbilityNode.RangeMinMax.x;
        _damage = _currentAbilityNode.DamageMinMax.x;
        _aoe = _currentAbilityNode.AOEMinMax.x;
        _isStatusAdded = false;
        _manaCost = _currentAbilityNode.ManaCostMinMax.x;

        UpdateCraftingValuesDisplayed();

        _spiceElement.ChangeAmount(_currentAbilityNode.SpiceCost);

        EnableCraftButtons();
    }

    void GetCraftContainerElements()
    {
        _craftTooltip = _root.Q<Label>("craftTooltip");
        _craftAbilityName = _root.Q<TextField>("craftAbilityName");

        _abilityRangeContainer = _root.Q<VisualElement>("craftAbilityRangeContainer");
        _abilityDamageContainer = _root.Q<VisualElement>("craftAbilityDamageContainer");
        _abilityAOEContainer = _root.Q<VisualElement>("craftAbilityAOEContainer");

        _abilityStatus = _root.Q<Toggle>("craftAbilityStatus");
        _abilityManaCost = _root.Q<Label>("craftAbilityManaCost");
        _abilityCostContainer = _root.Q<VisualElement>("craftAbilityCostContainer");

        _abilityButtonsContainer = _root.Q<VisualElement>("craftAbilityButtonsContainer");
        _craftAbilityCraftedAbilityContainer = _root.Q<VisualElement>("craftAbilityCraftedAbilityContainer");
    }

    void SetupCraftContainer()
    {
        _craftTooltip.text = ("Drag & drop unlocked node to start ability crafting");

        SetUpRangeContainer();
        SetUpDamageContainer();
        SetUpAOEContainer();
        SetUpStatusToggle();

        CreateCraftSpiceElement();

        ResetCraftValues();

        SetupActionButtons();
    }

    void SetUpRangeContainer()
    {
        _abilityRange = new("Range: 0");
        _rangePlus = new("", "craftButtonPlus", RangePlus);
        _rangeMinus = new("", "craftButtonMinus", RangeMinus);

        _rangePlus.SetEnabled(false);
        _rangeMinus.SetEnabled(false);

        _abilityRangeContainer.Clear();
        _abilityRangeContainer.Add(_abilityRange);
        _abilityRangeContainer.Add(_rangePlus);
        _abilityRangeContainer.Add(_rangeMinus);
    }

    void RangePlus()
    {
        AbilityCraftingValidity v = _currentAbilityNode.CheckAbilityValidity(_range + 1, _damage, _aoe, _isStatusAdded);
        if (!v.IsValid)
        {
            Helpers.DisplayTextOnElement(_root, _abilityStatus, v.Message, Color.red);
            return;
        }
        _range++;
        _manaCost = v.ManaCost;
        UpdateCraftingValuesDisplayed();
    }

    void RangeMinus()
    {
        AbilityCraftingValidity v = _currentAbilityNode.CheckAbilityValidity(_range - 1, _damage, _aoe, _isStatusAdded);
        if (!v.IsValid)
        {
            Helpers.DisplayTextOnElement(_root, _abilityStatus, v.Message, Color.red);
            return;
        }

        _range--;
        _manaCost = v.ManaCost;
        UpdateCraftingValuesDisplayed();
    }

    void SetUpDamageContainer()
    {
        _abilityDamage = new("Damage: 0");
        _damagePlus = new("", "craftButtonPlus", DamagePlus);
        _damageMinus = new("", "craftButtonMinus", DamageMinus);

        _damagePlus.SetEnabled(false);
        _damageMinus.SetEnabled(false);

        _abilityDamageContainer.Clear();
        _abilityDamageContainer.Add(_abilityDamage);
        _abilityDamageContainer.Add(_damagePlus);
        _abilityDamageContainer.Add(_damageMinus);
    }

    void DamagePlus()
    {
        AbilityCraftingValidity v = _currentAbilityNode.CheckAbilityValidity(_range, _damage + 10, _aoe, _isStatusAdded);
        if (!v.IsValid)
        {
            Helpers.DisplayTextOnElement(_root, _abilityStatus, v.Message, Color.red);
            return;
        }

        _damage += 10;
        _manaCost = v.ManaCost;
        UpdateCraftingValuesDisplayed();
    }

    void DamageMinus()
    {
        AbilityCraftingValidity v = _currentAbilityNode.CheckAbilityValidity(_range, _damage - 10, _aoe, _isStatusAdded);
        if (!v.IsValid)
        {
            Helpers.DisplayTextOnElement(_root, _abilityStatus, v.Message, Color.red);
            return;
        }

        _damage -= 10;
        _manaCost = v.ManaCost;
        UpdateCraftingValuesDisplayed();
    }

    void SetUpAOEContainer()
    {
        _abilityAOE = new("AOE: 0");
        _AOEPlus = new("", "craftButtonPlus", AOEPlus);
        _AOEMinus = new("", "craftButtonMinus", AOEMinus);

        _AOEPlus.SetEnabled(false);
        _AOEMinus.SetEnabled(false);

        _abilityAOEContainer.Clear();
        _abilityAOEContainer.Add(_abilityAOE);
        _abilityAOEContainer.Add(_AOEPlus);
        _abilityAOEContainer.Add(_AOEMinus);
    }

    void AOEPlus()
    {
        AbilityCraftingValidity v = _currentAbilityNode.CheckAbilityValidity(_range, _damage, _aoe + 1, _isStatusAdded);
        if (!v.IsValid)
        {
            Helpers.DisplayTextOnElement(_root, _abilityStatus, v.Message, Color.red);
            return;
        }

        _aoe++;
        _manaCost = v.ManaCost;
        UpdateCraftingValuesDisplayed();
    }

    void AOEMinus()
    {
        AbilityCraftingValidity v = _currentAbilityNode.CheckAbilityValidity(_range, _damage, _aoe - 1, _isStatusAdded);
        if (!v.IsValid)
        {
            Helpers.DisplayTextOnElement(_root, _abilityStatus, v.Message, Color.red);
            return;
        }

        _aoe--;
        _manaCost = v.ManaCost;
        UpdateCraftingValuesDisplayed();
    }

    void SetUpStatusToggle() { _abilityStatus.RegisterValueChangedCallback(StatusToggleValueChanged); }
    void StatusToggleValueChanged(ChangeEvent<bool> evt)
    {
        AbilityCraftingValidity v = _currentAbilityNode.CheckAbilityValidity(_range, _damage, _aoe, evt.newValue);

        if (!v.IsValid)
        {
            evt.PreventDefault();
            Helpers.DisplayTextOnElement(_root, _abilityStatus, v.Message, Color.red);
            return;
        }

        _isStatusAdded = evt.newValue;
        _manaCost = v.ManaCost;
        UpdateCraftingValuesDisplayed();
    }

    void UpdateCraftingValuesDisplayed()
    {
        _abilityRange.text = $"Range: {_range}";
        _abilityDamage.text = $"Damage: {_damage}";
        _abilityAOE.text = $"AOE: {_aoe}";

        _abilityManaCost.text = $"Mana cost: {_manaCost}";
    }

    void EnableCraftButtons()
    {
        _rangePlus.SetEnabled(true);
        _rangeMinus.SetEnabled(true);
        _damagePlus.SetEnabled(true);
        _damageMinus.SetEnabled(true);
        _AOEPlus.SetEnabled(true);
        _AOEMinus.SetEnabled(true);

        _abilityStatus.RegisterValueChangedCallback(StatusToggleValueChanged);
    }

    void DisableCraftButtons()
    {
        _abilityStatus.UnregisterValueChangedCallback(StatusToggleValueChanged);
    }

    void ResetCraftValues()
    {
        _range = 0;
        _damage = 0;
        _aoe = 0;
        _isStatusAdded = false;
        _manaCost = 0;
        _spiceElement.ChangeAmount(0);

        _craftAbilityName.value = "Name your ability";
        _abilityRange.text = $"Range: {0}";

        _abilityStatus.value = false;
        _abilityManaCost.text = "Mana cost: 0";
        DisableCraftButtons();
    }

    void CreateCraftSpiceElement()
    {
        _abilityCostContainer.Clear();

        Label cost = new("Cost to craft: ");
        _abilityCostContainer.Add(cost);

        _spiceElement = new(0);
        _abilityCostContainer.Add(_spiceElement);
    }


    void SetupActionButtons()
    {
        _abilityButtonsContainer.Clear();
        _craftButton = new("", "craftCraftButton", CraftAbility);
        _discardButton = new("", "craftDiscardButton", DiscardAbility);
        _abilityButtonsContainer.Add(_craftButton);
        _abilityButtonsContainer.Add(_discardButton);
    }

    void CraftAbility()
    {
        if (_instantiatedEffect != null)
            Destroy(_instantiatedEffect);

        // probably some noise and animation
        // probably need to show the created ability
        // probably need to signal that it will be available in armory
        // TODO: get the name
        Ability craftedAbility = _currentAbilityNode.CreateAbility("asd", _range, _damage, _aoe, _isStatusAdded);
        _gameManager.AddAbilityToPouch(craftedAbility);

        _craftAbilityCraftedAbilityContainer.Clear();
        Label header = new("Your ability will be available in armory. This is what you have created:");
        AbilityButton button = new(craftedAbility);
        _craftAbilityCraftedAbilityContainer.Add(header);
        _craftAbilityCraftedAbilityContainer.Add(button);
    }

    void DiscardAbility()
    {
        _gameManager.ChangeSpiceValue(500);

        _abilityGraphManager.ClearCraftSlot();
        ResetCraftValues();
        // instantiate effect
        if (_instantiatedEffect != null)
            Destroy(_instantiatedEffect);

        _instantiatedEffect = Instantiate(TestEffects[Random.Range(0, TestEffects.Length)], Vector3.zero, Quaternion.identity);
        _instantiatedEffect.layer = Tags.UIVFXLayer;
        foreach (Transform child in _instantiatedEffect.transform)
            child.gameObject.layer = Tags.UIVFXLayer;
        // VisualElement el = new();
        if (_vfxDisplayer == null)
        {
            _vfxDisplayer = new();
            _vfxDisplayer.pickingMode = PickingMode.Ignore;
            _vfxDisplayer.AddToClassList("vfx");
            _vfxDisplayer.style.position = Position.Absolute;
            _vfxDisplayer.style.width = Length.Percent(100);
            _vfxDisplayer.style.height = Length.Percent(100);
            _root.Add(_vfxDisplayer);
        }
        // el.Add(item);

    }

}
