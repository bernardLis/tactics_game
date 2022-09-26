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

    [Header("Scene set-up")]
    [SerializeField] GameObject _boulderBlockingRats;

    public Vector3 testVector1 = new();
    public Vector3 testVector2 = new();

    string _textToDraw1 = "Tutorial baby!!!111";
    string _textToDraw2 = "Important text! Read carefully <3";

    int _timesMovedMovepoint = 0;
    bool _wasRatBlockingBoulderPushed;
    int _ratBlockingBoulderPushedCount = 0;
    int _ratsKilled = 0;
    GameObject[] _enemies;

    void Start()
    {
        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;
        BattleCharacterController.OnCharacterStateChanged += OnCharacterStateChanged;
        MovePointController.OnMove += OnMovePointControllerMove;

        _battleCharacterController = BattleCharacterController.Instance;
        _movePointController = MovePointController.Instance;

        _boulderBlockingRats.GetComponent<PushableObstacle>().OnPushed += OnRatBlockingBoulderPushed;

        _enemies = GameObject.FindGameObjectsWithTag(Tags.Enemy);
        foreach (GameObject enemy in _enemies)
            enemy.GetComponent<CharacterStats>().OnCharacterDeath += OnEnemyDeath;
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
            _textToDraw2 = "or by clicking on the buttons.";
        }
    }

    void HandleSelectingInteractionTarget()
    {
        if (TurnManager.CurrentTurn == 1)
        {
            _textToDraw1 = "You can change selected ability ";
            _textToDraw2 = "or hover over tile you want to interact with and press 'f'.";
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

    void HandleSelectingFaceDir()
    {
        if (TurnManager.CurrentTurn == 1)
        {
            _textToDraw1 = "Select face direction with arrows";
            _textToDraw2 = "or click on the thingy.";
        }
    }

    public override void DrawShapes(Camera cam)
    {
        using (Draw.Command(cam))
        {
            Draw.Text(new Vector3(-2.5f, 7f), _textToDraw1, TextAlign.Left, 4, Color.black);
            Draw.Text(new Vector3(-2.5f, 6.5f), _textToDraw2, TextAlign.Left, 4, Color.black);

            if (DrawPushBoulderText())
            {
              //  Draw.Text(new Vector3(3.5f, -0.5f), "Boulder >>>>", TextAlign.Left, 4, Color.white);
              //  Draw.Text(new Vector3(7.5f, -0.5f), "Push it with 'Push' ability.", TextAlign.Left, 4, Color.white);
            }

            if (DrawPushAgainText())
            {
                Draw.Text(new Vector3(1f,-0.5f), "Good job!", TextAlign.Left, 4, Color.white);
                Draw.Text(new Vector3(4.5f,-0.5f), "Push it one more time.", TextAlign.Left, 4, Color.white);
            }

            if (DrawBattleText())
            {
                // Draw.Text(testVector1, "Attacking from the front or sides allows opponent to retaliate.", TextAlign.Left, 4, Color.black);
                Draw.Text(new Vector3(4.5f,-0.5f), "Now deal with those pesky critters.", TextAlign.Left, 4, Color.white);
            }
        }
    }

    void OnRatBlockingBoulderPushed()
    {
        _wasRatBlockingBoulderPushed = true;
        _ratBlockingBoulderPushedCount++;
    }

    bool DrawPushBoulderText()
    {
        if (_wasRatBlockingBoulderPushed)
            return false;
        if (TurnManager.CurrentTurn < 2)
            return false;

        return true;
    }

    bool DrawPushAgainText()
    {
        if (_ratBlockingBoulderPushedCount > 0 && _ratBlockingBoulderPushedCount < 2)
            return true;

        return false;
    }

    bool DrawBattleText()
    {
        if (_ratBlockingBoulderPushedCount >= 2 && _ratsKilled == 1)
            return true;
        return false;
    }

    void OnEnemyDeath(GameObject obj) { _ratsKilled++; }
}
