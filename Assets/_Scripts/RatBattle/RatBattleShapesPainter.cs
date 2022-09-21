using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using TMPro;
using System;

public class RatBattleShapesPainter : ImmediateModeShapeDrawer
{
    BattleCharacterController _battleCharacterController;
    MovePointController _movePointController;
    GameObject _playerCharacter;

    string _textToDraw1 = "Tutorial baby!!!111";
    string _textToDraw2 = "Important text! Read carefully <3";

    int _timesMovedMovepoint = 0;

    void Start()
    {
        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;
        BattleCharacterController.OnCharacterStateChanged += OnCharacterStateChanged;
        MovePointController.OnMove += OnMovePointControllerMove;

        _battleCharacterController = BattleCharacterController.Instance;
        _movePointController = MovePointController.Instance;
    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
        BattleCharacterController.OnCharacterStateChanged -= OnCharacterStateChanged;
        MovePointController.OnMove += OnMovePointControllerMove;

    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state == BattleState.PlayerTurn)
            HandlePlayerTurn();
        if (state == BattleState.EnemyTurn)
            HandleEnemyTurn();
    }

    void HandlePlayerTurn()
    {
        if (TurnManager.CurrentTurn == 1)
        {
            _playerCharacter = GameObject.FindGameObjectWithTag(Tags.Player);
            _textToDraw1 = "Move with arrows or mouse.";
            _textToDraw2 = "";
        }

        if (TurnManager.CurrentTurn == 2)
        {
            _textToDraw1 = "Boulder is blocking access to rats.";
            _textToDraw2 = "Push it out of the way.";
        }
    }

    void HandleEnemyTurn()
    {
        _textToDraw1 = "";
        _textToDraw2 = "";
    }

    void OnCharacterStateChanged(CharacterState state)
    {
        switch (state)
        {
            case CharacterState.None:
                HandleCharacterNone();
                break;
            case CharacterState.Selected:
                HandleCharacterSelected();
                break;
            case CharacterState.Moved:
                HandleCharacterMoved();
                break;
            case CharacterState.SelectingInteractionTarget:
                HandleSelectingInteractionTarget();
                break;
            case CharacterState.SelectingFaceDir:
                HandleSelectingFaceDir();
                break;
            case CharacterState.ConfirmingInteraction:
                HandleConfirmingInteraction();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    void OnMovePointControllerMove(Vector3 pos)
    {
        _timesMovedMovepoint++;
        if (TurnManager.CurrentTurn != 1)
            return;
        if (_timesMovedMovepoint < 3)
            return;

        if (_battleCharacterController.CharacterState == CharacterState.None &&
            _movePointController.transform.position != _playerCharacter.transform.position)
        {
            _textToDraw1 = "Move over the character.";
            _textToDraw2 = "";
        }

        if (_battleCharacterController.CharacterState == CharacterState.None &&
            _movePointController.transform.position == _playerCharacter.transform.position)
        {
            _textToDraw1 = "Select character with 'f'.";
            _textToDraw2 = "";
        }
    }

    void HandleCharacterNone()
    {
        if (TurnManager.CurrentTurn == 1)
        {
        }
    }

    void HandleCharacterSelected()
    {
        if (TurnManager.CurrentTurn == 1)
        {
            _textToDraw1 = "You can move to any of the highlighted tiles. ";
            _textToDraw2 = "Hover over one of them and press 'f'.";
        }

    }

    void HandleCharacterMoved()
    {
        if (TurnManager.CurrentTurn == 1)
        {
            _textToDraw1 = "You can select one of the abilities with 'a', 's' and 'q'";
            _textToDraw2 = "or move back by clicking 'b'.";
        }
    }

    void HandleSelectingInteractionTarget()
    {
        if (TurnManager.CurrentTurn == 1)
        {
            _textToDraw1 = "Move to tile you want to interact with and press 'f'.";
            _textToDraw2 = "";
        }

    }

    void HandleSelectingFaceDir()
    {
        if (TurnManager.CurrentTurn == 1)
        {
            _textToDraw1 = "Select face direction with arrows.";
            _textToDraw2 = "";
        }
    }

    void HandleConfirmingInteraction()
    {
        if (TurnManager.CurrentTurn == 1)
        {
            _textToDraw1 = "Press 'f' to confirm interaction.";
            _textToDraw2 = "You can back out by clicking 'b'.";
        }

    }

    public override void DrawShapes(Camera cam)
    {
        using (Draw.Command(cam))
        {
            Draw.Text(new Vector3(-2.5f, 7f), _textToDraw1, TextAlign.Left, 4, Color.black);
            Draw.Text(new Vector3(-2.5f, 6.5f), _textToDraw2, TextAlign.Left, 4, Color.black);
        }
    }
}
