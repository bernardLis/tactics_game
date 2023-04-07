using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilityCraftManager : MonoBehaviour
{
    const string _ussClassName = "ability-crafting__";
    const string _ussCraftButton = _ussClassName + "craft-button";
    const string _ussDiscardButton = _ussClassName + "discard-button";


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

    VisualElement _abilityDamageContainer;

    Label _abilityDamage;
    Label _abilityManaCost;

    VisualElement _abilityCostContainer;
    SpiceElement _spiceElement;

    VisualElement _abilityButtonsContainer;
    MyButton _craftButton;
    MyButton _discardButton;

    EffectHolder _addedToCraftingEffect;

    bool _subscribedToNodeActions;


    void Start()
    {
        _gameManager = GameManager.Instance;

        _dashboardManager = GetComponent<DashboardManager>();
        _dashboardManager.OnAbilitiesOpened += OnAbilitiesClicked;
        _dashboardManager.OnHideAllPanels += OnHideAllPanels;
        _root = _dashboardManager.Root;

        _abilityGraphManager = GetComponent<AbilityGraphManager>();

        _abilityCraft = _root.Q<VisualElement>("abilityCraft");

        GetCraftContainerElements();
        SetupCraftContainer();
    }

    void OnAbilitiesClicked()
    {
        if (_subscribedToNodeActions)
            return;
        _subscribedToNodeActions = true;

        _abilityGraphManager.CraftAbilityNodeSlot.OnNodeAdded += OnCraftNodeAdded;
        _abilityGraphManager.CraftAbilityNodeSlot.OnNodeRemoved += OnCraftNodeRemoved;
    }

    void OnHideAllPanels() { DiscardAbility(); }

    void OnCraftNodeAdded(AbilityNodeElement nodeVisualElement)
    {
        ClearEffects();

        _craftTooltip.text = "Add stars to get stronger abilities.";
        _abilityNode = nodeVisualElement.AbilityNode;
        _numberOfStars = (int)_abilityNode.GetStarRange().x;
        _abilityTemplate = _abilityNode.Abilities[0];

        UpdateStars();

        _addedToCraftingEffect = _abilityNode.AddedToCraftingEffect;
        _addedToCraftingEffect.PlayEffect(_abilityNode.AddedToCraftingEffectPosition, _abilityNode.AddedToCraftingEffectScale);
    }

    void OnCraftNodeRemoved()
    {
        ResetCraftValues();
        ClearEffects();
    }

    void GetCraftContainerElements()
    {
        _craftTooltip = _root.Q<Label>("craftTooltip");
        _starContainer = _root.Q<VisualElement>("craftAbilityStarContainer");

        _craftAbilityName = _root.Q<TextField>("craftAbilityName");
        _abilityDescriptionContainer = _root.Q<VisualElement>("craftAbilityDescriptionContainer");

        _abilityDamageContainer = _root.Q<VisualElement>("craftAbilityDamageContainer");

        _abilityManaCost = _root.Q<Label>("craftAbilityManaCost");
        _abilityCostContainer = _root.Q<VisualElement>("craftAbilityCostContainer");

        _abilityButtonsContainer = _root.Q<VisualElement>("craftAbilityButtonsContainer");
    }

    void SetupCraftContainer()
    {
        _craftTooltip.text = ("Drag & drop unlocked node to start ability crafting");
        SetUpStarContainer();
        SetUpDescriptionContainer();
        SetUpDamageContainer();

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
        if (_numberOfStars <= 0 || _numberOfStars <= (int)_abilityNode.GetStarRange().x)
        {
            Helpers.DisplayTextOnElement(_root, _starContainer, "Can't be less", Color.red);
            return;
        }
        _numberOfStars--;
        UpdateStars();
    }

    void AddStarsToAbility()
    {
        if (_numberOfStars >= 5 || _numberOfStars >= _abilityNode.GetStarRange().y)
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

    void SetUpDamageContainer()
    {
        _abilityDamage = new("Damage: 0");
        _abilityDamageContainer.Clear();
        _abilityDamageContainer.Add(_abilityDamage);
    }

    void UpdateCraftingValuesDisplayed()
    {
        _abilityDescription.text = $"{_abilityTemplate.Description}";
        _abilityDamage.text = $"Damage: {_abilityTemplate.BasePower}";

        _abilityManaCost.text = $"Mana cost: {_abilityTemplate.ManaCost}";

        _spiceElement.ChangeAmount(_abilityNode.GetSpiceCostByStars(_numberOfStars));
    }

    void ResetCraftValues()
    {
        _spiceElement.ChangeAmount(0);

        _craftAbilityName.value = "Name your ability";
        _abilityDamage.text = $"Damage: 0";

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
        _craftButton = new("", _ussCraftButton, CraftAbility);
        _discardButton = new("", _ussDiscardButton, DiscardAbility);
        _abilityButtonsContainer.Add(_craftButton);
        _abilityButtonsContainer.Add(_discardButton);
    }

    void CraftAbility()
    {
        if (_abilityNode == null)
        {
            Helpers.DisplayTextOnElement(_root, _abilityButtonsContainer, "Drag an unlocked node.", Color.red);
            return;

        }
        if (_gameManager.Spice < _abilityNode.GetSpiceCostByStars(_numberOfStars))
        {
            Helpers.DisplayTextOnElement(_root, _abilityButtonsContainer, "Not enough spice.", Color.red);
            return;
        }

        _gameManager.ChangeSpiceValue(-_abilityNode.GetSpiceCostByStars(_numberOfStars));

        Ability craftedAbility = _abilityNode.CreateAbility(_numberOfStars, _craftAbilityName.value);

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
        if (_abilityNode == null) return;
        _abilityNode = null;
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

}
