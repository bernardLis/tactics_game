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

    AbilityNodeSlotVisualElement _craftAbilityNodeSlot;
    Label _craftTooltip;

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
        _abilityCraft.Clear();

        CreateGraphs();
        CreateCraftContainer();

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
        Debug.Log($"_dragDropContainer.worldBound {_dragDropContainer.worldBound}");
        Debug.Log($"_craftAbilityNodeSlot.worldBound {_craftAbilityNodeSlot.worldBound}");
        Debug.Log($"_craftAbilityNodeSlot.Overlaps(_dragDropContainer.worldBound) {_craftAbilityNodeSlot.Overlaps(_dragDropContainer.worldBound)}");

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

    void CreateCraftContainer()
    {
        _craftTooltip = new("Drag & drop unlocked node to start ability crafting");
        _craftTooltip.AddToClassList("textPrimary");

        _craftAbilityNodeSlot = new();
        _craftAbilityNodeSlot.OnNodeAdded += OnCraftNodeAdded;

        _abilityCraft.Add(_craftTooltip);
        _abilityCraft.Add(_craftAbilityNodeSlot);
    }

    void OnCraftNodeAdded(AbilityNodeVisualElement nodeVisualElement)
    {
        _craftTooltip.text = "Change some values and hit the save button to get a new ability.";
        
        // button to craft 
        // button to discard
        

    }

}


