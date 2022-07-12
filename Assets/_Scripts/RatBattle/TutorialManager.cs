using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;

public class TutorialManager : MonoBehaviour
{
    Label _tutorialText;
    VisualElement _tutorialTextContainer;

    string _select = "Click 'F' to select A CHARACTER.";
    string _arrows = "Good job! Use arrows to move around.";
    string _reach = "Blue squares are within CHARACTER's reach.";
    string _move = "Move character by clicking 'F' while hovering over one of the blue squares.";

    string _abilities = "Well done! Now you can use one of your abilities.";
    string _hotkeys0 = "Each ability has a hotkey.";
    string _hotkeys1 = "A and S for basic abilities and QWER for secondary.";
    string _mouse = "You can also click on them with your mouse";
    string _selectAbility = "Select one of the abilities";

    string _abilityHighlight = "Ability reach is highlighted.";
    string _oneTile = "Currently, your abilities reach 1 tile only. It will change as you get stronger.";
    string _options = "You can hit or push the air with ability under A and Q or...";
    string _defend = "Defend your position by selecting S and selecting which way you want to face.";


    string _summary0 = "Summary: move with arrow keys.";
    string _summary1 = "Select with 'F'.";
    string _summary2 = "Abilities have hotkeys: A,S,Q,W,E,R,T.";
    string _summary3 = "One last thing, if you want to go back press 'B'";


    bool _wasSelected;
    bool _hasCharacterMoved;
    bool _hasSelectedAbility;

    void Start()
    {
        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;
        BattleCharacterController.OnCharacterStateChanged += BattleCharacterController_OnCharacterStateChange;

        _tutorialText = BattleUI.Instance.Root.Q<Label>("tutorialText");
        _tutorialTextContainer = BattleUI.Instance.Root.Q<VisualElement>("tutorialTextContainer");
    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
        BattleCharacterController.OnCharacterStateChanged -= BattleCharacterController_OnCharacterStateChange;
    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (TurnManager.BattleState == BattleState.PlayerTurn)
            HandlePlayerTurn();
        if (TurnManager.BattleState == BattleState.EnemyTurn)
            HandleEnemyTurn();

    }

    void BattleCharacterController_OnCharacterStateChange(CharacterState state)
    {
        if (state == CharacterState.None)
            return;
        if (state == CharacterState.Selected)
            HandleCharacterSelected();
        if (state == CharacterState.Moved)
            HandleCharacterMoved();
        if (state == CharacterState.SelectingInteractionTarget)
            HandleAbilitySelected();
        if (state == CharacterState.SelectingFaceDir)
            return;
        if (state == CharacterState.ConfirmingInteraction)
            return;
    }

    async void HandlePlayerTurn()
    {
        if (TurnManager.CurrentTurn == 1)
        {
            _tutorialTextContainer.style.display = DisplayStyle.Flex;
            _tutorialText.text = _select;
        }

        if (TurnManager.CurrentTurn == 2)
        {
            _tutorialTextContainer.style.display = DisplayStyle.Flex;
            _tutorialText.text = _summary0;
            await Task.Delay(5000);
            _tutorialText.text = _summary1;
            await Task.Delay(5000);
            _tutorialText.text = _summary2;
            await Task.Delay(5000);
            _tutorialText.text = _summary3;
            await Task.Delay(5000);
            _tutorialTextContainer.style.display = DisplayStyle.None;
        }
    }

    void HandleEnemyTurn()
    {
        if (TurnManager.CurrentTurn != 1)
            return;

        _tutorialTextContainer.style.display = DisplayStyle.None;
    }

    async void HandleCharacterSelected()
    {
        if (_wasSelected)
            return;
        _wasSelected = true;

        _tutorialTextContainer.style.display = DisplayStyle.Flex;
        _tutorialText.text = _arrows;
        await Task.Delay(5000);
        _tutorialText.text = _reach;
        await Task.Delay(5000);
        _tutorialText.text = _move;
    }

    async void HandleCharacterMoved()
    {
        if (_hasCharacterMoved)
            return;
        _hasCharacterMoved = true;

        _tutorialText.text = _abilities;
        await Task.Delay(5000);
        _tutorialText.text = _hotkeys0;
        await Task.Delay(5000);
        _tutorialText.text = _hotkeys1;
        await Task.Delay(5000);
        _tutorialText.text = _mouse;
        await Task.Delay(5000);
        _tutorialText.text = _selectAbility;
    }

    async void HandleAbilitySelected()
    {

        if (_hasSelectedAbility)
            return;
        _hasSelectedAbility = true;

        _tutorialText.text = _abilityHighlight;
        await Task.Delay(5000);
        _tutorialText.text = _oneTile;
        await Task.Delay(5000);
        _tutorialText.text = _options;
        await Task.Delay(5000);
        _tutorialText.text = _defend;
    }
}
