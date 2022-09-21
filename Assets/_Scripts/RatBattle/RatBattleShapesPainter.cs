using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class RatBattleShapesPainter : ImmediateModeShapeDrawer
{
    BattleCharacterController _battleCharacterController;

    string _textToDraw = "";

    void Start()
    {
        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;
        BattleCharacterController.OnCharacterStateChanged += OnCharacterStateChanged;

    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
        BattleCharacterController.OnCharacterStateChanged -= OnCharacterStateChanged;

    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state == BattleState.PlayerTurn)
            HandlePlayerTurn();
    }

    void HandlePlayerTurn()
    {
        if (TurnManager.CurrentTurn == 1)
            _textToDraw = "Hello 1";
        if (TurnManager.CurrentTurn == 2)
            _textToDraw = "Hello 2";
    }

    void OnCharacterStateChanged(CharacterState state)
    {
        switch (state)
        {
            case CharacterState.None:
                break;
            case CharacterState.Selected:
                break;
            case CharacterState.Moved:
                break;
            case CharacterState.SelectingInteractionTarget:
                break;
            case CharacterState.SelectingFaceDir:
                break;
            case CharacterState.ConfirmingInteraction:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }


    public override void DrawShapes(Camera cam)
    {
        using (Draw.Command(cam))
        {
            Draw.Text(new Vector3(-2.5f, 6.5f), _textToDraw, TextAlign.Left, 18, Color.blue);
        }
    }
}
