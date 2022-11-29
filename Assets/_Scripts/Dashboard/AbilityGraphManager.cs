using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilityGraphManager : MonoBehaviour
{
    GameManager _gameManager;
    DashboardManager _dashboardManager;

    List<AbilityNodeGraph> _abilityNodeGraphs;

    VisualElement _root;
    VisualElement _abilityGraphs;

    VisualElement _abilityCraft;
    Label _craftTooltip;
    VisualElement _craftAbilityNodeSlotContainer;
    AbilityNodeSlotVisualElement _craftAbilityNodeSlot;
    TextField _craftAbilityName;

    VisualElement _craftAbilityRangeContainer;
    VisualElement _craftAbilityDamageContainer;
    VisualElement _craftAbilityAOEContainer;

    Label _craftAbilityRange;
    MyButton _rangePlus;
    MyButton _rangeMinus;

    Label _craftAbilityDamage;
    Label _craftAbilityAOE;

    Toggle _craftAbilityStatus;

    Label _craftAbilityManaCost;

    VisualElement _craftAbilityCostContainer;
    SpiceElement _craftSpiceElement;

    VisualElement _craftAbilityButtonsContainer;

    AbilityNode _craftCurrentAbilityNode;

    int _range;
    int _damage;
    int _aoe;
    int _manaCost;

    // drag & drop
    // https://gamedev-resources.com/create-an-in-game-inventory-ui-with-ui-toolkit/
    bool _isDragging;

    VisualElement _dragDropContainer;
    AbilityNodeVisualElement _draggedNode;


    void Start()
    {
        _gameManager = GameManager.Instance;
        _abilityNodeGraphs = _gameManager.GetAbilityNodeGraphs();

        _dashboardManager = GetComponent<DashboardManager>();
        _dashboardManager.OnAbilitiesOpened += OnAbilitiesClicked;
        _root = _dashboardManager.Root;

        _abilityGraphs = _root.Q<VisualElement>("abilityGraphs");
        _abilityGraphs.Clear();

        _abilityCraft = _root.Q<VisualElement>("abilityCraft");


        CreateGraphs();
        GetCraftContainerElements();
        SetupCraftContainer();

        // drag & drop
        _dragDropContainer = new VisualElement();
        _dragDropContainer.AddToClassList("abilityNodeDragAndDrop");
        _root.Add(_dragDropContainer);

        _root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        _root.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    void OnAbilitiesClicked()
    {

    }


    void CreateGraphs()
    {
        VisualElement graphContainer = new();
        graphContainer.AddToClassList("graphContainer");
        graphContainer.AddToClassList("textPrimary");
        _abilityGraphs.Add(graphContainer);

        Label title = new(_abilityNodeGraphs[0].Title);
        title.style.fontSize = 48;
        graphContainer.Add(title);

        VisualElement wrapper = new();
        wrapper.style.flexDirection = FlexDirection.Row;
        graphContainer.Add(wrapper);
        // HERE: for now, only 1 graph
        for (int i = 0; i < _abilityNodeGraphs[0].AbilityNodes.Length; i++)
        {
            VisualElement container = new();

            AbilityNode n = _abilityNodeGraphs[0].AbilityNodes[i];
            AbilityNodeVisualElement el = new(n);
            el.RegisterCallback<PointerDownEvent>(OnNodePointerDown);

            AbilityNodeSlotVisualElement slot = new(el, n.IsUnlocked);

            container.Add(slot);
            container.Add(GetNodeCostElement(n));

            wrapper.Add(container);
        }
    }

    void OnNodePointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0)
            return;

        AbilityNodeVisualElement node = (AbilityNodeVisualElement)evt.currentTarget;
        if (!node.AbilityNode.IsUnlocked)
            return;

        AbilityNodeVisualElement nodeCopy = new(node.AbilityNode);
        nodeCopy.BlockTooltip();
        StartNodeDrag(evt.position, nodeCopy);
    }

    void StartNodeDrag(Vector2 position, AbilityNodeVisualElement draggedNode)
    {
        _draggedNode = draggedNode;

        //Set tracking variables
        _isDragging = true;

        //Set the new position
        _dragDropContainer.BringToFront();
        _dragDropContainer.style.top = position.y - _dragDropContainer.layout.height / 2;
        _dragDropContainer.style.left = position.x - _dragDropContainer.layout.width / 2;
        //Set the image
        _dragDropContainer.Add(draggedNode);
        //Flip the visibility on
        _dragDropContainer.style.visibility = Visibility.Visible;
    }

    void OnPointerMove(PointerMoveEvent evt)
    {
        //Only take action if the player is dragging an item around the screen
        if (!_isDragging)
            return;

        if (_draggedNode != null)
        {
            //Set the new position
            _dragDropContainer.style.top = evt.position.y - _dragDropContainer.layout.height / 2;
            _dragDropContainer.style.left = evt.position.x - _dragDropContainer.layout.width / 2;
        }
    }

    void OnPointerUp(PointerUpEvent evt)
    {
        if (!_isDragging)
            return;

        if (_draggedNode != null)
            HandleNodePointerUp();
    }

    void HandleNodePointerUp()
    {
        if (_dragDropContainer.worldBound.Overlaps(_craftAbilityNodeSlot.worldBound))
            _craftAbilityNodeSlot.AddNode(_draggedNode);

        _dragDropContainer.Clear();
        _draggedNode = null;
        _isDragging = false;
        _dragDropContainer.style.visibility = Visibility.Hidden;
    }

    VisualElement GetNodeCostElement(AbilityNode abilityNode)
    {
        VisualElement cost = new();
        cost.style.flexDirection = FlexDirection.Row;
        cost.style.justifyContent = Justify.Center;
        cost.Add(new SpiceElement(abilityNode.SpiceCost));
        return cost;
    }

    void GetCraftContainerElements()
    {
        _craftTooltip = _root.Q<Label>("craftTooltip");
        _craftAbilityNodeSlotContainer = _root.Q<VisualElement>("craftAbilityNodeSlotContainer");
        _craftAbilityName = _root.Q<TextField>("craftAbilityName");

        _craftAbilityRangeContainer = _root.Q<VisualElement>("craftAbilityRangeContainer");
        _craftAbilityDamageContainer = _root.Q<VisualElement>("craftAbilityDamageContainer");
        _craftAbilityAOEContainer = _root.Q<VisualElement>("craftAbilityAOEContainer");

        _craftAbilityStatus = _root.Q<Toggle>("craftAbilityStatus");
        _craftAbilityManaCost = _root.Q<Label>("craftAbilityManaCost");
        _craftAbilityCostContainer = _root.Q<VisualElement>("craftAbilityCostContainer");

        _craftAbilityButtonsContainer = _root.Q<VisualElement>("craftAbilityButtonsContainer");
    }

    void SetupCraftContainer()
    {
        _craftTooltip.text = ("Drag & drop unlocked node to start ability crafting");

        _craftAbilityNodeSlot = new();
        _craftAbilityNodeSlotContainer.Add(_craftAbilityNodeSlot);
        _craftAbilityNodeSlot.OnNodeAdded += OnCraftNodeAdded;

        SetUpRangeContainer();

        ResetCraftValues();
        CreateCraftSpiceElement();
    }

    void SetUpRangeContainer()
    {
        _craftAbilityRange = new("Range: 0");
        _rangePlus = new("", "craftButtonPlus", RangePlus);
        _rangeMinus = new("", "craftButtonMinus", RangeMinus);

        _rangePlus.SetEnabled(false);
        _rangeMinus.SetEnabled(false);

        _craftAbilityRangeContainer.Clear();
        _craftAbilityRangeContainer.Add(_craftAbilityRange);
        _craftAbilityRangeContainer.Add(_rangePlus);
        _craftAbilityRangeContainer.Add(_rangeMinus);
    }

    void RangePlus()
    {
        _range++;
        BaseRangeChange();
    }

    void RangeMinus()
    {
        _range--;
        BaseRangeChange();
    }

    void BaseRangeChange()
    {
        _craftAbilityRange.text = $"Range: {_range}";
    }

    void EnableCraftButtons()
    {
        _rangePlus.SetEnabled(true);
        _rangeMinus.SetEnabled(true);
    }

    void ResetCraftValues()
    {
        _range = 0;
        _damage = 0;
        _aoe = 0;
        _manaCost = 0;
        _craftSpiceElement.ChangeAmount(0);

        _craftAbilityName.value = "Name your ability";
        _craftAbilityRange.text = $"Range: {0}";

        _craftAbilityStatus.value = false;
        _craftAbilityManaCost.text = "Mana cost: 0";
    }

    void CreateCraftSpiceElement()
    {
        _craftAbilityCostContainer.Clear();

        Label cost = new("Cost to craft: ");
        _craftAbilityCostContainer.Add(cost);

        _craftSpiceElement = new(0);
        _craftAbilityCostContainer.Add(_craftSpiceElement);
    }

    void OnCraftNodeAdded(AbilityNodeVisualElement nodeVisualElement)
    {
        _craftTooltip.text = "Change some values and hit the craft button to get a new ability.";
        EnableCraftButtons();
    }
}


