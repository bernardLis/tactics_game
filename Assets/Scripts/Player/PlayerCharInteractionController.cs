using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class PlayerCharInteractionController : CharacterInteractionController
{
    Camera cam;

    PlayerCharMovementController playerCharMovementController;
    PlayerStats playerStats;

    // UI
    AbilityUI abilityUI;
    UIDocument UIDocument;
    VisualElement characterAction;
    Button QButton;
    Button WButton;
    Button EButton;
    Button RButton;

    PlayerInput playerInput;

    bool inHighlight = false;


    protected override void Awake()
    {
        base.Awake();

        // TODO: Supposedly, this is an expensive call
        cam = Camera.main;

        playerStats = GetComponent<PlayerStats>();
        playerCharMovementController = GetComponent<PlayerCharMovementController>();

        // getting ui elements
        // TODO: maybe I should migrate UI to UI class and call it from here.
        UIDocument = GetComponent<UIDocument>();
        abilityUI = GameUI.instance.GetComponent<AbilityUI>();

        var rootVisualElement = UIDocument.rootVisualElement;

        characterAction = rootVisualElement.Q<VisualElement>("CharacterAction");

        // get interaction buttons
        QButton = rootVisualElement.Q<Button>("QButton");
        WButton = rootVisualElement.Q<Button>("WButton");
        EButton = rootVisualElement.Q<Button>("EButton");
        RButton = rootVisualElement.Q<Button>("RButton");

        // register interaction callbacks (buttons)
        QButton.clickable.clicked += QButtonClicked;
        WButton.clickable.clicked += WButtonClicked;
        EButton.clickable.clicked += EButtonClicked;
        RButton.clickable.clicked += RButtonClicked;
    }

    void OnEnable()
    {
        // TODO: does this cost a lot, can I do something smarter
        // maybe characters don't need interaction controllers, they just need to know what abilities they have and I will 
        playerInput = MovePointController.instance.GetComponent<PlayerInput>();

        // inputs
        playerInput.actions["QButtonClick"].performed += ctx => QButtonClickInput();
        playerInput.actions["WButtonClick"].performed += ctx => WButtonClickInput();
        playerInput.actions["EButtonClick"].performed += ctx => EButtonClickInput();
        playerInput.actions["RButtonClick"].performed += ctx => RButtonClickInput();

        playerInput.actions["Back"].performed += ctx => BackClickInput();

        // UI
        characterAction.style.display = DisplayStyle.Flex;

        MovePointController.instance.blockMovePoint = true;

        QButton.style.backgroundImage = new StyleBackground(playerStats.abilities[0].aIcon);
        WButton.style.backgroundImage = new StyleBackground(playerStats.abilities[1].aIcon);
        EButton.style.backgroundImage = new StyleBackground(playerStats.abilities[2].aIcon);
        RButton.style.backgroundImage = new StyleBackground(playerStats.abilities[3].aIcon);
    }

    void OnDisable()
    {
        characterAction.style.display = DisplayStyle.None;
        selectedAbility = null;
        
        if(playerInput == null)
            return;
        
        playerInput.actions["QButtonClick"].performed -= ctx => QButtonClickInput();
        playerInput.actions["WButtonClick"].performed -= ctx => WButtonClickInput();
        playerInput.actions["EButtonClick"].performed -= ctx => EButtonClickInput();
        playerInput.actions["RButtonClick"].performed -= ctx => RButtonClickInput();

        playerInput.actions["Back"].performed -= ctx => BackClickInput();

    }

    void QButtonClicked()
    {
        if (playerStats.currentMana >= playerStats.abilities[0].manaCost)
        {
            ButtonClick();
            selectedAbility = playerStats.abilities[0];
            playerStats.abilities[0].HighlightTargetable();
        }
        else
        {
            Debug.Log("Not enough mana");
        }
    }

    void WButtonClicked()
    {
        if (playerStats.currentMana >= playerStats.abilities[1].manaCost)
        {
            ButtonClick();
            selectedAbility = playerStats.abilities[1];
            playerStats.abilities[1].HighlightTargetable();
        }
        else
        {
            Debug.Log("Not enough mana");
        }
    }

    void EButtonClicked()
    {
        if (playerStats.currentMana >= playerStats.abilities[2].manaCost)
        {
            ButtonClick();
            selectedAbility = playerStats.abilities[2];
            playerStats.abilities[2].HighlightTargetable();
        }
        else
        {
            Debug.Log("Not enough mana");
        }
    }

    void RButtonClicked()
    {
        if (playerStats.currentMana >= playerStats.abilities[3].manaCost)
        {
            ButtonClick();
            selectedAbility = playerStats.abilities[3];
            playerStats.abilities[3].HighlightTargetable();
        }
        else
        {
            Debug.Log("Not enough mana");
        }
    }

    void BackClickInput()
    {
        // if we are in highlight targetable - clear highlight, show ui
        if (inHighlight)
        {
            inHighlight = false;

            highlighter.ClearHighlightedTiles();

            selectedAbility = null;
            abilityUI.HideAbilityUI();

            // move movepoint back to character & block it
            MovePointController.instance.transform.position = transform.position;
            MovePointController.instance.blockMovePoint = true;
            MovePointController.instance.UpdateTileInfoUI();

            // display UI
            characterAction.style.display = DisplayStyle.Flex;
        }
        // else go back to the previous position
        else
        {
            HideUI();

            playerCharMovementController.enabled = true;
            playerCharMovementController.BackClickInput();

            this.enabled = false;
        }
    }

    public void SetPosition()
    {
        //characterAction.style.display = DisplayStyle.Flex;
        Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(characterAction.panel, transform.position, cam);
        characterAction.transform.position = new Vector3(newPosition.x - characterAction.layout.width / 2, newPosition.y, transform.position.z);
    }

    // TODO: should I avoid using late update to set the positon, is it taxing on performance? 
    // it works perfectly...
    // I am enabling/disabling this script on show/hide the panel
    void LateUpdate()
    {
        SetPosition();
    }

    // wrapper function for utilities on button click
    void ButtonClick()
    {
        MovePointController.instance.blockMovePoint = false;

        inHighlight = true;
        HideUI();
    }

    public void HideUI()
    {
        characterAction.style.display = DisplayStyle.None;
    }

    void QButtonClickInput()
    {
        // https://forum.unity.com/threads/trigger-button-click-from-code.1124356/
        using (var e = new NavigationSubmitEvent() { target = QButton })
            QButton.SendEvent(e);
    }
    void WButtonClickInput()
    {
        using (var e = new NavigationSubmitEvent() { target = WButton })
            WButton.SendEvent(e);
    }
    void EButtonClickInput()
    {
        using (var e = new NavigationSubmitEvent() { target = EButton })
            EButton.SendEvent(e);
    }
    void RButtonClickInput()
    {
        using (var e = new NavigationSubmitEvent() { target = RButton })
            RButton.SendEvent(e);
    }

    public override void FinishCharacterTurn()
    {
        // clearing the highlight
        highlighter.ClearHighlightedTiles();

        // clear flags
        inHighlight = false;
        selectedAbility = null;

        // finish character's turn after the interaction is performed
        MovePointController.instance.UnselectSelected();
        TurnManager.instance.PlayerCharacterTurnFinished();

        // disable self
        this.enabled = false;
    }
}
