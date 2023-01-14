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

    AbilityNode _abilityNode;
    int _numberOfStars = 0;
    Ability _abilityTemplate;

    VisualElement _starContainer;
    StarRankElement _rankVisualElement;
    MyButton _addStarsButton;
    MyButton _subtractStarsButton;

    TextField _craftAbilityName;

    VisualElement _abilityDescriptionContainer;
    Label _abilityDescription;

    VisualElement _abilityRangeContainer;
    VisualElement _abilityDamageContainer;
    VisualElement _abilityAOEContainer;
    VisualElement _abilityStatusContainer;

    Label _abilityRange;
    Label _abilityDamage;
    Label _abilityAOE;
    Label _abilityManaCost;

    VisualElement _abilityCostContainer;
    SpiceElement _spiceElement;

    VisualElement _abilityButtonsContainer;
    MyButton _craftButton;
    MyButton _discardButton;

    VisualElement _craftedAbilitiesContainer;
    List<AbilityButton> _craftedAbilityButtons = new();

    EffectHolder _addedToCraftingEffect;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnDayPassed += OnDayPassed;

        _dashboardManager = GetComponent<DashboardManager>();
        _dashboardManager.OnAbilitiesOpened += OnAbilitiesClicked;
        _dashboardManager.OnHideAllPanels += OnHideAllPanels;
        _root = _dashboardManager.Root;

        _abilityGraphManager = GetComponent<AbilityGraphManager>();
        _abilityGraphManager.OnCraftNodeAdded += OnCraftNodeAdded;

        _abilityCraft = _root.Q<VisualElement>("abilityCraft");
        _craftedAbilitiesContainer = _root.Q<VisualElement>("craftedAbilitiesContainer");

        GetCraftContainerElements();
        SetupCraftContainer();
        PopulateCraftedAbilities();
    }

    void OnDayPassed(int day)
    {
        UpdateCraftedAbilities();
    }

    void OnAbilitiesClicked()
    {
    }

    void OnHideAllPanels()
    {
        DiscardAbility();
    }

    void OnCraftNodeAdded(AbilityNodeElement nodeVisualElement)
    {
        ClearEffects();

        _craftTooltip.text = "Add stars to get stronger abilities.";
        _abilityNode = nodeVisualElement.AbilityNode;
        _numberOfStars = _abilityNode.StarRange.x;
        _abilityTemplate = _abilityNode.AbilityNodeTemplates[0].Ability;

        UpdateStars();

        _addedToCraftingEffect = _abilityNode.AddedToCraftingEffect;
        _addedToCraftingEffect.PlayEffect(_abilityNode.AddedToCraftingEffectPosition, _abilityNode.AddedToCraftingEffectScale);
    }

    void GetCraftContainerElements()
    {
        _craftTooltip = _root.Q<Label>("craftTooltip");
        _starContainer = _root.Q<VisualElement>("craftAbilityStarContainer");

        _craftAbilityName = _root.Q<TextField>("craftAbilityName");
        _abilityDescriptionContainer = _root.Q<VisualElement>("craftAbilityDescriptionContainer");

        _abilityRangeContainer = _root.Q<VisualElement>("craftAbilityRangeContainer");
        _abilityDamageContainer = _root.Q<VisualElement>("craftAbilityDamageContainer");
        _abilityAOEContainer = _root.Q<VisualElement>("craftAbilityAOEContainer");
        _abilityStatusContainer = _root.Q<VisualElement>("craftAbilityStatusContainer");

        _abilityManaCost = _root.Q<Label>("craftAbilityManaCost");
        _abilityCostContainer = _root.Q<VisualElement>("craftAbilityCostContainer");

        _abilityButtonsContainer = _root.Q<VisualElement>("craftAbilityButtonsContainer");
    }

    void SetupCraftContainer()
    {
        _craftTooltip.text = ("Drag & drop unlocked node to start ability crafting");
        SetUpStarContainer();
        SetUpDescriptionContainer();
        SetUpRangeContainer();
        SetUpDamageContainer();
        SetUpAOEContainer();
        SetUpStatusContainer();

        CreateCraftSpiceElement();
        ResetCraftValues();
        SetupActionButtons();
    }

    void SetUpStarContainer()
    {
        _starContainer.Clear();

        _addStarsButton = new("", "craftButtonMinus", SubtractStarsFromAbility);
        _starContainer.Add(_addStarsButton);

        _rankVisualElement = new(0);
        _starContainer.Add(_rankVisualElement);

        _subtractStarsButton = new("", "craftButtonPlus", AddStarsToAbility);
        _starContainer.Add(_subtractStarsButton);
    }

    void SubtractStarsFromAbility()
    {
        if (_numberOfStars <= 0 || _numberOfStars <= _abilityNode.StarRange.x)
        {
            Helpers.DisplayTextOnElement(_root, _starContainer, "Can't be less", Color.red);
            return;
        }
        _numberOfStars--;
        UpdateStars();
    }

    void AddStarsToAbility()
    {
        if (_numberOfStars >= 5 || _numberOfStars >= _abilityNode.StarRange.y)
        {
            Helpers.DisplayTextOnElement(_root, _starContainer, "Can't be more", Color.red);
            return;
        }
        _numberOfStars++;
        UpdateStars();

    }

    void UpdateStars()
    {
        _abilityTemplate = _abilityNode.GetAbilityByStars(_numberOfStars);
        _rankVisualElement.SetRank(_numberOfStars);
        UpdateCraftingValuesDisplayed();
    }

    void SetUpDescriptionContainer()
    {
        _abilityDescription = new("");
        _abilityDescription.style.whiteSpace = WhiteSpace.Normal;
        _abilityDescriptionContainer.Clear();
        _abilityDescriptionContainer.Add(_abilityDescription);
    }

    void SetUpRangeContainer()
    {
        _abilityRange = new("Range: 0");
        _abilityRangeContainer.Clear();
        _abilityRangeContainer.Add(_abilityRange);
    }

    void SetUpDamageContainer()
    {
        _abilityDamage = new("Damage: 0");
        _abilityDamageContainer.Clear();
        _abilityDamageContainer.Add(_abilityDamage);
    }

    void SetUpAOEContainer()
    {
        _abilityAOE = new("AOE: 0");
        _abilityAOEContainer.Clear();
        _abilityAOEContainer.Add(_abilityAOE);
    }

    void SetUpStatusContainer()
    {
        _abilityStatusContainer.Clear();
    }

    void UpdateCraftingValuesDisplayed()
    {
        _abilityDescription.text = $"{_abilityTemplate.Description}";
        _abilityRange.text = $"Range: {_abilityTemplate.Range}";
        _abilityDamage.text = $"Damage: {_abilityTemplate.BasePower}";
        _abilityAOE.text = $"AOE: {_abilityTemplate.AreaOfEffect}";

        _abilityStatusContainer.Clear();
        _abilityStatusContainer.Add(new Label("Status: "));
        if (_abilityTemplate.Status != null)
            _abilityStatusContainer.Add(new ModifierElement(_abilityTemplate.Status));

        _abilityManaCost.text = $"Mana cost: {_abilityTemplate.ManaCost}";

        _spiceElement.ChangeAmount(_abilityNode.GetSpiceCostByStars(_numberOfStars));
    }

    void ResetCraftValues()
    {
        _spiceElement.ChangeAmount(0);

        _craftAbilityName.value = "Name your ability";
        _abilityRange.text = $"Range: 0";
        _abilityDamage.text = $"Damage: 0";
        _abilityAOE.text = $"AOE: 0";
        _abilityStatusContainer.Clear();

        _abilityManaCost.text = $"Mana cost: 0";
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
        if (_gameManager.Spice < _abilityNode.GetSpiceCostByStars(_numberOfStars))
        {
            Helpers.DisplayTextOnElement(_root, _abilityButtonsContainer, "Not enough spice", Color.red);
            return;
        }

        _gameManager.ChangeSpiceValue(-_abilityNode.GetSpiceCostByStars(_numberOfStars));

        Ability craftedAbility = _abilityNode.CreateAbility(_numberOfStars, _craftAbilityName.value);
        craftedAbility.TimeLeftToCrafted = _abilityNode.DaysOnCooldownRemaining;
        _gameManager.AddCraftedAbility(craftedAbility);

        AbilityButton button = new(craftedAbility);
        _craftedAbilityButtons.Add(button);
        button.AddCooldownOverlay();
        _craftedAbilitiesContainer.Add(button);

        // vfx
        Vector3 pos = _abilityButtonsContainer.worldTransform.GetPosition();
        pos.x = pos.x + _abilityButtonsContainer.resolvedStyle.width / 2;
        pos.y = Camera.main.pixelHeight - pos.y - _abilityButtonsContainer.resolvedStyle.height; // inverted, plus play on bottom of element
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(pos);
        worldPos.z = 0;

        _abilityNode.AbilityCraftedEffect.PlayEffect(worldPos, _abilityNode.AbilityCraftedEffectScale);
        DiscardAbility();
    }


    void DiscardAbility()
    {
        _abilityGraphManager.ClearCraftSlot();
        ResetCraftValues();
        ClearEffects();
    }

    void ClearEffects()
    {
        if (_addedToCraftingEffect)
        {
            _addedToCraftingEffect.DestroyEffect();
            _addedToCraftingEffect = null;
        }
    }

    /* crafted abilities */
    void PopulateCraftedAbilities()
    {
        foreach (Ability ability in _gameManager.CraftedAbilities)
        {
            AbilityButton button = new(ability);
            button.AddCooldownOverlay();
            _craftedAbilitiesContainer.Add(button);
            _craftedAbilityButtons.Add(button);
        }
    }

    void UpdateCraftedAbilities()
    {
        List<AbilityButton> buttonsToRemove = new();
        foreach (AbilityButton button in _craftedAbilityButtons)
        {
            button.Ability.TimeLeftToCrafted--;
            button.UpdateCooldownOverlay();
            if (button.Ability.TimeLeftToCrafted <= 0)
            {
                CreateReport(button.Ability);
                buttonsToRemove.Add(button);
                _craftedAbilitiesContainer.Remove(button);
            }
        }

        foreach (AbilityButton button in buttonsToRemove)
            _craftedAbilityButtons.Remove(button);
    }

    void CreateReport(Ability craftedAbility)
    {
        Report report = ScriptableObject.CreateInstance<Report>();
        report.Initialize(ReportType.Ability, null, null, null, null, null, craftedAbility);
        _gameManager.AddNewReport(report);
        _gameManager.SaveJsonData();
    }
}
