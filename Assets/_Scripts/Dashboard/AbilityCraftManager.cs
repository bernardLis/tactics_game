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
    List<VisualElement> _starElements;
    MyButton _addStarsButton;
    MyButton _subtractStarsButton;

    VisualElement _abilityDescriptionContainer;
    Label _abilityDescription;
    TextField _craftAbilityName;

    VisualElement _abilityRangeContainer;
    VisualElement _abilityDamageContainer;
    VisualElement _abilityAOEContainer;

    Label _abilityRange;
    Label _abilityDamage;
    Label _abilityAOE;
    // HERE: status
    Label _abilityManaCost;

    VisualElement _abilityCostContainer;
    SpiceElement _spiceElement;

    VisualElement _abilityButtonsContainer;
    MyButton _craftButton;
    MyButton _discardButton;

    VisualElement _craftAbilityCraftedAbilityContainer;


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
        _craftTooltip.text = "Add stars to get stronger abilities.";
        _abilityNode = nodeVisualElement.AbilityNode;
        _numberOfStars = _abilityNode.StarRange.x;
        _abilityTemplate = _abilityNode.AbilityNodeTemplates[0].Ability;

        UpdateCraftingValuesDisplayed();
        UpdateStars();

        _spiceElement.ChangeAmount(_abilityNode.SpiceCost);
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
        // HERE: status

        _abilityManaCost = _root.Q<Label>("craftAbilityManaCost");
        _abilityCostContainer = _root.Q<VisualElement>("craftAbilityCostContainer");

        _abilityButtonsContainer = _root.Q<VisualElement>("craftAbilityButtonsContainer");
        _craftAbilityCraftedAbilityContainer = _root.Q<VisualElement>("craftAbilityCraftedAbilityContainer");
    }

    void SetupCraftContainer()
    {
        _craftTooltip.text = ("Drag & drop unlocked node to start ability crafting");
        SetUpStarContainer();
        SetUpDescriptionContainer();
        SetUpRangeContainer();
        SetUpDamageContainer();
        SetUpAOEContainer();
        // HERE: status set up the container

        CreateCraftSpiceElement();
        ResetCraftValues();
        SetupActionButtons();
    }

    void SetUpStarContainer()
    {
        _starContainer.Clear();
        _starElements = new();

        _addStarsButton = new("", "craftButtonMinus", SubtractStarsFromAbility);
        _starContainer.Add(_addStarsButton);

        for (int i = 0; i < 5; i++)
        {
            VisualElement star = new();
            star.AddToClassList("craftAbilityStarGray");
            _starElements.Add(star);
            _starContainer.Add(star);
        }

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
        for (int i = 0; i < _starElements.Count; i++)
        {
            _starElements[i].RemoveFromClassList("craftAbilityStar");
            if (i < _numberOfStars)
                _starElements[i].AddToClassList("craftAbilityStar");
        }

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

    void UpdateCraftingValuesDisplayed()
    {
        _abilityDescription.text = $"{_abilityTemplate.Description}";
        _abilityRange.text = $"Range: {_abilityTemplate.Range}";
        _abilityDamage.text = $"Damage: {_abilityTemplate.BasePower}";
        _abilityAOE.text = $"AOE: {_abilityTemplate.AreaOfEffect}";
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
        // HERE: status, clear container
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
        // probably some noise and animation
        // probably need to show the created ability
        // probably need to signal that it will be available in armory
        // TODO: get the name
        Ability craftedAbility = _abilityNode.CreateAbility(_numberOfStars, _craftAbilityName.value);
        _gameManager.AddAbilityToPouch(craftedAbility);

        _craftAbilityCraftedAbilityContainer.Clear();
        Label header = new("Your ability will be available in armory. This is what you have created:");
        AbilityButton button = new(craftedAbility);
        _craftAbilityCraftedAbilityContainer.Add(header);
        _craftAbilityCraftedAbilityContainer.Add(button);
    }

    void DiscardAbility()
    {
        _abilityGraphManager.ClearCraftSlot();
        ResetCraftValues();
    }

}
