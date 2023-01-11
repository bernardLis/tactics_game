using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using DG.Tweening;

public class DraggableAbilities : MonoBehaviour
{
    GameManager _gameManager;
    DeskManager _deskManager;

    VisualElement _root;
    VisualElement _abilityContainer;

    // drag & drop
    // https://gamedev-resources.com/create-an-in-game-inventory-ui-with-ui-toolkit/
    bool _isDragging;

    AbilitySlot _originalSlot;
    AbilitySlot _newSlot;
    VisualElement _dragDropContainer;
    AbilityButton _draggedAbility;

    List<AbilitySlot> _allSlots = new();

    List<CharacterCard> _allCards = new();

    const string _ussDragDropContainer = "dashboard__ability-drag-drop-container";


    public void Initialize(VisualElement root, VisualElement abilityContainer)
    {
        _gameManager = GameManager.Instance;
        _deskManager = DeskManager.Instance;

        _root = root;
        _abilityContainer = abilityContainer;

        _dragDropContainer = new VisualElement();
        _dragDropContainer.AddToClassList(_ussDragDropContainer);
        _root.Add(_dragDropContainer);

        _root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        _root.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    public void AddDraggableAbility(AbilityButton abilityButton) { abilityButton.RegisterCallback<PointerDownEvent>(OnAbilityPointerDown); }

    public void AddCharacterCard(CharacterCard card)
    {
        _allCards.Add(card);
        _allSlots.AddRange(card.AbilitySlots);
        foreach (AbilityButton el in card.AbilityButtons)
            AddDraggableAbility(el);
    }

    public void RemoveCharacterCard(CharacterCard card)
    {
        foreach (AbilitySlot slot in card.AbilitySlots)
            _allSlots.Remove(slot);

        _allCards.Remove(card);
    }

    void OnAbilityPointerDown(PointerDownEvent evt)
    {
        if (evt.button != 0)
            return;

        AbilityButton abilityButton = (AbilityButton)evt.currentTarget;

        AbilitySlot abilitySlot = null;
        if (abilityButton.parent is AbilitySlot)
        {
            abilitySlot = (AbilitySlot)abilityButton.parent;
            abilitySlot.RemoveButton();
        }
        StartAbilityDrag(evt.position, abilitySlot, abilityButton);
    }

    void StartAbilityDrag(Vector2 position, AbilitySlot originalSlot, AbilityButton draggedAbility)
    {
        _draggedAbility = draggedAbility;
        // _draggedItem.PickedUp();
        _draggedAbility.style.position = Position.Absolute;
        _draggedAbility.style.top = 0;
        _draggedAbility.style.left = 0;

        //Set tracking variables
        _isDragging = true;
        _originalSlot = originalSlot;
        //Set the new position
        _dragDropContainer.BringToFront();
        _dragDropContainer.style.top = position.y - _dragDropContainer.layout.height / 2;
        _dragDropContainer.style.left = position.x - _dragDropContainer.layout.width / 2;
        //Set the image
        _dragDropContainer.Add(_draggedAbility);
        //Flip the visibility on
        _dragDropContainer.style.visibility = Visibility.Visible;
    }

    void OnPointerMove(PointerMoveEvent evt)
    {
        //Only take action if the player is dragging an item around the screen
        if (!_isDragging)
            return;

        if (_draggedAbility != null)
        {
            //Set the new position
            _dragDropContainer.style.left = evt.position.x - _dragDropContainer.layout.width / 2;
            _dragDropContainer.style.top = evt.position.y - _dragDropContainer.layout.height / 2;
        }
    }

    void OnPointerUp(PointerUpEvent evt)
    {
        if (!_isDragging)
            return;

        if (_draggedAbility != null)
            HandleAbilityPointerUp();
    }

    void HandleAbilityPointerUp()
    {
        IEnumerable<AbilitySlot> slots = _allSlots.Where(x =>
               x.worldBound.Overlaps(_dragDropContainer.worldBound));

        List<VisualElement> characterCards = _root.Query(className: "character-card-mini__main").ToList();
        IEnumerable<VisualElement> overlappingCards = characterCards.Where(x =>
                    x.worldBound.Overlaps(_dragDropContainer.worldBound));

        if (slots.Count() != 0)
        {
            AddAbilityToClosestSlot(slots);
            return;
        }

        // drag item from desk to character to add it to character
        if (overlappingCards.Count() != 0)
        {
            AddAbilityToCharacter(overlappingCards);
            return;
        }

        // move item around the desk - update & remember position
        if (_originalSlot != null)
            _gameManager.AddAbilityToPouch(_draggedAbility.Ability);

        _abilityContainer.Add(_draggedAbility);
        SetDraggedAbilityPosition(new Vector2(_dragDropContainer.style.left.value.value,
             _dragDropContainer.style.top.value.value - _abilityContainer.worldBound.y));
        DragCleanUp();
    }

    void AddAbilityToClosestSlot(IEnumerable<AbilitySlot> slots)
    {
        _newSlot = slots.OrderBy(x => Vector2.Distance
           (x.worldBound.position, _dragDropContainer.worldBound.position)).First();

        if (_newSlot.AbilityButton != null)
        {
            _newSlot.AbilityButton.Ability.UpdateDeskPosition(_dragDropContainer.worldBound.position);
            _deskManager.SpitAbilityOntoDesk(_newSlot.AbilityButton.Ability);
            _newSlot.RemoveButton();
        }

        _newSlot.AddButton(_draggedAbility);
        DragCleanUp();

        _gameManager.SaveJsonData();
    }

    void SetDraggedAbilityPosition(Vector2 newPos)
    {
        _draggedAbility.style.left = newPos.x;
        _draggedAbility.style.top = newPos.y;
        _draggedAbility.Ability.UpdateDeskPosition(newPos);
    }

    void AddAbilityToCharacter(IEnumerable<VisualElement> overlappingCards)
    {
        VisualElement closesEl = overlappingCards.OrderBy(x => Vector2.Distance
                         (x.worldBound.position, _dragDropContainer.worldBound.position)).First();
        CharacterCardMini closestCard = (CharacterCardMini)closesEl;
        if (closestCard.Character.CanTakeAnotherItem())
        {
            closestCard.Character.AddAbility(_draggedAbility.Ability);
            _gameManager.RemoveAbilityFromPouch(_draggedAbility.Ability);
            DragCleanUp();
            return;
        }

        ShakeReturnAbilityToContainer(_draggedAbility);
        DragCleanUp();
    }

    async void ShakeReturnAbilityToContainer(AbilityButton abilityButton)
    {
        _abilityContainer.Add(abilityButton);
        abilityButton.style.position = Position.Absolute;
        abilityButton.style.left = _dragDropContainer.worldBound.xMin;
        abilityButton.style.top = _dragDropContainer.worldBound.yMin - _abilityContainer.worldBound.y; ;
        int endLeft = Mathf.CeilToInt(_dragDropContainer.worldBound.xMin) + Random.Range(-50, 50);
        int endTop = Mathf.CeilToInt(_dragDropContainer.worldBound.yMin) + Random.Range(-50, 50);

        // when item is shaking and you grab it it behaves weirdly.
        abilityButton.UnregisterCallback<PointerDownEvent>(OnAbilityPointerDown);
        DOTween.To(() => abilityButton.style.left.value.value, x => abilityButton.style.left = x, endLeft, 0.5f)
                .SetEase(Ease.OutElastic);
        await DOTween.To(() => abilityButton.style.top.value.value, x => abilityButton.style.top = x, endTop, 0.5f)
                .SetEase(Ease.OutElastic).AsyncWaitForCompletion();
        abilityButton.RegisterCallback<PointerDownEvent>(OnAbilityPointerDown);

        abilityButton.Ability.UpdateDeskPosition(new Vector2(endLeft, endTop));
    }

    void DragCleanUp()
    {
        //Clear dragging related visuals and data
        _isDragging = false;

        _originalSlot = null;
        _draggedAbility = null;

        _dragDropContainer.Clear();
        _dragDropContainer.style.visibility = Visibility.Hidden;
    }

    public void RemoveDragContainer()
    {
        if (_dragDropContainer != null)
        {
            _root.Remove(_dragDropContainer);
            _dragDropContainer = null;
        }
    }

}
